// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
namespace SlashParadox.Essence
{
    /// <summary>
    /// Data that can be attached to a <see cref="GameManager"/>. Only set by the <see cref="GameManager"/> and the
    /// <see cref="EssenceProjectSettings"/>.
    /// </summary>
    public abstract class GameManagerData : EssenceScriptableObject { }
}
#endif