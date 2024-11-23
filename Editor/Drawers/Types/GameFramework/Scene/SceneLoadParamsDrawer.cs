using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(SceneLoadParams))]
    public class SceneLoadParamsDrawer : EssencePropertyDrawer
    {
        private VerticalPropertyGroup vGroup = new VerticalPropertyGroup();
        private FoldoutPropertyLabel _foldoutLabel = new FoldoutPropertyLabel();
        protected override bool IsDrawerValid(SerializedProperty property)
        {
            SerializedProperty unloadProperty = property.FindPropertyRelative(nameof(SceneLoadParams.UnloadCurrentScenes));
            SerializedProperty activeProperty = property.FindPropertyRelative(nameof(SceneLoadParams.UpdateActiveScene));

            return unloadProperty != null && activeProperty != null;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            SerializedProperty unloadProperty = property.FindPropertyRelative(nameof(SceneLoadParams.UnloadCurrentScenes));
            SerializedProperty activeProperty = property.FindPropertyRelative(nameof(SceneLoadParams.UpdateActiveScene));

            _foldoutLabel.Label = label;
            vGroup.Label = _foldoutLabel;
            vGroup.ClearItems();
            vGroup.AddItem(new FunctionalPropertyItem<bool>(rect =>
                                                            {
                                                                unloadProperty.boolValue = EditorGUI.Toggle(rect, unloadProperty.name, unloadProperty.boolValue);
                                                                return unloadProperty.boolValue;
                                                            }));
            vGroup.AddItem(new FunctionalPropertyItem<bool>(rect =>
                                                            {
                                                                bool enabled = GUI.enabled;
                                                                GUI.enabled = enabled && !unloadProperty.boolValue;
                                                                activeProperty.boolValue = EditorGUI.Toggle(rect, activeProperty.name, activeProperty.boolValue);
                                                                GUI.enabled = enabled;
                                                                return activeProperty.boolValue;
                                                            }));

            vGroup.Draw(ref position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return vGroup.GetHeight();
        }
    }
}
