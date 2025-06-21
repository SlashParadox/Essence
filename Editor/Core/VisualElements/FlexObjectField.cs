using SlashParadox.Essence.Kits;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    [UxmlElement]
    public partial class FlexObjectField : ObjectField
    {
        private Label _objectLabel;

        private string _baseLabel;

        private string _format;

        [UxmlAttribute]
        public string FormatString
        {
            get { return _format; }
            set
            {
                _format = value;
                OnLabelUpdate(ChangeEvent<string>.GetPooled(_baseLabel, _baseLabel));
            }
        }

        public FlexObjectField() : base()
        {
            labelElement.style.minWidth = new StyleLength(Length.Auto());
            labelElement.style.width = new StyleLength(Length.Auto());

            FieldInfo fields = typeof(ObjectField).GetField("m_ObjectFieldDisplay", ReflectionKit.DefaultFlags);
            _objectLabel = ((VisualElement)fields?.GetValue(this))?.Q<Label>();
            _baseLabel = _objectLabel?.text;
            _objectLabel.RegisterValueChangedCallback(OnLabelUpdate);
        }

        private void OnLabelUpdate(ChangeEvent<string> inLabel)
        {
            _baseLabel = inLabel.newValue;
            ((INotifyValueChanged<string>) _objectLabel)?.SetValueWithoutNotify(string.IsNullOrEmpty(_format) ? _baseLabel : _format.Replace("%s", _baseLabel));
        }
    }
}
