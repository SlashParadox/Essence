// Copyright (c) Craig Williams, SlashParadox

using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// An enhanced <see cref="FloatField"/> that allow variable label size based on the text.
    /// </summary>
    [UxmlElement]
    public partial class FlexFloatField : FloatField
    {
        public FlexFloatField()
        {
            labelElement.style.minWidth = new StyleLength(Length.Auto());
            labelElement.style.width = new StyleLength(Length.Auto());
        }
    }
}
