// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEngine.InputSystem;

namespace SlashParadox.Essence
{
    public delegate void InputRegistryEvent(InputRegistry registry);

    public delegate void InputCallbackEvent(InputAction.CallbackContext context);

    /// <summary>
    /// An enum representing how an input action is consumed by a <see cref="InputRegistry"/>'s callbacks.
    /// </summary>
    [Flags]
    public enum InputConsumption
    {
        None = 0,
        Self = 1,
        Others = 2,
        All = 3
    }

    /// <summary>
    /// An enum class for what grouping an input registry belongs to. This aids in sorting input registries
    /// before processing, if you have particular inputs that must always stick together, but then need an internal sort order.
    /// </summary>
    public class InputRegistryGroup : EnumType<InputRegistryGroup>
    {
        public static InputRegistryGroup Default = new InputRegistryGroup(0, nameof(Default));
        public static InputRegistryGroup Menu = new InputRegistryGroup(1000, nameof(Menu));

        public InputRegistryGroup(int value, string name) : base(value, name) { }
    }

    /// <summary>
    /// A collection of input bindings. These are used to handle routing player input to other objects, while also preventing
    /// lower priority objects from receiving any input.
    /// </summary>
    public class InputRegistry : IComparable<InputRegistry>
    {
        /// <summary>
        /// The method that an <see cref="InputRegistry"/> blocks other registries from reaching inputs.
        /// </summary>
        public enum BlockType
        {
            None,
            BlockWhileEnabled,
            BlockAlways
        }

        /// <summary>
        /// Data for some callback method for an input.
        /// </summary>
        /// <remarks>This remains an internal class so that all updates are deferred to the <see cref="InputRegistry"/>.</remarks>
        internal class InputCallback
        {
            /// <summary>The phase this callback triggers on.</summary>
            public readonly InputActionPhase Phase;

            /// <summary>A handle for tracking the callback.</summary>
            public readonly ID.SharedHandle Handle;

            /// <summary>The method that this callback consumes an input from other callbacks and registries.</summary>
            public InputConsumption Consumption { get; protected set; }

            /// <summary>The action to call upon activation.</summary>
            public Action<InputAction.CallbackContext> Callback { get; protected set; }

            internal InputCallback(InputActionPhase phase, InputConsumption consumption, Action<InputAction.CallbackContext> callback, ID.SharedHandle handle)
            {
                Phase = phase;
                Handle = handle;
                Consumption = consumption;
                Callback = callback;
            }
        }

        /// <summary>
        /// Data for callbacks to a specific action name.
        /// </summary>
        private class InputActionStateData
        {
            /// <summary>The <see cref="InputCallback"/>s registered to an <see cref="InputAction"/>.</summary>

            // TODO: Should this be a map of handles to callbacks? Would the memory difference be worth it?
            public readonly List<InputCallback> Bindings = new List<InputCallback>();
        }

        /// <summary>An event called whenever the registry needs updates processed.</summary>
        internal event InputRegistryEvent OnRegistryNeedsUpdate;

        /// <summary>A table of <see cref="InputAction"/> names to their bindings. Names are used to allow sharing between <see cref="InputActionMap"/>s.</summary>
        private readonly Dictionary<string, InputActionStateData> _stateData = new Dictionary<string, InputActionStateData>();

        /// <summary>A <see cref="ID.SharedHandle"/> generator for <see cref="InputCallback"/>s.</summary>
        private readonly ID.Generator _bindingIDGenerator = new ID.Generator();

        /// <summary>The <see cref="InputRegistryGroup"/> this registry belongs to. Determines general priority.</summary>
        private InputRegistryGroup _group;

        /// <summary>A series of updates that have been deferred to prevent issues while processing input this frame.</summary>
        private Action _pendingUpdates;

        /// <summary>A list of pending handles for <see cref="InputCallback"/>s not yet added.</summary>
        private readonly List<ID.SharedHandle> _pendingHandles = new List<ID.SharedHandle>();

        /// <summary>The ID name of the registry. Used for tracking and debugging.</summary>
        public string Name { get; private set; }

        /// <summary>The internal priority of the registry. Appended to the <see cref="_group"/> priority.</summary>
        public int Priority { get; private set; }

        /// <summary>If true, the registry is enabled and can process input.</summary>
        public bool IsEnabled { get; private set; } = true;

