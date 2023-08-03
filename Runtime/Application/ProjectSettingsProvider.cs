// Copyright (c) Craig Williams, SlashParadox

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An enhanced <see cref="UnityEditor.SettingsProvider"/> for <see cref="ProjectSettingsObject{T}"/>s.
    /// </summary>
    public class ProjectSettingsProvider : UnityEditor.SettingsProvider
    {
        /// <summary>The original settings object.</summary>
        private readonly ScriptableObject _settingsObject;

        /// <summary>The current <see cref="UnityEditor.Editor"/> made.</summary>
        private UnityEditor.Editor _baseEditor;

        /// <summary>
        /// A constructor for a <see cref="ProjectSettingsProvider"/>.
        /// </summary>
        /// <param name="settingsObj">The original <see cref="ScriptableObject"/> for the settings.</param>
        /// <param name="path">The display path in the menu.</param>
        /// <param name="scope">The scope of the settings.</param>
        public ProjectSettingsProvider(ScriptableObject settingsObj, string path, UnityEditor.SettingsScope scope)
            : base(path, scope)
        {
            _settingsObject = settingsObj;
            UnityEditor.SerializedObject serializedSettingsObject = new UnityEditor.SerializedObject(_settingsObject);

            keywords = GetSearchKeywordsFromSerializedObject(serializedSettingsObject);
        }

        /// <summary>
        /// Gets a provider for a given type of <see cref="ProjectSettingsObject{T}"/>.
        /// </summary>
        /// <param name="settings">The <see cref="ProjectSettingsObject{T}"/> to get the provider for.</param>
        /// <typeparam name="T">The type of the <paramref name="settings"/>.</typeparam>
        /// <returns>Returns the <see cref="UnityEditor.SettingsProvider"/>.</returns>
        public static UnityEditor.SettingsProvider GetProvider<T>(ProjectSettingsObject<T> settings) where T : ProjectSettingsObject<T>
        {
            if (settings == null)
                return null;

            ProjectSettingsAttribute attribute = settings.SettingsAttribute;
            if (attribute == null)
                return null;

            string displayPath = $"{attribute.SettingsType.DisplayPath}{(!string.IsNullOrEmpty(attribute.DisplayPath) ? attribute.DisplayPath : nameof(T))}";
            if (string.IsNullOrEmpty(displayPath))
                return null;

            UnityEditor.SettingsScope scope = attribute.SettingsType.Equals(ProjectSettingsType.User) ? UnityEditor.SettingsScope.User : UnityEditor.SettingsScope.Project;
            return new ProjectSettingsProvider(settings, displayPath, scope);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (_baseEditor == null)
                _baseEditor = UnityEditor.Editor.CreateEditor(_settingsObject);

            base.OnActivate(searchContext, rootElement);
        }

        public override void OnDeactivate()
        {
            Object.DestroyImmediate(_baseEditor);
            _baseEditor = null;
            base.OnDeactivate();
        }

        public override void OnGUI(string searchContext)
        {
            if (_settingsObject == null || _baseEditor == null)
                return;

            GUILayout.BeginVertical();
            GUILayout.Space(10.0f);
            _baseEditor.OnInspectorGUI();
            GUILayout.EndVertical();
        }
    }
#endif
}