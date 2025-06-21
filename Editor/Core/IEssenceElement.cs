using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector
{
    public interface IEssenceElement
    {
        public abstract void SetProperty(SerializedProperty property);
    }
}