        /// <summary>The method that the registry blocks input from other registries.</summary>
        public BlockType BlockingType { get; private set; }

        /// <summary>
        /// Creates a new <see cref="InputRegistry"/>.
        /// </summary>
        /// <param name="id">The name of the registry.</param>
        /// <param name="blockType">The method that this registry blocks other registries.</param>
        /// <param name="priority">The priority of the registry. Higher numbers are processed first.</param>
        /// <param name="group">The group that the registry belongs to, determining priority.</param>
        /// <returns>Returns the created <see cref="InputRegistry"/>.</returns>
        public static InputRegistry Create(string id, BlockType blockType = BlockType.None, int priority = 0, InputRegistryGroup group = null)
        {
            return Create<InputRegistry>(id, blockType, priority, group);
        }

        /// <summary>
        /// Creates a new <see cref="InputRegistry"/>.
        /// </summary>
        /// <typeparam name="TRegistry">The type of <see cref="InputRegistry"/> to create.</typeparam>
        /// <param name="id">The name of the registry.</param>
        /// <param name="blockType">The method that this registry blocks other registries.</param>
        /// <param name="priority">The priority of the registry. Higher numbers are processed first.</param>
        /// <param name="group">The group that the registry belongs to, determining priority.</param>
        /// <returns>Returns the created <see cref="InputRegistry"/>.</returns>
        public static TRegistry Create<TRegistry>(string id, BlockType blockType = BlockType.None, int priority = 0, InputRegistryGroup group = null)
            where TRegistry : InputRegistry, new()
        {
            TRegistry registry = new TRegistry();
            registry.Initialize(id, blockType, priority, group);
            registry.OnInitialized();

            return registry;
        }

        public int CompareTo(InputRegistry other)
        {
            int fullPriority = (_group?.Value ?? 0) + Priority;
            int otherFullPriority = (other._group?.Value ?? 0) + other.Priority;
            return otherFullPriority.CompareTo(fullPriority);
        }

        /// <summary>
        /// Adds a new callback binding to the registry.
        /// </summary>
        /// <param name="actionName">The name of the <see cref="InputAction"/> to listen for.</param>
        /// <param name="callback">The action to call upon activation.</param>
        /// <param name="phase">The <see cref="InputActionPhase"/> the binding listens for.</param>
        /// <param name="consumption">The method that an input is consumed if processed with this binding.</param>
        /// <returns>Returns a <see cref="ID.SharedHandle"/> to the binding.</returns>
        public ID.SharedHandle RegisterCallback(string actionName, Action<InputAction.CallbackContext> callback, InputActionPhase phase, InputConsumption consumption = InputConsumption.All)
        {
            if (string.IsNullOrEmpty(actionName) || callback == null)
                return null;

            if (phase is InputActionPhase.Waiting or InputActionPhase.Disabled)
                return null;

            // Create a pending handle and a pending update to process later.
            ID.SharedHandle handle = _bindingIDGenerator.GetSharedID();
            _pendingHandles.Add(handle);
            _pendingUpdates += () => { RegisterCallbackInternal(actionName, callback, phase, consumption, handle); };
            OnRegistryNeedsUpdate?.Invoke(this);

            return handle;
        }

        /// <summary>
        /// Removes a callback binding from the registry.
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <param name="handle">The handle of the binding.</param>
        public void UnregisterCallback(string actionName, ID.SharedHandle handle)
        {
            if (handle == null || !_stateData.ContainsKey(actionName))
                return;

            // The handle was pending, so we don't need to create a pending update.
            if (_pendingHandles.RemoveSingleSwap(handle) > 0)
                return;

            _pendingUpdates += () => { UnregisterCallbackInternal(actionName, handle); };
            OnRegistryNeedsUpdate?.Invoke(this);
        }

        /// <summary>
        /// Sets whether or not the <see cref="InputRegistry"/> can process input.
        /// </summary>
        /// <param name="enabled">If true, input can be processed.</param>
        public void SetEnabled(bool enabled)
        {
            _pendingUpdates += () => { IsEnabled = enabled; };
            OnRegistryNeedsUpdate?.Invoke(this);
        }

        /// <summary>
        /// Sets the base priority of the <see cref="InputRegistry"/>.
        /// </summary>
        /// <param name="priority">The new input priority. Added to the <see cref="_group"/>'s priority.</param>
        public void SetPriority(int priority)
        {
            _pendingUpdates += () => { Priority = priority; };
            OnRegistryNeedsUpdate?.Invoke(this);
        }

