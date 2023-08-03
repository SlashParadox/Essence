// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A helpful utility for fitting a game view built for a specific aspect ratio into other aspect ratios. Best for mobile games
    /// by placing on a root object all other objects are children of.
    /// </summary>
    public class ScreenFitter : EssenceBehaviour
    {
        /// <summary>The reference screen size, in width by height.</summary>
        [SerializeField] [Min(1.0f)] private Vector2 referenceAspectRatio = new Vector2(16.0f, 9.0f);

        /// <summary>If true, the Z-axis scale is locked to its initial value.</summary>
        [SerializeField] private bool lockZScale;

#if UNITY_EDITOR
        /// <summary>A reference camera to display the <see cref="referenceAspectRatio"/>.</summary>
        [Header("Editor Settings")] [SerializeField] [ReadOnly] [AutoFind("Main Camera")]
        private Camera referenceCam;

        /// <summary>A reference area to display the <see cref="referenceAspectRatio"/>.</summary>
        [SerializeField] [ReadOnly] [AutoFind("ReferenceArea")]
        private Transform referenceArea;

        /// <summary>The color to highlight the reference work area in.</summary>
        [SerializeField] private Color workAreaColor = new Color(1.0f, 0.0f, 0.0f, 0.15f);

        /// <summary>The color to highlight the resized work area in.</summary>
        [SerializeField] private Color resizedAreaColor = new Color(0.0f, 1.0f, 0.0f, 0.15f);
#endif
        private void Awake()
        {
            FitToScreenSize();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!referenceCam)
                return;

            Vector3[] frustumPoints = new Vector3[4];

            if (referenceCam.usePhysicalProperties)
            {
                if (!UnityEditor.CameraEditorUtils.TryGetSensorGateFrustum(referenceCam, null, frustumPoints, out float _))
                    return;
            }
            else
            {
                if (!UnityEditor.CameraEditorUtils.TryGetFrustum(referenceCam, null, frustumPoints, out float _))
                    return;
            }
            
            Color originalColor = Gizmos.color;

            Vector3 center = MathKit.Midpoint(frustumPoints[0], frustumPoints[2]);
            Vector2 size = new Vector2(Vector3.Distance(frustumPoints[0], frustumPoints[3]), Vector3.Distance(frustumPoints[0], frustumPoints[1]));
            Vector2 refSize = GetMaxAreaByAspectRatio(size, referenceAspectRatio);

            Gizmos.color = workAreaColor;
            Gizmos.DrawCube(center, refSize);
            
            float scaler = GetRequiredScaleFactor(refSize, size);

            Gizmos.color = resizedAreaColor;
            Gizmos.DrawCube(center, refSize * scaler);

            Gizmos.color = originalColor;

            if (referenceArea)
                referenceArea.localScale = refSize;
        }
#endif

        /// <summary>
        /// Rescales the target <see cref="GameObject"/> to the screen's current aspect ratio.
        /// </summary>
        private void FitToScreenSize()
        {
            Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
            Vector2 scaledRefSize = GetMaxAreaByAspectRatio(currentScreenSize, referenceAspectRatio);
            float multipier = GetRequiredScaleFactor(scaledRefSize, currentScreenSize);

            Vector3 currentScale = new Vector3(multipier, multipier, lockZScale ? gameObject.transform.localScale.z : multipier);
            gameObject.transform.localScale = currentScale;
        }

        /// <summary>
        /// Gets the maximum size possible for an aspect ratio within a given screen size.
        /// </summary>
        /// <param name="screenSize">The maximum screen size.</param>
        /// <param name="aspectRatio">The aspect ratio to cut out.</param>
        /// <returns>Returns the most space possible out of the <paramref name="screenSize"/> for the <paramref name="aspectRatio"/>.</returns>
        private Vector2 GetMaxAreaByAspectRatio(Vector2 screenSize, Vector2 aspectRatio)
        {
            bool scaleByHeight = aspectRatio.x <= aspectRatio.y;
            Vector2 refSize = Vector2.one;

            if (scaleByHeight)
            {
                refSize.y = screenSize.y;
                refSize.x = screenSize.y / aspectRatio.y * referenceAspectRatio.x;
            }
            else
            {
                refSize.x = screenSize.x;
                refSize.y = screenSize.x / aspectRatio.x * referenceAspectRatio.y;
            }

            return refSize;
        }

        /// <summary>
        /// Gets the required amount of scaling to apply to a size to fit within a target size.
        /// </summary>
        /// <param name="currentSize">The current size.</param>
        /// <param name="targetSize">The target max size.</param>
        /// <returns>Returns how much to scale <paramref name="currentSize"/> by to fit into the <paramref name="targetSize"/></returns>
        private float GetRequiredScaleFactor(Vector2 currentSize, Vector2 targetSize)
        {
            Vector2 refSize = currentSize;
            float xScaleFactor = 1.0f;
            float yScaleFactor = 1.0f;

            bool scaleByX = refSize.x > targetSize.x;
            if (scaleByX)
            {
                xScaleFactor = targetSize.x / currentSize.x;
                refSize *= xScaleFactor;
            }

            bool scaleByY = refSize.y > targetSize.y;
            if (scaleByY)
            {
                yScaleFactor = targetSize.y / currentSize.y;
            }

            return System.Math.Min(xScaleFactor, yScaleFactor);
        }
    }
}
#endif