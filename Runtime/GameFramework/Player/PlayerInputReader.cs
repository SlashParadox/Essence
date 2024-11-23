// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A reader for a <see cref="PlayerInput"/>. Handles routing input and blocking components.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputReader : EssenceBehaviour
    {
        /// <summary>The attached player input component.</summary>
        [SerializeField] [AutoFind] [ReadOnly] private PlayerInput playerInput;

        /// <summary>The current stack of <see cref="InputRegistry"/> objects, which handle routing input.</summary>
        private readonly List<InputRegistry> _inputStack = new List<InputRegistry>();

        /// <summary>If true, the stack is processing an input, and shouldn't do any updates.</summary>
        private bool _isProcessingStack;

        /// <summary>If true, the <see cref="_inputStack"/> has pending updates to process.</summary>
        private bool _isInputStackDirty;

        /// <summary>A series of updates that have been deferred to prevent issues while processing input this frame.</summary>
        /// <remarks>These should process before player input does. Ensure this in the script order.</remarks>
        private Action _pendingUpdates;

        /// <summary>The attached player input component.</summary>
        public PlayerInput Input { get { return playerInput; } }

        protected virtual void Awake()
        {
            FindComponentIfNull(playerInput);
        }

        private void LateUpdate()
        {
            // Safety check.
            if (_isProcessingStack)
                return;

            ProcessPendingUpdates();
            ProcessInputStackPendingUpdates();
        }

        protected override void Start()
        {
            base.Start();

            InputRegistry registry = InputRegistry.Create("Test Registry", InputRegistry.BlockType.BlockWhileEnabled, 0, InputRegistryGroup.Menu);
            registry.RegisterCallback("Move", Test, InputActionPhase.Performed, InputConsumption.None);
            registry.RegisterCallback("Move", Test, InputActionPhase.Performed, InputConsumption.None);

            InputRegistry registryb = InputRegistry.Create("Test Registry 2", InputRegistry.BlockType.BlockAlways, 1);
            registryb.RegisterCallback("Move", Test2, InputActionPhase.Performed);

            AddInputRegistry(registry);
            AddInputRegistry(registryb);
        }

        protected override void OnDestroy()
        {
            ShutdownPlayerInput();

            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            InitializePlayerInput();
        }

        protected override void OnDisable()
        {
            ShutdownPlayerInput();

            base.OnDisable();
        }

        /// <summary>
        /// Adds a new <see cref="InputRegistry"/> to the stack.
        /// </summary>
        /// <param name="registry">The new <see cref="InputRegistry"/>.</param>
        public void AddInputRegistry(InputRegistry registry)
        {
            if (registry == null)
                return;

            _pendingUpdates += () => { AddInputRegistryInternal(registry); };
        }

        /// <summary>
        /// Removes the given <see cref="InputRegistry"/> from the stack.
        /// </summary>
        /// <param name="registry">The <see cref="InputRegistry"/> to remove.</param>
        /// <remarks>If you want to temporarily disable the registry, try setting its enable state.</remarks>
        public void RemoveInputRegistry(InputRegistry registry)
        {
            if (registry == null)
                return;

            _pendingUpdates += () => { RemoveInputRegistryInternal(registry); };
        }

        private void Test(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValueAsObject());
        }

        private void Test2(InputAction.CallbackContext context)
        {
            Debug.Log("Don't Print!");
        }

        /// <summary>
        /// Initializes the reader with the <see cref="playerInput"/>.
        /// </summary>
        private void InitializePlayerInput()
        {
            if (!LogKit.LogIfFalse(playerInput, $"[{gameObject}] No player input found!"))
                return;

            playerInput.onActionTriggered += OnActionTriggered;

            OnPlayerInputInitialized();
        }

        /// <summary>
        /// Shuts down and cleans up the reader.
        /// </summary>
        private void ShutdownPlayerInput()
        {
            if (!playerInput)
                return;

            playerInput.onActionTriggered -= OnActionTriggered;

            OnPlayerInputShutdown();
        }

        /// <summary>
        /// A global event when an <see cref="InputAction"/> triggers within the <see cref="playerInput"/>.
        /// It is then immediately processed.
        /// </summary>
        /// <param name="context">The context of the action.</param>
        /// <remarks>We can't store all contexts within a frame, as the context is a reference that will change values by the end of the frame.</remarks>
        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            ProcessInputStack(context);
            Debug.Log($"{context.action.name}, {context.phase}");
        }

        /// <summary>
        /// Processes an input by sending it through the entire <see cref="_inputStack"/>.
        /// </summary>
        /// <param name="context">The processing input.</param>
        private void ProcessInputStack(InputAction.CallbackContext context)
        {
            _isProcessingStack = true;

            foreach (InputRegistry registry in _inputStack)
            {
                // Ensure that null registries are removed on the next update process.
                if (registry == null)
                {
                    _isInputStackDirty = true;
                    continue;
                }

                // If the registry can't take input, check if it should still block other registries.
                if (!registry.IsEnabled)
                {
                    if (registry.BlockingType.HasFlag(InputRegistry.BlockType.BlockAlways))
                        break;

                    continue;
                }

                bool blockFurtherInputs = registry.InvokeCallbacksForInput(context);

                // Even if the registry has no callbacks, check if it blocks lower registries.
                if (blockFurtherInputs || registry.BlockingType != InputRegistry.BlockType.None)
                    break;
            }

            _isProcessingStack = false;
        }

        private void OnInputRegistryNeedsUpdate(InputRegistry registry)
        {
            _isInputStackDirty = true;
        }

        private void ProcessPendingUpdates()
        {
            _pendingUpdates?.Invoke();
            _pendingUpdates = null;
        }

        private void ProcessInputStackPendingUpdates()
        {
            if (!_isInputStackDirty)
                return;

            _inputStack.RemoveAll(registry => registry == null);

            foreach (InputRegistry registry in _inputStack)
            {
                registry.ProcessPendingUpdates();
            }

            _inputStack.Sort((a, b) => a.CompareTo(b));

            _isInputStackDirty = false;
        }

        /// <summary>
        /// Adds a new <see cref="InputRegistry"/> to the stack. Called as a pending update.
        /// </summary>
        /// <param name="registry">The new <see cref="InputRegistry"/>.</param>
        private void AddInputRegistryInternal(InputRegistry registry)
        {
            if (registry == null)
                return;

            if (_inputStack.Contains(registry))
                return;

            _inputStack.Add(registry);

            registry.OnRegistryNeedsUpdate += OnInputRegistryNeedsUpdate;

            _isInputStackDirty = true;
        }
        
        /// <summary>
        /// Removes the given <see cref="InputRegistry"/> from the stack. Called as a pending update.
        /// </summary>
        /// <param name="registry">The <see cref="InputRegistry"/> to remove.</param>
        public void RemoveInputRegistryInternal(InputRegistry registry)
        {
            if (registry == null)
                return;

            registry.OnRegistryNeedsUpdate -= OnInputRegistryNeedsUpdate;
            
            if (_inputStack.RemoveSingleSwap(registry) <= 0)
                return;

            _isInputStackDirty = true;
        }

        /// <summary>
        /// An event called when initializing the <see cref="playerInput"/>.
        /// </summary>
        protected virtual void OnPlayerInputInitialized() { }

        /// <summary>
        /// An event called when shutting down the <see cref="playerInput"/>.
        /// </summary>
        protected virtual void OnPlayerInputShutdown() { }
    }
}