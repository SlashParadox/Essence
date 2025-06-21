using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    [VisualTreePath]
    public class CurveDataBaseField : BindableElement
    {
        public SerializedProperty CurveProperty { get; private set; }

        /// <summary>The <see cref="Button"/> to set the current display mode.</summary>
        private Button _modeButton;

        /// <summary>The element to add the child class's curve elements to.</summary>
        private VisualElement _curvePlacementField;

        /// <summary>The area that the data table is displayed.</summary>
        private VisualElement _tableArea;

        /// <summary>The property for the <see cref="CurveTableBase{TValue}.tableMode"/>.</summary>
        private SerializedProperty _tableModeProperty;

        /// <summary>If true, the child's curve field is added above the <see cref="_tableArea"/>. Otherwise, it is added below.</summary>
        protected virtual bool AddCurveBeforeTable { get; } = true;

        private VisualElement _curveElementInstance;

        public CurveDataBaseField()
        {
            Initialize();
        }

        public CurveDataBaseField(SerializedProperty property, VisualElement curveElement) : this()
        {
            SetProperty(property);
            SetCurveElement(curveElement);
        }

        protected virtual void Initialize()
        {
            VisualTreeAsset tree = EditorCache.LoadVisualTreeForType(GetType());
            if (tree)
                tree.CloneTree(this);

            _modeButton = this.Q<Button>("ModeButton");
            _curvePlacementField = this.Q<VisualElement>(AddCurveBeforeTable ? "TopCurveField" : "BottomCurveField");
            _tableArea = this.Q<VisualElement>("TableArea");
        }

        public void SetProperty(SerializedProperty property)
        {
            this.BindProperty(property);
            CurveProperty = property;
            _tableModeProperty = property?.FindPropertyRelative("tableMode");

            if (CurveProperty == null || _tableModeProperty == null)
                return;

            Foldout foldout = this.Q<Foldout>("TopFold");
            if (foldout != null)
            {
                foldout.text = CurveProperty.displayName;
                foldout.tooltip = CurveProperty.tooltip;
            }

            // Apply the current display mode.
            UpdateMode();
            if (_modeButton != null)
                _modeButton.clicked += OnModeButtonClicked;
        }

        public virtual void SetCurveElement(VisualElement curveElement)
        {
            if (_curvePlacementField == null)
                return;

            if (_curveElementInstance != null)
                _curvePlacementField.Remove(_curveElementInstance);

            _curveElementInstance = curveElement;
            if (_curveElementInstance != null)
                _curvePlacementField.Add(_curveElementInstance);
        }

        /// <summary>
        /// Updates the current display mode between pure curve editing or table editing.
        /// </summary>
        protected void UpdateMode()
        {
            if (_tableModeProperty == null)
                return;

            string nextMode = _tableModeProperty.boolValue ? "Curve" : "Table";
            if (_modeButton != null)
                _modeButton.text = $"Switch To {nextMode} Mode";

            _curvePlacementField?.SetEnabled(!_tableModeProperty.boolValue);

            DisplayStyle display = _tableModeProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            if (_tableArea != null)
                _tableArea.style.display = new StyleEnum<DisplayStyle>(display);
        }

        /// <summary>
        /// An event called when the <see cref="_modeButton"/> is clicked.
        /// </summary>
        private void OnModeButtonClicked()
        {
            if (_tableModeProperty == null)
                return;

            _tableModeProperty.boolValue = !_tableModeProperty.boolValue;
            _tableModeProperty.serializedObject.ApplyModifiedProperties();

            UpdateMode();
        }
    }
}
