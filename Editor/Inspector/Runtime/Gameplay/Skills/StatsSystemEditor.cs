// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Gameplay.Skills;
using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    /// <summary>
    /// A custom editor for a <see cref="StatsSystem"/>
    /// </summary>
    [CustomEditor(typeof(StatsSystem), true)]
    public class StatsSystemEditor : EssenceEditor
    {
        /// <summary>A <see cref="Foldout"/> to show the current <see cref="ActiveStat"/>s.</summary>
        private Foldout _activeStatsFoldout;

        /// <summary>A <see cref="ListView"/> to show the current <see cref="ActiveStat"/>s.</summary>
        private ListView _activeStatsView;

        /// <summary>The currently displayed <see cref="ActiveStat"/>s.</summary>
        private List<ActiveStat> _currentActiveStats;

        /// <summary>The amount of time to wait before refreshing the displayed <see cref="ActiveStat"/>s.</summary>
        [SerializeField] private int activeStatRefreshMs = 500;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = CreateDefaultInspectorElement();

            CreateActiveStatsSection(root);

            return root;
        }

        /// <summary>
        /// Creates a display of the current <see cref="ActiveStat"/>s.
        /// </summary>
        /// <param name="root">The root <see cref="VisualElement"/> to append to.</param>
        private void CreateActiveStatsSection(VisualElement root)
        {
            // Build the foldout.
            _activeStatsFoldout = new Foldout().SetName("ActiveStatsFoldout");
            _activeStatsFoldout.viewDataKey = "ActiveStatsFoldout";
            _activeStatsFoldout.toggleOnLabelClick = true;
            _activeStatsFoldout.value = false;
            root.Add(_activeStatsFoldout);

            // Build the list.
            _activeStatsView = new ListView().SetName("ActiveStatsList");
            _activeStatsView.SetEnabled(false);
            _activeStatsView.reorderMode = ListViewReorderMode.Animated;
            _activeStatsView.reorderable = true;
            _activeStatsView.allowAdd = false;
            _activeStatsView.allowRemove = false;

            // Set up bindings on the list.
            _activeStatsView.makeItem = CreateActiveStatField;
            _activeStatsView.bindItem = BindActiveStatField;
            _activeStatsView.schedule.Execute(OnRefreshActiveStats).Every(activeStatRefreshMs);
            _activeStatsFoldout.Add(_activeStatsView);

            OnRefreshActiveStats();
        }

        /// <summary>
        /// Creates a <see cref="VisualElement"/> for an <see cref="ActiveStat"/>.
        /// </summary>
        /// <returns>Returns the created <see cref="VisualElement"/>.</returns>
        protected virtual VisualElement CreateActiveStatField()
        {
            return new ActiveStatField();
        }

        /// <summary>
        /// An event called when binding data to an active stat element.
        /// </summary>
        /// <param name="element">The <see cref="VisualElement"/> to set data to.</param>
        /// <param name="stat">The appropriate <see cref="ActiveStat"/>.</param>
        /// <param name="index">The index of the <paramref name="stat"/>.</param>
        protected virtual void OnBindActiveStatField(VisualElement element, ActiveStat stat, int index)
        {
            ActiveStatField field = element as ActiveStatField;
            field?.SetActiveStat(stat);
        }

        /// <summary>
        /// Binds data to an <see cref="ActiveStat"/>'s element.
        /// </summary>
        /// <param name="element">The <see cref="VisualElement"/> to set data to.</param>
        /// <param name="index">The index of the <paramref name="element"/>.</param>
        private void BindActiveStatField(VisualElement element, int index)
        {
            ActiveStat stat = _currentActiveStats.IsValidIndex(index) ? _currentActiveStats[index] : null;
            if (element == null || stat == null)
                return;

            OnBindActiveStatField(element, stat, index);
        }

        /// <summary>
        /// Refreshes the displayed <see cref="ActiveStat"/>s.
        /// </summary>
        private void OnRefreshActiveStats()
        {
            if (_activeStatsView == null)
                return;

            StatsSystem statsSystem = target as StatsSystem;
            if (!statsSystem)
                return;

            ref readonly Dictionary<Stat, ActiveStat> ownedStats = ref statsSystem.GetOwnedStatsInternal();
            _currentActiveStats = ownedStats.Values.ToList();
            int count = _currentActiveStats.IsEmptyOrNull() ? 0 : _currentActiveStats.Count;

            _activeStatsFoldout.text = $"Active Stats [Count: {count}]";

            if (_currentActiveStats.IsEmptyOrNull())
            {
                _activeStatsFoldout.SetEnabled(false);
                _activeStatsFoldout.value = false;
                _activeStatsView.visible = false;
                return;
            }

            _activeStatsFoldout.SetEnabled(true);

            _activeStatsView.itemsSource = _currentActiveStats;
            _activeStatsView.RefreshItems();
        }
    }
}
