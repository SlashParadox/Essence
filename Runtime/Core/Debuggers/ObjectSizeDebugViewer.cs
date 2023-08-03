// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A debug viewer to see the size of an object.
    /// </summary>
    public class ObjectSizeDebugViewer : EssenceBehaviour
    {
#if UNITY_EDITOR
        /// <summary>The <see cref="Color"/> of the cube to use.</summary>
        [SerializeField] private Color displayColor = new Color(1.0f, 0.0f, 0.0f, 0.4f);

        /// <summary>If true, the viewer always displays, even if not selected.</summary>
        [SerializeField] private bool alwaysDisplay = true;

        private void Reset()
        {
            hideFlags |= HideFlags.DontSaveInBuild;
        }

        private void OnDrawGizmos()
        {
            if (!alwaysDisplay && UnityEditor.Selection.activeObject != gameObject)
                return;

            Color originalColor = Gizmos.color;
            Gizmos.color = displayColor;
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = originalColor;
        }
#endif
    }
}
#endif