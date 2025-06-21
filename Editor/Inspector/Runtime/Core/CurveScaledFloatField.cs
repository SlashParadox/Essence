using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    [UxmlElement] [VisualTreePath]
    public partial class CurveScaledFloatField : VisualElement
    {
        private SerializedProperty _floatProperty;

        private FloatField _baseValueField;

        private FlexObjectField _curveTableField;

        private TextField _curveNameField;

        private ElementFoldout _curveFoldout;

        public CurveScaledFloatField()
        {
            EditorCache.LoadVisualTreeForType(typeof(CurveScaledFloatField))?.CloneTree(this);
            _baseValueField = this.Q<FloatField>("BaseValueField");
            _curveTableField = this.Q<FlexObjectField>("CurveTableField");
            _curveNameField = this.Q<TextField>("CurveNameField");
            _curveFoldout = this.Q<ElementFoldout>("CurveFoldout");

            _curveTableField?.RegisterValueChangedCallback(OnCurveTableUpdated);
        }

        public CurveScaledFloatField(SerializedProperty inProperty) : this()
        {
            SetProperty(inProperty);
        }

        public void SetProperty(SerializedProperty inProperty)
        {
            _floatProperty = inProperty;
            if (_floatProperty == null)
                return;

            if (_baseValueField != null)
                _baseValueField.label = _floatProperty.displayName;

            _baseValueField?.BindProperty(_floatProperty.FindPropertyRelative("baseValue"));
            _curveTableField?.BindProperty(_floatProperty.FindPropertyRelative("curveTable"));
            _curveNameField?.BindProperty(_floatProperty.FindPropertyRelative("curveKey"));

            OnCurveTableUpdated(ChangeEvent<UnityEngine.Object>.GetPooled(null, _curveTableField?.value));
        }

        private void OnCurveTableUpdated(ChangeEvent<UnityEngine.Object> inEvent)
        {
            if (_curveTableField == null || _curveFoldout == null)
                return;

            _curveFoldout.ToggleVisible = inEvent.newValue;
            _curveFoldout.toggleOnLabelClick = inEvent.newValue;
            Toggle toggle = _curveFoldout.Q<Toggle>();
            toggle.toggleOnLabelClick = inEvent.newValue;

            if (!inEvent.newValue)
                _curveFoldout.value = false;
        }
    }
}
