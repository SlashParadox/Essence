using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base for a node on a data tree, such as a binary tree.
    /// </summary>
    public abstract class DataTreeNode
    {
        /// <summary>The parent of this node.</summary>
        private DataTreeNode _parent;

        /// <summary>All direct child nodes. These are all siblings.</summary>
        private readonly List<DataTreeNode> _children = new List<DataTreeNode>();
        
        /// <summary>If true, this node has direct children.</summary>
        public bool HasChildren { get { return _children.Count > 0; } }

        /// <summary>
        /// Returns the node's <see cref="_parent"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of the node.</typeparam>
        /// <returns>Returns the casted parent node.</returns>
        public TNode GetParent<TNode>() where TNode : DataTreeNode
        {
            return _parent as TNode;
        }
        
        /// <summary>
        /// Adds a node to this node's direct children, if possible.
        /// </summary>
        /// <param name="inNode">The <see cref="DataTreeNode"/> to add.</param>
        /// <returns>Returns if the child was successfully added.</returns>
        public bool AddChild(DataTreeNode inNode)
        {
            if (inNode == null || inNode._parent != null)
                return false;

            if (!CanAddChild(inNode))
                return false;

            inNode._parent = this;
            _children.Add(inNode);

            return true;
        }

        /// <summary>
        /// Removes a node from this node's direct children, if possible.
        /// </summary>
        /// <param name="inNode">The node to remove.</param>
        /// <returns>Returns if the child was successfully removed.</returns>
        public bool RemoveChild(DataTreeNode inNode)
        {
            if (inNode == null || inNode._parent != this)
                return false;

            inNode._parent = null;
            return _children.Remove(inNode);
        }
        
        /// <summary>
        /// Removes this node from its direct parent, if possible.
        /// </summary>
        /// <returns>Returns if the node was successfully removed.</returns>
        public bool RemoveFromParent()
        {
            return _parent?.RemoveChild(this) ?? false;
        }

        /// <summary>
        /// Finds a child node, based on some internal value.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>Returns the matching child, if possible. If this node matches, it will be returned.</returns>
        public DataTreeNode FindChild<TValue>(TValue value)
        {
            if (MatchesValue(value))
                return this;

            foreach (DataTreeNode child in _children)
            {
                DataTreeNode foundNode = child.FindChild(value);
                if (foundNode != null)
                    return foundNode;
            }

            return null;
        }

        /// <summary>
        /// Finds a child node, using a search function.
        /// </summary>
        /// <param name="searchFunc">The function to search for a matching node with.</param>
        /// <typeparam name="TNode">The type of the <see cref="DataTreeNode"/>.</typeparam>
        /// <returns>Returns the matching child, if possible. If this node matches, it will be returned.</returns>
        public TNode FindChild<TNode>(Func<TNode, bool> searchFunc) where TNode : DataTreeNode
        {
            if (searchFunc == null)
                return null;
            
            if (this is TNode node && searchFunc.Invoke(node))
                return node;
            
            foreach (DataTreeNode child in _children)
            {
                TNode foundNode = child.FindChild(searchFunc);
                if (foundNode != null)
                    return foundNode;
            }

            return null;
        }

        /// <summary>
        /// Iterates on all nodes using the given action.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="recursive">If true, the action is recursively applied to the node's children.</param>
        /// <typeparam name="TNode">The type of the <see cref="DataTreeNode"/>.</typeparam>
        public void IterateChildren<TNode>(Action<TNode> action, bool recursive) where TNode : DataTreeNode
        {
            if (action == null)
                return;
            
            foreach (DataTreeNode child in _children)
            {
                action.Invoke(child as TNode);
                
                if (recursive)
                    child.IterateChildren(action, true);
            }
        }

        /// <summary>
        /// Checks if this node matches a given value. Used in <see cref="FindChild{TValue}(TValue)"/>.
        /// </summary>
        /// <param name="value">The value being searched for.</param>
        /// <typeparam name="TValue">The type of the <see cref="value"/>.</typeparam>
        /// <returns>Returns if this node matches the given value.</returns>
        protected abstract bool MatchesValue<TValue>(TValue value);

        /// <summary>
        /// Checks if a given node can be added as a child to this node.
        /// </summary>
        /// <param name="inNode">The node to check.</param>
        /// <returns>Returns if the node can be added.</returns>
        protected abstract bool CanAddChild(DataTreeNode inNode);
    }
}
