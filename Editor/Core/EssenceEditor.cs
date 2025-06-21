// Copyright (c) Craig Williams, SlashParadox

using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A base class for <see cref="UnityEditor.Editor"/>s.
    /// </summary>
    public abstract class EssenceEditor : UnityEditor.Editor
    {
        /// <summary>A default <see cref="VisualTreeAsset"/> to use. If set, this is used to create a default property GUI.</summary>
        [SerializeField] protected VisualTreeAsset defaultVisualTree;

        public override VisualElement CreateInspectorGUI()
        {
            return defaultVisualTree ? CreateDefaultVisualTree() : CreateDefaultInspectorElement();
        }

        /// <summary>
        /// A quick helper method for creating a default editor.
        /// </summary>
        /// <returns>Returns the <see cref="VisualElement"/> representation of the default inspector.</returns>
        protected VisualElement CreateDefaultInspectorElement()
        {
            VisualElement element = new VisualElement();
            InspectorElement.FillDefaultInspector(element, serializedObject, this);

            return element;
        }

        /// <summary>
        /// Creates a default <see cref="VisualElement"/>, based on the <see cref="defaultVisualTree"/>.
        /// </summary>
        /// <returns>Returns the created <see cref="VisualElement"/>.</returns>
        protected VisualElement CreateDefaultVisualTree()
        {
            if (!defaultVisualTree)
                return null;

            VisualElement root = new VisualElement();
            AppendTreeAsset(ref root, defaultVisualTree);

            return root;
        }

        /// <summary>
        /// Appends a <see cref="VisualTreeAsset"/> to the given <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="tree">The <see cref="VisualElement"/> to add to. If not set, a new element will be created.</param>
        /// <param name="treeAsset">The <see cref="VisualTreeAsset"/> to use.</param>
        /// <returns>Returns the created <see cref="VisualElement"/> for the <paramref name="treeAsset"/>.</returns>
        protected VisualElement AppendTreeAsset(ref VisualElement tree, VisualTreeAsset treeAsset)
        {
            if (!treeAsset)
                return null;

            tree ??= new VisualElement();

            VisualElement newBranch = new VisualElement().SetName(treeAsset.name);
            treeAsset.CloneTree(newBranch);
            tree.Add(newBranch);

            return newBranch;
        }
    }
}
