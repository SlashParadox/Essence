// Copyright (c) Craig Williams, SlashParadox

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence
{
    public partial class EssenceTagSettings
    {
        /// <summary>
        /// A custom editor for the <see cref="EssenceTagSettings"/>.
        /// </summary>
        [CustomEditor(typeof(EssenceTagSettings))]
        public class EssenceTagSettingsDrawer : UnityEditor.Editor
        {
            private const float TagScrollHeight = 300.0f;
            private const float NodeButtonWidth = 25.0f;

            /// <summary>If true, the tags are displayed in full.</summary>
            private bool _areTagsDisplayed = true;

            /// <summary>The vertical position of the scroll view of the tags.</summary>
            private Vector2 _scrollPos = Vector2.zero;

            /// <summary>The <see cref="FieldInfo"/> for <see cref="EssenceTagSettings._rootTag"/>.</summary>
            private FieldInfo _rootTagInfo;

            /// <summary>The <see cref="FieldInfo"/> for <see cref="EssenceTagSettings.serializedTags"/>.</summary>
            private FieldInfo _serializedTagsInfo;

            /// <summary>The <see cref="MethodInfo"/> for <see cref="EssenceTagSettings.AddTagToTree"/>.</summary>
            private MethodInfo _addTagMethodInfo;

            /// <summary>The <see cref="MethodInfo"/> for <see cref="EssenceTagSettings.RemoveTagFromTree"/>.</summary>
            private MethodInfo _removeTagMethodInfo;

            /// <summary>A tag being added to the registry.</summary>
            private string _tagToAdd = string.Empty;

            /// <summary>A tag being removed from the registry.</summary>
            private string _tagToRemove = string.Empty;

            private void OnEnable()
            {
                _rootTagInfo = ReflectionKit.GetFieldInfo(serializedObject.targetObject, ReflectionKit.DefaultFlags, nameof(_rootTag));
                _serializedTagsInfo = ReflectionKit.GetFieldInfo(serializedObject.targetObject, ReflectionKit.DefaultFlags, nameof(serializedTags));
                _addTagMethodInfo = ReflectionKit.GetMethodInfo<EssenceTagSettings>(nameof(AddTagToTree), ReflectionKit.DefaultFlags);
                _removeTagMethodInfo = ReflectionKit.GetMethodInfo<EssenceTagSettings>(nameof(RemoveTagFromTree), ReflectionKit.DefaultFlags);
            }

            public override void OnInspectorGUI()
            {
                serializedObject.UpdateIfRequiredOrScript();

                if (_rootTagInfo == null || _serializedTagsInfo == null || _addTagMethodInfo == null || _removeTagMethodInfo == null)
                {
                    EditorGUILayout.HelpBox("Unable to add tags due to missing variables and methods!", MessageType.Error);
                    return;
                }

                DisplayTagMap();

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            /// <summary>
            /// Displays the registered <see cref="ObjectTag"/>s.
            /// </summary>
            private void DisplayTagMap()
            {
                _areTagsDisplayed = EditorGUILayout.Foldout(_areTagsDisplayed, new GUIContent("Object Tags", null, "All object tags that are registered."), true);
                if (!_areTagsDisplayed)
                    return;

                ObjectTagData root = _rootTagInfo.GetValue(serializedObject.targetObject) as ObjectTagData;
                List<string> tags = _serializedTagsInfo.GetValue(serializedObject.targetObject) as List<string>;
                if (root == null || tags == null)
                {
                    EditorGUILayout.HelpBox("Unable to find root tag or serialized tags!", MessageType.Error);
                    return;
                }

                ++EditorGUI.indentLevel;

                EditorGUILayout.BeginVertical();

                DisplayAddTagField();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true, GUILayout.ExpandHeight(false), GUILayout.Height(TagScrollHeight));
                DisplayTagChildren(root);

                // Remove the selected tag last to prevent collection modification.
                if (!string.IsNullOrEmpty(_tagToRemove))
                    _removeTagMethodInfo.Invoke(serializedObject.targetObject, new object[] { _tagToRemove });

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                ReflectionKit.SetFieldValue(serializedObject.targetObject, tags, ReflectionKit.DefaultFlags, nameof(serializedTags));
            }

            /// <summary>
            /// Displays a field to add a new tag.
            /// </summary>
            private void DisplayAddTagField()
            {
                EditorGUILayout.BeginHorizontal();

                _tagToAdd = EditorGUILayout.TextField("Add String: ", _tagToAdd);
                if (GUILayout.Button("Add", GUILayout.Width(100.0f)))
                {
                    _addTagMethodInfo.Invoke(serializedObject.targetObject, new object[] { _tagToAdd });
                    _tagToAdd = string.Empty;
                }

                EditorGUILayout.EndHorizontal();
            }

            /// <summary>
            /// Iterates through all children of the given <see cref="ObjectTagData"/> to display them in a foldout map.
            /// </summary>
            /// <param name="root">The root <see cref="ObjectTagData"/>.</param>
            private void DisplayTagChildren(ObjectTagData root)
            {
                root?.IterateChildren<ObjectTagData>(child =>
                                                     {
                                                         ++EditorGUI.indentLevel;

                                                         DisplaySingleTagData(child);
                                                         if (child.IsFoldoutOpen)
                                                             DisplayTagChildren(child);

                                                         --EditorGUI.indentLevel;
                                                     }, false);
            }

            /// <summary>
            /// Displays a single <see cref="ObjectTagData"/>.
            /// </summary>
            /// <param name="data">The tag to display.</param>
            private void DisplaySingleTagData(ObjectTagData data)
            {
                if (data == null)
                    return;

                EditorGUILayout.BeginHorizontal();

                if (data.HasChildren)
                    data.IsFoldoutOpen = EditorGUILayout.Foldout(data.IsFoldoutOpen, data.InternalTag.tag, true);
                else
                    EditorGUILayout.LabelField(data.InternalTag.tag);

                // Display an add button, which starts off the add field with the selected tag.
                if (GUILayout.Button("+", GUILayout.Width(NodeButtonWidth)))
                {
                    _tagToAdd = $"{data.InternalTag.tag}.";
                }

                GUILayout.Space(3.0f);

                // Display a remove button.
                if (GUILayout.Button("-", GUILayout.Width(NodeButtonWidth)))
                {
                    if (EditorUtility.DisplayDialog("Remove Tag?", $"Remove Tag '{data.InternalTag.tag}? Doing so will remove all child tags!", "Yes", "Cancel"))
                        _tagToRemove = data.InternalTag.tag;
                }

                GUILayout.Space(15.0f);

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif