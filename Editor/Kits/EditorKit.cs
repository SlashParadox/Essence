// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A helper class for <see cref="UnityEditor"/> functionality.
    /// </summary>
    public static class EditorKit
    {
        /// <summary>
        /// An <see cref="Enum"/> for determining the current dragging state of the mouse.
        /// </summary>
        private enum DragState
        {
            /// <summary>No drag is occuring.</summary>
            NoDrag,

            /// <summary>A drag is ready, and is waiting for the mouse to move.</summary>
            Initialized,

            /// <summary>A drag is occuring.</summary>
            Dragging
        }

        /// <summary>The standard horizontal spacing used by labels.</summary>
        public static readonly float LabelHorizontalSpacing = 2.0f;

        /// <summary>The index of the left mouse button in <see cref="Event"/>s.</summary>
        public static readonly int LeftMouseButtonIndex = 0;

        /// <summary>The minimum distance for the mouse to move to allow dragging.</summary>
        private static readonly float MinDragRequirement = 16.0f;

        /// <summary><see cref="MethodInfo"/> for making a scrolling text area, from the <see cref="EditorGUI"/>.</summary>
        private static readonly MethodInfo ScrollableTextAreaInternalMethod;

        /// <summary>The current <see cref="DragState"/>.</summary>
        private static DragState _lastDragState = DragState.NoDrag;

        /// <summary>The initial position of the mouse at the start of a drag.</summary>
        private static Vector2 _dragStartPosition = Vector2.zero;

        static EditorKit()
        {
            ScrollableTextAreaInternalMethod = typeof(EditorGUI).GetMethod("ScrollableTextAreaInternal", ReflectionKit.DefaultFlags);
        }

        /// <summary>
        /// Checks if a <see cref="GUIContent"/> is valid.
        /// </summary>
        /// <param name="inContent">The <see cref="GUIContent"/> to check.</param>
        /// <returns>Returns if the <paramref name="inContent"/> is valid and not none.</returns>
        public static bool IsValidGUIContent(this GUIContent inContent)
        {
            return inContent != null && inContent != GUIContent.none;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the variable stored in a <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check.</param>
        /// <returns></returns>
        public static Type GetVariableType(this SerializedProperty property)
        {
            string[] path = property.propertyPath.Split(ReflectionKit.DefaultPathSeparator);
            return ReflectionKit.GetVariableType(property.serializedObject.targetObject, path);
        }

        /// <summary>
        /// Gets the array index of a <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check.</param>
        /// <returns>Returns the <paramref name="property"/>'s index, or <see cref="Literals.InvalidIndex"/>.</returns>
        public static int GetArrayIndex(this SerializedProperty property)
        {
            return property.isArray ? ReflectionKit.ParseCollectionIndex(property.propertyPath) : Literals.InvalidIndex;
        }

        /// <summary>
        /// Gets the parent <see cref="SerializedProperty"/> of another <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check.</param>
        /// <returns>Returns the parent <see cref="SerializedProperty"/>.</returns>
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            string path = property.propertyPath;

            if (path.EndsWith("]"))
                path = path[..(path.LastIndexOf("Array", StringComparison.Ordinal) - 1)];
            else
                path = path[..path.LastIndexOf('.')];

            return property.serializedObject.FindProperty(path);
        }

        /// <summary>
        /// Draws a <see cref="SerializedProperty"/>, either with its true drawer or the default. Non-layout version.
        /// </summary>
        /// <param name="position">The <see cref="Rect"/> used to position the drawer.</param>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        /// <param name="includeChildren">If true, child properties are also drawn.</param>
        public static void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label = null, bool includeChildren = true)
        {
            PropertyDrawer drawer = EditorCache.GetPropertyDrawer(property);

            if (drawer != null)
                drawer.OnGUI(position, property, label);
            else
                EditorGUI.PropertyField(position, property, label, includeChildren);
        }

        /// <summary>
        /// Checks if a given area is being dragged across by the mouse. This creates a
        /// control ID based on the given <paramref name="dragArea"/>.
        /// </summary>
        /// <param name="dragArea">The space to detect the start of a drag from.</param>
        /// <param name="onDrag">A callback for every drag update.</param>
        /// <returns>Returns if the given zone is being dragged.</returns>
        public static bool CheckDrag(Rect dragArea, Action onDrag = null)
        {
            return CheckDrag(dragArea, GUIUtility.GetControlID(FocusType.Keyboard, dragArea), onDrag);
        }

        /// <summary>
        /// Checks if a given area is being dragged across by the mouse.
        /// </summary>
        /// <param name="dragArea">The space to detect the start of a drag from.</param>
        /// <param name="controlID">The id of the control space.</param>
        /// <param name="onDrag">A callback for every drag update.</param>
        /// <returns>Returns if the given area is being dragged.</returns>
        public static bool CheckDrag(Rect dragArea, int controlID, Action onDrag = null)
        {
            Event currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                {
                    // When the mouse is clicked, check if it is on the wanted zone.
                    if (currentEvent.button == LeftMouseButtonIndex && dragArea.Contains(currentEvent.mousePosition))
                    {
                        _lastDragState = DragState.Initialized; // The drag is initialized.
                        _dragStartPosition = currentEvent.mousePosition; // Initialize the start position.
                        GUIUtility.hotControl = controlID; // Set the controlling ID.
                        EditorGUIUtility.SetWantsMouseJumping(1); // Mouse jumping is now allowed.
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    // When the mouse is let go, all dragging stops.
                    if (_lastDragState != DragState.NoDrag)
                    {
                        EditorGUIUtility.SetWantsMouseJumping(0); // Mouse jumping is not allowed.
                        GUIUtility.hotControl = 0; // reset the controlling ID
                        _lastDragState = DragState.NoDrag; // The drag is not occuring.
                    }

                    break;
                }
                case EventType.MouseDrag:
                {
                    // Return immediately if the control is not correct.
                    if (GUIUtility.hotControl != controlID)
                        return false;

                    // Perform based on the current drag state.
                    switch (_lastDragState)
                    {
                        // Perform based on the current drag state.
                        case DragState.Initialized:
                        {
                            // If the mouse is far enough away, begin dragging.
                            if ((currentEvent.mousePosition - _dragStartPosition).sqrMagnitude > MinDragRequirement)
                                _lastDragState = DragState.Dragging;
                            break;
                        }
                        case DragState.Dragging:
                        {
                            // While dragging, invoke the callback, if it exists.
                            onDrag?.Invoke();
                            break;
                        }
                    }

                    break;
                }
                case EventType.Repaint:
                {
                    // When repainting, show the sliding arrow over the draggable area.
                    EditorGUIUtility.AddCursorRect(dragArea, MouseCursor.SlideArrow);
                    break;
                }
            }

            // Return based off if the control ID is correct, and dragging is occuring.
            return GUIUtility.hotControl == controlID && _lastDragState == DragState.Dragging;
        }

        /// <summary>
        /// Draws a scrollable text area.
        /// </summary>
        /// <param name="position">The position of the text area.</param>
        /// <param name="value">The original value of the text.</param>
        /// <param name="scrollPosition">The current scroll position. Updated to the new position.</param>
        /// <param name="style">The <see cref="GUIStyle"/> to use.</param>
        /// <returns>Returns the final text value.</returns>
        public static string ScrollableTextArea(Rect position, string value, ref Vector2 scrollPosition, GUIStyle style)
        {
            if (ScrollableTextAreaInternalMethod == null)
                return value;

            GUIStyle usedStyle = style ?? EditorStyles.textArea;
            object[] parameters = { position, value, scrollPosition, usedStyle };

            // Draw the area with the method.
            string outValue = (string)ScrollableTextAreaInternalMethod.Invoke(null, parameters);

            // Re-update the scroll position.
            scrollPosition = (Vector2)parameters[2];

            return outValue;
        }
    }
}
#endif