// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A publicized version of the <see cref="HelpBox"/>.
    /// </summary>
    [UxmlElement]
    public partial class HelpBoxField : HelpBox
    {
        /// <summary>The icon <see cref="VisualElement"/>.</summary>
        private readonly VisualElement _helpIcon;

        /// <summary>See: <see cref="IconSize"/></summary>
        private float _iconSize;

        /// <summary>see: <see cref="IconMargin"/></summary>
        private Vector4 _iconMargin;

        /// <summary>The size of the icon.</summary>
        [UxmlAttribute]
        public float IconSize
        {
            get { return _iconSize; }
            set
            {
                _iconSize = value;
                UpdateIconSize();
            }
        }

        /// <summary>The margin of the icon. Order is Top, Right, Bottom, Left.</summary>
        [UxmlAttribute]
        public Vector4 IconMargin
        {
            get { return _iconMargin; }
            set
            {
                _iconMargin = value;
                UpdateIconMargin();
            }
        }

        public HelpBoxField()
        {
            ReflectionKit.GetFieldValue(this, out _helpIcon, "m_Icon");
            if (_helpIcon != null)
                Debug.Log("Success");

            FieldInfo fields = typeof(HelpBox).GetField("m_Icon", ReflectionKit.DefaultFlags);
            _helpIcon = (VisualElement)fields?.GetValue(this);
            _iconSize = 100;

            UpdateIconSize();
            UpdateIconMargin();
        }

        /// <summary>
        /// Updates the size of the <see cref="_helpIcon"/>.
        /// </summary>
        private void UpdateIconSize()
        {
            if (_helpIcon == null)
                return;

            _helpIcon.style.minWidth = new StyleLength(IconSize);
            _helpIcon.style.minHeight = new StyleLength(IconSize);
            _helpIcon.style.marginRight = 5.0f;
        }

        /// <summary>
        /// Updates the <see cref="_helpIcon"/>'s margin.
        /// </summary>
        private void UpdateIconMargin()
        {
            _helpIcon.style.marginTop = _iconMargin.x;
            _helpIcon.style.marginRight = _iconMargin.y;
            _helpIcon.style.marginBottom = _iconMargin.z;
            _helpIcon.style.marginLeft = _iconMargin.w;
        }
    }
}
