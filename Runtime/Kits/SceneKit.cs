using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A kit for handling Unity <see cref="Scene"/>s.
    /// </summary>
    public class SceneKit
    {
        /// <summary>
        /// Gets all <see cref="Component"/>s of a specified type in a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The <see cref="Scene"/> to check.</param>
        /// <param name="checkChildren">If true, children of the <paramref name="scene"/>'s root objects are also checked.</param>
        /// <typeparam name="T">The type of the <see cref="Component"/>.</typeparam>
        /// <returns>Returns all found components.</returns>
        public static List<T> GetComponentsInScene<T>(Scene scene, bool checkChildren) where T : Component
        {
            List<T> results = new List<T>();
            GameObject[] objs = scene.GetRootGameObjects();
            foreach (GameObject obj in objs)
            {
                results.AddRange(checkChildren ? obj.GetComponentsInChildren<T>() : obj.GetComponents<T>());
            }

            return results;
        }

        /// <summary>
        /// Gets the first matching <see cref="Component"/> of a specified type in a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The <see cref="Scene"/> to check.</param>
        /// <param name="checkChildren">If true, children of the <paramref name="scene"/>'s root objects are also checked.</param>
        /// <typeparam name="T">The type of the <see cref="Component"/>.</typeparam>
        /// <returns>Returns the found component.</returns>
        public static T GetFirstComponentInScene<T>(Scene scene, bool checkChildren) where T : Component
        {
            GameObject[] objs = scene.GetRootGameObjects();
            foreach (GameObject obj in objs)
            {
                T foundComponent = checkChildren ? obj.GetComponentInChildren<T>() : obj.GetComponent<T>();
                if (foundComponent)
                    return foundComponent;
            }

            return null;
        }
    }
}
