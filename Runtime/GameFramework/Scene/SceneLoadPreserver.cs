using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A component that registers the attaching <see cref="GameObject"/> to preserve between non-destructive scene transitions.
    /// </summary>
    public class SceneLoadPreserver : EssenceBehaviour
    {
        /// <summary>Tags representing this object's load preservation categories. If this contains a requested tag, the object is safe between loads.</summary>
        [SerializeField] private ObjectTagGroup preservationCategories;

        /// <summary>
        /// Checks if this preserver has the given category.
        /// </summary>
        /// <param name="inCategory">The category tag to check.</param>
        /// <returns>Returns if this object has the given category and can be preserved.</returns>
        public bool HasCategory(ObjectTag inCategory)
        {
            return preservationCategories.HasTag(inCategory);
        }
    }
}
