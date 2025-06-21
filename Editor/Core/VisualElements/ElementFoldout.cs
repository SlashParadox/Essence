using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector
{
    [UxmlElement]
    public partial class ElementFoldout : Foldout
    {
        protected readonly VisualElement Checkmark;
        protected readonly VisualElement HeaderElement;

        [UxmlAttribute]
        public VisualTreeAsset HeaderAsset
        {
            get { return _headerAsset; }
            set
            {
                _headerAsset = value;
                UpdateHeaderVisual();
            }
        }

        [UxmlAttribute]
        public bool ToggleVisible
        {
            get { return _toggleVisible; }
            set
            {
                _toggleVisible = value;
                UpdateToggleVisibility();
            }
        }

        private bool _toggleVisible = true;

        public VisualElement HeaderField { get; private set; }

        private VisualTreeAsset _headerAsset;

        public ElementFoldout()
        {
            Checkmark = this.Q<VisualElement>("unity-checkmark");
            HeaderElement = Checkmark?.parent;
            Label label = HeaderElement?.Q<Label>();
            HeaderElement.style.flexShrink = new StyleFloat(1.0f);

            if (label != null)
                label.visible = false;

            UpdateHeaderVisual();
            UpdateToggleVisibility();
        }



        private void UpdateHeaderVisual()
        {
            if (HeaderElement == null)
                return;

            if (HeaderField != null)
                HeaderElement.Remove(HeaderField);

            HeaderField = null;

            if (!_headerAsset)
                return;

            HeaderField = _headerAsset.Instantiate();
            HeaderField.style.flexGrow = new StyleFloat(1.0f);
            HeaderElement.Add(HeaderField);
        }

        private void UpdateToggleVisibility()
        {
            if (Checkmark == null)
                return;

            Checkmark.style.display = new StyleEnum<DisplayStyle>(_toggleVisible ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}
