// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Gameplay.Skills;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    /// <summary>
    /// A <see cref="VisualElement"/> for an <see cref="ActiveStat"/>.
    /// </summary>
    [VisualTreePath]
    public class ActiveStatField : BindableElement
    {
        /// <summary>A <see cref="Toggle"/> for showing more information on the stat.</summary>
        protected Toggle InfoFoldout { get; private set; }

        /// <summary>The field for the <see cref="ActiveStat.OwningStat"/>.</summary>
        protected ObjectField StatField { get; }

        /// <summary>The field for the <see cref="ActiveStat.BaseValue"/>.</summary>
        protected FloatField BaseValueField { get; }

        /// <summary>The field for the <see cref="ActiveStat.CurrentValue"/>.</summary>
        protected FloatField CurrentValueField { get; }

        public ActiveStatField()
        {
            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorCache.FindPathForTypeVisualTree(GetType()));
            if (tree)
                tree.CloneTree(this);

            InfoFoldout = this.Q<Toggle>(nameof(InfoFoldout));
            StatField = this.Q<ObjectField>(nameof(StatField));
            BaseValueField = this.Q<FloatField>(nameof(BaseValueField));
            CurrentValueField = this.Q<FloatField>(nameof(CurrentValueField));
        }

        /// <summary>
        /// Sets the displayed <see cref="ActiveStat"/>.
        /// </summary>
        /// <param name="stat">The <see cref="ActiveStat"/> to display.</param>
        public void SetActiveStat(ActiveStat stat)
        {
            if (stat == null)
            {
                visible = false;
                return;
            }

            StatField.SetValueWithoutNotify(stat.OwningStat);
            BaseValueField.SetValueWithoutNotify(stat.BaseValue);
            CurrentValueField.SetValueWithoutNotify(stat.CurrentValue);
        }
    }
}
