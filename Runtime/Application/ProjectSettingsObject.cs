// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An <see cref="EnumType{T}"/> for <see cref="ProjectSettingsObject{T}"/>s.
    /// </summary>
    public sealed class ProjectSettingsType : EnumType<ProjectSettingsType>
    {
        /// <summary>The project settings can be loaded at runtime.</summary>
        public static ProjectSettingsType Runtime = new ProjectSettingsType(0, nameof(Runtime), "Project/", "Resources/");
        
#if UNITY_EDITOR
        /// <summary>The project settings can only be loaded at editor time.</summary>
        public static ProjectSettingsType Editor = new ProjectSettingsType(1, nameof(Editor), "Project/User/", "Editor");
        
        /// <summary>The project settings can only be loaded at editor time, and should not be source controlled.</summary>
        public static ProjectSettingsType User = new ProjectSettingsType(2, nameof(User), "Preferences", "Editor/User");
#endif
        
        /// <summary>A display path portion to add when displaying the settings.</summary>
        public string DisplayPath { get; private set; }
        
        /// <summary>The appended file path portion to the asset.</summary>
        public string FilePath { get; private set; }

        private ProjectSettingsType(int index, string name, string displayPath, string filePath)
            : base(index, name)
        {
            DisplayPath = displayPath;
            FilePath = filePath;
        }
    }

    /// <summary>
    /// A required <see cref="ScriptableSingletonAttribute"/> for <see cref="ProjectSettingsObject{T}"/>s.
    /// </summary>
    public class ProjectSettingsAttribute : ScriptableSingletonAttribute
    {
        /// <summary>The absolute start of the path to the settings asset.</summary>
        public static readonly string BasePath = "Assets/Settings/";

        /// <summary>The <see cref="ProjectSettingsType"/>, containing information on the type.</summary>
        public readonly ProjectSettingsType SettingsType;

        /// <summary>If true, <see cref="ScriptableSingletonAttribute.AssetPath"/> is the whole path.</summary>
        public readonly bool OverrideWholePath;

        /// <summary>A path for displaying on the actual settings menu.</summary>
        public readonly string DisplayPath;

        /// <summary>
        /// A constructor for a <see cref="ProjectSettingsAttribute"/>.
        /// </summary>
        /// <param name="settingsTypeName">The name of the <see cref="ProjectSettingsType"/> to use. Required.</param>
        /// <param name="displayPath">A path for displaying on the actual settings menu.</param>
        /// <param name="baseAssetPath">The base path. Does not have to contain directories, as those can be made automatically.</param>
        /// <param name="overrideWholePath">If true, <see cref="ScriptableSingletonAttribute.AssetPath"/> is the whole path.</param>
        public ProjectSettingsAttribute(string settingsTypeName, string displayPath, string baseAssetPath, bool overrideWholePath = false)
            : base(false, baseAssetPath)
        {
            // We must ensure that the class is ready to go!
            RuntimeHelpers.RunClassConstructor(typeof(ProjectSettingsType).TypeHandle);
            
            DisplayPath = displayPath;
            SettingsType = ProjectSettingsType.GetEnumByName(settingsTypeName);
            OverrideWholePath = overrideWholePath;
        }

        public override string GetAssetPath()
        {
            if (OverrideWholePath || SettingsType == null)
                return AssetPath;

            return $"{BasePath}{SettingsType.FilePath}{AssetPath}";
        }
    }

    /// <summary>
    /// A base class for custom project settings.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="ProjectSettingsObject{T}"/>.</typeparam>
    /// <remarks>These classes must be set up in a particular way to appear.
    /// First, apply a <see cref="ProjectSettingsAttribute"/> to your custom class. Ensure it contains a reference to the name of a
    /// <see cref="ProjectSettingsType"/>, a display path, and an asset name and path.
    /// Then, create a static method which returns a <see cref="UnityEditor.SettingsProvider"/>. You can easily acquire one by calling
    /// "<see cref="ProjectSettingsProvider.GetProvider{T}"/>, passing in <see cref="SingletonScriptableObject{T}.GetOrCreateInstance"/>.
    /// It must also contain the <see cref="UnityEditor.SettingsProvider"/> <see cref="System.Attribute"/>.
    /// See <see cref="EssenceProjectSettings"/> for an example.</remarks>
    public abstract class ProjectSettingsObject<T> : SingletonScriptableObject<T> where T : ProjectSettingsObject<T>
    {
        /// <summary>The <see cref="ProjectSettingsAttribute"/> attached to this type.</summary>
        public ProjectSettingsAttribute SettingsAttribute { get { return GetType().GetCustomAttribute<ProjectSettingsAttribute>(); } }
    }

    /// <summary>
    /// A base class for custom project settings, which has a publicly accessible instance.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="PublicProjectSettingsObject{T}"/>.</typeparam>
    /// <remarks>These classes must be set up in a particular way to appear.
    /// First, apply a <see cref="ProjectSettingsAttribute"/> to your custom class. Ensure it contains a reference to the name of a
    /// <see cref="ProjectSettingsType"/>, a display path, and an asset name and path.
    /// Then, create a static method which returns a <see cref="UnityEditor.SettingsProvider"/>. You can easily acquire one by calling
    /// "<see cref="ProjectSettingsProvider.GetProvider{T}"/>, passing in <see cref="SingletonScriptableObject{T}.GetOrCreateInstance"/>.
    /// It must also contain the <see cref="UnityEditor.SettingsProvider"/> <see cref="System.Attribute"/>.
    /// See <see cref="EssenceProjectSettings"/> for an example.</remarks>
    public abstract class PublicProjectSettingsObject<T> : ProjectSettingsObject<T> where T : PublicProjectSettingsObject<T>
    {
        /// <summary>
        /// Gets the current instance of the <see cref="PublicProjectSettingsObject{T}"/>.
        /// </summary>
        /// <typeparam name="TSingleton">The type of the <see cref="PublicProjectSettingsObject{T}"/>.</typeparam>
        /// <returns>Returns the <see cref="ProjectSettingsObject{T}._currentSingleton"/>.</returns>
        public static TSingleton Get<TSingleton>() where TSingleton : T
        {
            return (TSingleton)GetOrCreateInstance();
        }
    }
}
#endif