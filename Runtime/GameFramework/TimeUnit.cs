// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.GameFramework
{
    /// <summary>
    /// A game framework unit to manage time and timers.
    /// </summary>
    [DisallowMultipleComponent]
    public class TimeUnit : SingletonBehaviour<TimeUnit>
    {
        /// <summary>
        /// Data on a timer.
        /// </summary>
        private class TimerData
        {
            /// <summary>The callback for each run of the timer.</summary>
            private readonly TimerDelegate _callback;

            /// <summary>The target amount of time for the timer to run.</summary>
            private float _time;

            /// <summary>An initial amount of time to wait for the first iteration of the timer.</summary>
            private float _initialDelay;

            /// <summary>The number of iterations left on the timer. If starting at 0 or less, the timer is infinite.</summary>
            private int _iterations;

            /// <summary>The currently compiled time elapsed.</summary>
            private float _currentTime;

            /// <summary>The target maximum time before invoking the <see cref="_callback"/>.</summary>
            private float _currentMaxTime;

            public TimerData(TimerDelegate callback, float time, float initialDelay, int loopCount)
            {
                _callback = callback;
                _time = System.Math.Max(time, 0.0f);
                _initialDelay = System.Math.Max(initialDelay, 0.0f);
                _iterations = System.Math.Max(loopCount, 0);

                _currentTime = 0.0f;
                _currentMaxTime = _time + _initialDelay;
            }

            // ReSharper disable Unity.PerformanceAnalysis
            /// <summary>
            /// Updates the timer, and calls the <see cref="_callback"/> if ready.
            /// </summary>
            /// <param name="deltaTime">The delta time difference since the last update.</param>
            /// <returns>Returns if the timer is ready to be removed.</returns>
            public bool TickTimer(float deltaTime)
            {
                // The timer is invalid. It should have already been removed.
                if (!IsValid())
                    return true;

                _currentTime += deltaTime;
                if (_currentTime < _currentMaxTime)
                    return false;

                // Leave behind any excess time to properly update future loops.
                _currentTime -= _currentMaxTime;

                // An iteration count of 0 means the timer loops infinitely, and merely has to reset some values.
                bool hasEnded = false;
                if (_iterations > 0)
                {
                    --_iterations;

                    // If there are no more iterations, invalidate the timer data.
                    if (_iterations <= 0)
                    {
                        Invalidate();
                        hasEnded = true;
                    }
                }

                // If there are still more iterations, reset to have no more
                if (_iterations >= 0)
                {
                    _initialDelay = 0.0f;
                    _currentMaxTime = _time;
                }

                _callback?.Invoke();
                return hasEnded;
            }

            public bool WillComplete(float deltaTime)
            {
                return IsValid() && _currentTime + deltaTime >= _currentMaxTime;
            }

            /// <summary>
            /// Checks if the timer is still valid.
            /// </summary>
            /// <returns>Returns if the timer is valid.</returns>
            public bool IsValid()
            {
                return _time >= 0.0f;
            }

            /// <summary>
            /// Invalidates the timer.
            /// </summary>
            public void Invalidate()
            {
                _time = -1.0f;
                _iterations = -1;
            }
        }

        public delegate void TimerDelegate();

        /// <summary>A map of handles to their timers.</summary>
        private readonly Dictionary<ID.SharedHandle, TimerData> _timers = new Dictionary<ID.SharedHandle, TimerData>();

        /// <summary>The generator for timer handles.</summary>
        private readonly ID.Generator _timerGenerator = new ID.Generator();

        private void LateUpdate()
        {
            if (_timers.Count <= 0)
                return;

            float deltaTime = Time.deltaTime;
            List<ID.SharedHandle> handles = new List<ID.SharedHandle>(_timers.Keys);
            List<ID.SharedHandle> removedHandles = new List<ID.SharedHandle>();
            foreach (ID.SharedHandle handle in handles)
            {
                if (handle == null)
                    continue;

                if (!handle.IsValid())
                {
                    removedHandles.Add(handle);
                    continue;
                }

                TimerData data = _timers[handle];
                bool hasEnded = data.TickTimer(deltaTime);

                if (hasEnded)
                    removedHandles.Add(handle);
            }

            foreach (ID.SharedHandle removedHandle in removedHandles)
            {
                // Double check that the handle has not been restored.
                if (_timers[removedHandle].IsValid())
                    continue;

                _timers.Remove(removedHandle);
            }
        }

        /// <summary>
        /// Creates a new timer with the given handle. The handle can be null to initialize.
        /// </summary>
        /// <param name="handle">The <see cref="ID.SharedHandle"/> of the timer. If not set, the handle is initialized automatically. If already set, the timer matching
        /// the handle is replaced with a new one.</param>
        /// <param name="time">The amount of time to delay before activating the timer.</param>
        /// <param name="callback">The delegate to invoke at the end of the timer.</param>
        /// <param name="iterations">The number of iterations to invoke the timer for. If zero or less, the timer is infinitely looping.</param>
        /// <param name="initialDelay">An initial delay to apply to the first iteration.</param>
        public static void CreateTimer(ref ID.SharedHandle handle, float time, TimerDelegate callback, int iterations = 1, float initialDelay = 0.0f)
        {
            if (!CurrentSingleton)
                return;

            CurrentSingleton.CreateTimerInternal(ref handle, time, callback, iterations, initialDelay);
        }

        /// <summary>
        /// Creates a new timer.
        /// </summary>
        /// <param name="time">The amount of time to delay before activating the timer.</param>
        /// <param name="callback">The delegate to invoke at the end of the timer.</param>
        /// <param name="iterations">The number of iterations to invoke the timer for. If zero or less, the timer is infinitely looping.</param>
        /// <param name="initialDelay">An initial delay to apply to the first iteration.</param>
        /// <returns>Returns the handle of the timer.</returns>
        public static ID.SharedHandle CreateTimer(float time, TimerDelegate callback, int iterations = 1, float initialDelay = 0.0f)
        {
            if (!CurrentSingleton)
                return null;

            ID.SharedHandle handle = null;
            CurrentSingleton.CreateTimerInternal(ref handle, time, callback, iterations, initialDelay);

            return handle;
        }

        /// <summary>
        /// Removes a timer and invalidates the handle.
        /// </summary>
        /// <param name="handle">The <see cref="ID.SharedHandle"/> of the timer.</param>
        public static void RemoveTimer(ref ID.SharedHandle handle)
        {
            if (!CurrentSingleton || handle == null || !CurrentSingleton._timers.ContainsKey(handle))
            {
                handle = null;
                return;
            }

            CurrentSingleton._timers[handle].Invalidate();
            handle = null;
        }

        public static bool WillTimerCompleteThisFrame(ID.SharedHandle handle)
        {
            if (!CurrentSingleton || handle == null)
                return false;

            CurrentSingleton._timers.TryGetValue(handle, out TimerData timer);
            return timer != null && timer.WillComplete(Time.deltaTime);
        }

        /// <summary>
        /// Internally creates a new timer.
        /// </summary>
        /// <param name="handle">The <see cref="ID.SharedHandle"/> of the timer. If not set, the handle is initialized automatically. If already set, the timer matching
        /// the handle is replaced with a new one.</param>
        /// <param name="time">The amount of time to delay before activating the timer.</param>
        /// <param name="callback">The delegate to invoke at the end of the timer.</param>
        /// <param name="iterations">The number of iterations to invoke the timer for. If zero or less, the timer is infinitely looping.</param>
        /// <param name="initialDelay">An initial delay to apply to the first iteration.</param>
        private void CreateTimerInternal(ref ID.SharedHandle handle, float time, TimerDelegate callback, int iterations, float initialDelay)
        {
            handle ??= new ID.SharedHandle(_timerGenerator.GetID());
            if (time <= 0.0f && iterations > 0)
            {
                callback?.Invoke();
                return;
            }

            TimerData data = new TimerData(callback, time, initialDelay, iterations);
            _timers[handle] = data;
        }
    }
}
