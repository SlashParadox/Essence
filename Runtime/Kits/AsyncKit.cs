using System.Threading;
using UnityEngine;

namespace SlashParadox.Essence
{
    public static class AsyncKit
    {
        /// <summary>
        /// Checks if an asynchronous task should be cancelled. This checks against the given <see cref="token"/>
        /// as well as the <see cref="Application.exitCancellationToken"/>.
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken"/> to check.</param>
        /// <returns>Returns if the asynchronous task should be cancelled.</returns>
        public static bool ShouldCancelTasks(CancellationToken token)
        {
            return Application.exitCancellationToken.IsCancellationRequested || token.IsCancellationRequested;
        }
        
        /// <summary>
        /// Checks if an asynchronous task should be cancelled. This checks against the <see cref="Application.exitCancellationToken"/>
        /// and the given <paramref name="behaviour"/>'s <see cref="MonoBehaviour.destroyCancellationToken"/>.
        /// </summary>
        /// <param name="behaviour">The <see cref="MonoBehaviour"/> to check the <see cref="CancellationToken"/> on.</param>
        /// <returns>Returns if the asynchronous task should be cancelled.</returns>
        public static bool ShouldCancelTasks(this MonoBehaviour behaviour)
        {
            if (behaviour && behaviour.destroyCancellationToken.IsCancellationRequested)
                return true;
            
            return Application.exitCancellationToken.IsCancellationRequested;
        }

        /// <summary>
        /// Checks if an asynchronous task should be cancelled. This checks against the given <paramref name="token"/>
        /// as well as the <see cref="Application.exitCancellationToken"/> and the given <paramref name="behaviour"/>'s
        /// <see cref="MonoBehaviour.destroyCancellationToken"/>.
        /// </summary>
        /// <param name="behaviour">The <see cref="MonoBehaviour"/> to check the <see cref="CancellationToken"/> on.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to check.</param>
        /// <returns>Returns if the asynchronous task should be cancelled.</returns>
        public static bool ShouldCancelTasks(this MonoBehaviour behaviour, CancellationToken token)
        {
            if (behaviour && behaviour.destroyCancellationToken.IsCancellationRequested)
                return true;
            
            return Application.exitCancellationToken.IsCancellationRequested || token.IsCancellationRequested;
        }
    }
}
