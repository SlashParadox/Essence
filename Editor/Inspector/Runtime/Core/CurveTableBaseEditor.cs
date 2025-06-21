using SlashParadox.Essence.Kits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    [CustomEditor(typeof(CurveTableBase<,,>), true)]
    public class CurveTableBaseEditor : EssenceEditor
    {
        private TextField _newCurveField;

        private MultiColumnListView _curveListView;

        private VisualElement _curveSection;
        private PropertyField _curveField;
        private MethodInfo addNewCurveMethod;
        private MethodInfo _getCurveNamesMethod;

        private SerializedProperty _curvesProperty;
        private SerializedProperty _listProperty;

        private List<object> _curveNames = new List<object>();

        private int currentIndex = 0;


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = base.CreateInspectorGUI();
            if (root == null)
                return null;

            _newCurveField = root.Q<TextField>("NewCurveField");
            _curveListView = root.Q<HashMapField>().Q<MultiColumnListView>("DataView");
            _curveSection = root.Q<VisualElement>("CurveSection");

            _curvesProperty = serializedObject.FindProperty("curves");
            _listProperty = _curvesProperty?.FindPropertyRelative("editorData");
            root.Q<HashMapField>().SetSerializedProperty(_curvesProperty);

            addNewCurveMethod = serializedObject.targetObject.GetType().GetMethod("MakeInstance", ReflectionKit.DefaultFlags);
            _getCurveNamesMethod = serializedObject.targetObject.GetType().GetMethod("GetCurveNames", ReflectionKit.DefaultFlags);

            if (_newCurveField != null)
                _newCurveField.RegisterCallback<NavigationSubmitEvent>(OnNewCurveFieldSubmitted);

            if (_curveListView != null)
            {
                _curveListView.reorderable = false;
                _curveListView.selectionChanged += objects =>
                                                   {
                                                       UpdateDisplayedCurve(_curveListView.selectedIndex);
                                                   };

                _curveListView.itemsRemoved += ints =>
                                               {
                                                   if (ints.Contains(currentIndex))
                                                       UpdateDisplayedCurve(-1);
                                               };

                _curveListView.itemIndexChanged += (i, i1) => UpdateDisplayedCurve(-1);
            }

            RefreshCurveListView();
            return root;
        }

        protected void UpdateDisplayedCurve(int index)
        {
            currentIndex = index;
            if (_curveSection == null || _listProperty == null)
                return;

            if (!MathKit.InRange(index, 0, _listProperty.arraySize))
            {
                if (_curveField == null)
                    return;

                _curveSection.Remove(_curveField);
                _curveField = null;
                return;
            }

            if (_curveField == null)
            {

                _curveField = new PropertyField();
                _curveSection.Add(_curveField);
            }

            SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(_curveListView.selectedIndex);
            _curveField.BindProperty(itemProperty?.FindPropertyRelative("value"));
            _curveField.label = "";
        }

        private void OnNewCurveFieldSubmitted(NavigationSubmitEvent inEvent)
        {
            return;
            if (_newCurveField == null)
                return;

            string curveName = _newCurveField.value;
            _newCurveField.SetValueWithoutNotify(string.Empty);

            if (string.IsNullOrWhiteSpace(curveName) || string.IsNullOrEmpty(curveName))
                return;

            SerializedProperty editorDataProperty = _curvesProperty.FindPropertyRelative("editorData");
            if (editorDataProperty == null)
                return;

            editorDataProperty.InsertArrayElementAtIndex(editorDataProperty.arraySize);
            editorDataProperty.GetArrayElementAtIndex(editorDataProperty.arraySize - 1).FindPropertyRelative("key").stringValue = curveName;
            editorDataProperty.GetArrayElementAtIndex(editorDataProperty.arraySize - 1).FindPropertyRelative("value").boxedValue = addNewCurveMethod.Invoke(serializedObject.targetObject, null);
            serializedObject.ApplyModifiedProperties();
            RefreshCurveListView();
            return;

            if (addNewCurveMethod == null)
                return;

            object result = addNewCurveMethod.Invoke(serializedObject.targetObject, new object[]{ curveName });
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
            if (result != null && (bool)result)
                RefreshCurveListView();
        }

        private void RefreshCurveListView()
        {
            return;
            if (_curvesProperty == null || _curveListView == null || _getCurveNamesMethod == null)
                return;

            _curveNames.Clear();
            ICollection curveNames = ((IDictionary)_curvesProperty.boxedValue).Keys;
            foreach (object curve in curveNames)
            {
                _curveNames.Add((string)curve);
            }

            _curveListView.RefreshItems();
        }
    }
}