        /// <summary>
        /// Sets how this <see cref="InputRegistry"/> should block lower-priority registries from processing input.
        /// </summary>
        /// <param name="type">The new blocking method.</param>
        public void SetBlockingType(BlockType type)
        {
            _pendingUpdates += () => { BlockingType = type; };
            OnRegistryNeedsUpdate?.Invoke(this);
        }

        /// <summary>
        /// Initializes the <see cref="InputRegistry"/> with basic data.
        /// </summary>
        /// <param name="id">The name of the registry.</param>
        /// <param name="blockType">The method that this registry blocks other registries.</param>
        /// <param name="priority">The priority of the registry. Higher numbers are processed first.</param>
        /// <param name="group">The group that the registry belongs to, determining priority.</param>
        private void Initialize(string id, BlockType blockType, int priority, InputRegistryGroup group)
        {
            Name = id ?? string.Empty;
            BlockingType = blockType;
            Priority = priority;
            _group = group;
        }

        /// <summary>
        /// Adds a new callback binding to the registry. Called as a pending update.
        /// </summary>
        /// <param name="actionName">The name of the <see cref="InputAction"/> to listen for.</param>
        /// <param name="callback">The action to call upon activation.</param>
        /// <param name="phase">The <see cref="InputActionPhase"/> the binding listens for.</param>
        /// <param name="consumption">The method that an input is consumed if processed with this binding.</param>
        /// <param name="handle">The <see cref="ID.SharedHandle"/> created earlier. Only processed if it is within the <see cref="_pendingHandles"/>.</param>
        /// <returns>Returns a <see cref="ID.SharedHandle"/> to the binding.</returns>
        private void RegisterCallbackInternal(string actionName, Action<InputAction.CallbackContext> callback, InputActionPhase phase, InputConsumption consumption, ID.SharedHandle handle)
        {
            // If the handle is no longer pending, it was likely removed.
            int index = _pendingHandles.IndexOf(handle);
            if (index < 0)
                return;

            _pendingHandles.RemoveAtSwap(index);

            _stateData.GetOrInitializeValue(actionName, out InputActionStateData state);
            state.Bindings.Add(new InputCallback(phase, consumption, callback, handle));
        }

        /// <summary>
        /// Removes a callback binding from the registry. Called as a pending update.
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <param name="handle">The handle of the binding.</param>
        private void UnregisterCallbackInternal(string actionName, ID.SharedHandle handle)
        {
            if (handle == null || !_stateData.TryGetValue(actionName, out InputActionStateData state))
                return;

            state.Bindings.RemoveAtSwap(state.Bindings.FindIndex(binding => binding.Handle == handle));
        }

        /// <summary>
        /// Processes any pending updates for the registry. This should only be called from the <see cref="PlayerInputReader"/>.
        /// </summary>
        internal void ProcessPendingUpdates()
        {
            _pendingUpdates?.Invoke();
            _pendingUpdates = null;
        }

        /// <summary>
        /// Finds all <see cref="InputCallback"/>s for a given <see cref="InputAction.CallbackContext"/>, and invokes any unblocked ones.
        /// This should only be called from the <see cref="PlayerInputReader"/>.
        /// </summary>
        /// <param name="inContext">The action context to process.</param>
        /// <returns>Returns if the given context was consumed, and should not be processed by lower <see cref="InputRegistry"/>s.</returns>
        internal bool InvokeCallbacksForInput(InputAction.CallbackContext inContext)
        {
            if (inContext.action == null)
                return false;

            if (!_stateData.TryGetValue(inContext.action.name, out InputActionStateData state))
                return false;

            if (state.Bindings.Count <= 0)
                return false;

            bool isBlocking = false;

            foreach (InputCallback callback in state.Bindings)
            {
                if (callback.Phase != inContext.phase)
                    continue;

                isBlocking |= callback.Consumption.HasFlag(InputConsumption.Others);

                callback.Callback?.Invoke(inContext);

                if (callback.Consumption.HasFlag(InputConsumption.Self))
                    return isBlocking;
            }

            return isBlocking;
        }

        /// <summary>
        /// An event called when the registry is created and initialized. Only called once.
        /// </summary>
        protected virtual void OnInitialized() { }
    }
}