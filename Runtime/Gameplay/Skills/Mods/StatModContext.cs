// Copyright (c) Craig Williams, SlashParadox

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// Context for the creation of a <see cref="StatMod"/>. This should hold any information a mod needs to operate.
    /// </summary>
    public class StatModContext
    {
        /// <summary>A custom <see cref="StatModSpec"/> to use. If set, certain systems may prefer it over dynamic specs.</summary>
        protected StatModSpec CustomSpec;

        /// <summary>A cached active <see cref="StatModSpec"/>, obtained from <see cref="GetStatModSpec{T}"/>.</summary>
        [CopyImmutable] private StatModSpec _cachedModSpec;

        /// <summary>The object that created and owns the mod. This must be set in a constructor.</summary>
        [CopyImmutable] public UnityEngine.Object Owner { get; }

        /// <summary>The <see cref="StatsSystem"/> that originated the <see cref="StatMod"/>.</summary>
        [CopyImmutable] public StatsSystem SourceSystem { get; private set; }

        /// <summary>The <see cref="StatsSystem"/> that the <see cref="StatMod"/> will apply to.</summary>
        [CopyImmutable] public StatsSystem TargetSystem { get; private set; }

        public StatModContext(UnityEngine.Object inOwner)
        {
            Owner = inOwner;
        }

        /// <summary>
        /// Sets the <see cref="SourceSystem"/>.
        /// </summary>
        /// <param name="inSystem">The source of the <see cref="StatMod"/>.</param>
        /// <returns>Returns this <see cref="StatModContext"/>.</returns>
        public StatModContext SetSourceSystem(StatsSystem inSystem)
        {
            SourceSystem = inSystem;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="TargetSystem"/>.
        /// </summary>
        /// <param name="inSystem">The target of the <see cref="StatMod"/>.</param>
        /// <returns>Returns this <see cref="StatModContext"/>.</returns>
        public StatModContext SetTargetSystem(StatsSystem inSystem)
        {
            TargetSystem = inSystem;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="CustomSpec"/>. Certain systems may prefer this over a dynamic spec.
        /// </summary>
        /// <param name="inSpec">The <see cref="StatModSpec"/> to use.</param>
        /// <returns>Returns this <see cref="StatModContext"/>.</returns>
        public StatModContext SetCustomModSpec(StatModSpec inSpec)
        {
            CustomSpec = inSpec;
            return this;
        }

        /// <summary>
        /// Gets the <see cref="StatModSpec"/> to use, and caches it.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="StatModSpec"/>.</typeparam>
        /// <returns>Returns the appropriate <see cref="StatModSpec"/>.</returns>
        public T GetStatModSpec<T>() where T : StatModSpec
        {
            if (_cachedModSpec != null)
                return (T)_cachedModSpec;

            T spec = (T)GetUsedModSpec();
            _cachedModSpec = spec;

            return spec;
        }

        /// <summary>
        /// Checks if the <see cref="StatModContext"/> is valid.
        /// </summary>
        /// <returns>Returns if the <see cref="StatModContext"/> can be used.</returns>
        public virtual bool IsValid()
        {
            return Owner && SourceSystem && TargetSystem;
        }

        /// <summary>
        /// Gets the appropriate <see cref="StatModSpec"/> to use.
        /// </summary>
        /// <returns>Returns the context's <see cref="StatModSpec"/>.</returns>
        protected virtual StatModSpec GetUsedModSpec()
        {
            return CustomSpec;
        }
    }
}
