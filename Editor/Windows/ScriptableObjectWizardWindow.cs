using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A helper window for creating any <see cref="ScriptableObject"/>.
    /// </summary>
    public class ScriptableObjectWizardWindow : EditorWindow
    {
        public readonly List<MonoScript> Scripts = new List<MonoScript>();

        private string _classSearchString = string.Empty;

        private Vector2 _scrollPos = Vector2.zero;

        private void OnEnable()
        {
            GatherScripts();
            UpdateScriptList();
        }

        private void OnGUI()
        {
            string lastSearch = _classSearchString;
            _classSearchString = EditorGUILayout.TextField("Search: ", _classSearchString, EditorStyles.toolbarSearchField);
            EditorGUILayout.Space();

            if (lastSearch != _classSearchString)
                UpdateScriptList();

            DrawScriptList();
        }

        private void GatherScripts()
        {
            if (Scripts.IsNotEmptyOrNull())
                return;

            string[] assets = AssetDatabase.FindAssets("t: MonoScript");
            foreach (string guid in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (!scriptAsset)
                    continue;

                Type type = scriptAsset.GetClass();
                if (type == null
                    || type.IsAbstract
                    || !type.IsSubclassOf(typeof(ScriptableObject))
                    || type.IsSubclassOf(typeof(UnityEditor.Editor))
                    || type.IsSubclassOf(typeof(EditorWindow)))
                {
                    continue;
                }

                Scripts.Add(scriptAsset);
            }

            SortKit.QuickSort(Scripts, (a, b) => string.Compare(a.GetClass().FullName, b.GetClass().FullName, StringComparison.Ordinal));
        }

        private void UpdateScriptList()
        {
        }

        private void DrawScriptList()
        {
            float height = position.height - 100.0f;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, EditorStyles.textArea, GUILayout.ExpandHeight(false), GUILayout.Height(height));

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 15;
            style.fontStyle = FontStyle.Bold;

            foreach (MonoScript script in Scripts)
            {
                string fullName = script.GetClass().FullName;
                if (!string.IsNullOrEmpty(_classSearchString) && fullName != null && !fullName.Contains(_classSearchString, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (GUILayout.Button(new GUIContent(fullName), style, GUILayout.ExpandWidth(true)))
                {
                    ScriptableObject asset = ScriptableObject.CreateInstance(script.GetClass());
                    ProjectWindowUtil.CreateAsset(asset, $"New{asset.GetType().Name}.asset");
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Creates a card creator window.
        /// </summary>
        [MenuItem("Window/Essence/Scriptable Object Wizard")]
        [MenuItem("Assets/Create/New Scriptable Object", priority = -5000)]
        public static void CreateCardCreatorWindow()
        {
            ScriptableObjectWizardWindow window = GetWindow<ScriptableObjectWizardWindow>("Scriptable Object Wizard");
            window.minSize = new Vector2(500.0f, 500.0f);
        }
    }
}
