// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A Unity-safe wrapper around <see cref="System.Type"/>s.
    /// </summary>
    [System.Serializable]
    public class MemberType
    {
        /// <summary>The <see cref="string"/>, assembly-qualified name of the <see cref="Type"/>.</summary>
        [SerializeField] private string assemblyQualifiedName = string.Empty;

        /// <summary>See: <see cref="Type"/></summary>
        private System.Type _type;

        /// <summary>The stored <see cref="System.Type"/> referenced.</summary>
        public System.Type Type { get { return GetReferencedType(); } set { SetReferencedType(value); } }

        /// <summary>
        /// A base constructor for a <see cref="MemberType"/>.
        /// </summary>
        public MemberType()
        {
            _type = null;
            assemblyQualifiedName = string.Empty;
        }

        /// <summary>
        /// A constructor for a <see cref="MemberType"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to reference.</param>
        public MemberType(System.Type type)
        {
            Type = type;
        }

        /// <summary>
        /// A constructor for a <see cref="MemberType"/>.
        /// </summary>
        /// <param name="inAssemblyQualifiedName">The assembly qualified name of the <see cref="System.Type"/>.</param>
        public MemberType(string inAssemblyQualifiedName)
        {
            Type = AssemblyQualifiedStringToType(inAssemblyQualifiedName);
        }

        /// <summary>
        /// Converts a <see cref="System.Type"/> to its assembly qualified <see cref="string"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to convert.</param>
        /// <returns>Returns the assembly qualified <see cref="string"/>.</returns>
        public static string TypeToAssemblyQualifiedString(System.Type type)
        {
            return type != null ? type.AssemblyQualifiedName : string.Empty;
        }

        /// <summary>
        /// Converts an assembly qualified <see cref="string"/> to its associated <see cref="System.Type"/>.
        /// </summary>
        /// <param name="inAssemblyQualifiedName">The assembly qualified <see cref="string"/>.</param>
        /// <returns>Returns the found <see cref="System.Type"/>.</returns>
        public static System.Type AssemblyQualifiedStringToType(string inAssemblyQualifiedName)
        {
            return string.IsNullOrEmpty(inAssemblyQualifiedName) ? null : System.Type.GetType(inAssemblyQualifiedName);
        }

        public override string ToString()
        {
            return assemblyQualifiedName;
        }

        /// <summary>
        /// Checks if a given type can be set.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to check.</param>
        /// <param name="error">An error message to print if false.</param>
        /// <returns>Returns if the type can be set.</returns>
        public virtual bool CanTypeBeSet(System.Type type, out string error)
        {
            error = null;
            return true;
        }

        /// <summary>
        /// Gets the <see cref="_type"/>.
        /// </summary>
        /// <returns>Returns the <see cref="_type"/>.</returns>
        private System.Type GetReferencedType()
        {
            if (_type != null)
                return _type;

            _type = AssemblyQualifiedStringToType(assemblyQualifiedName);
            return _type;
        }

        /// <summary>
        /// Sets the <see cref="_type"/>.
        /// </summary>
        private void SetReferencedType(System.Type type)
        {
            if (type != null && !CanTypeBeSet(type, out string error))
            {
                System.Diagnostics.Debug.Print($"Unable to set type {type} to this MemberType because [{error}]");
                _type = null;
            }
            
            _type = type;
            assemblyQualifiedName = _type != null ? _type.AssemblyQualifiedName : string.Empty;
        }

        public static implicit operator System.Type(MemberType memberType)
        {
            return memberType.Type;
        }

        public static implicit operator MemberType(System.Type type)
        {
            return new MemberType(type);
        }
        
        public static bool operator ==(MemberType a, MemberType b)
        {
            bool aIsNull = ReferenceEquals(a, null);
            if (aIsNull && ReferenceEquals(b, null))
                return true;
            
            return !aIsNull && a.Equals(b);
        }
        
        public static bool operator !=(MemberType a, MemberType b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MemberType);
        }

        protected bool Equals(MemberType other)
        {
            bool otherIsNull = ReferenceEquals(other, null);
            if (ReferenceEquals(this, null) && !otherIsNull)
                return true;
            
            return !otherIsNull && Type == other.Type;
        }

        public override int GetHashCode()
        {
            return Type?.GetHashCode() ?? 0;
        }
    }

    [System.Serializable]
    public class MemberType<T> : MemberType
    {
        private bool _canBeBaseType;

        public MemberType()
        {
            _canBeBaseType = true;
        }

        public MemberType(bool canBeBaseType = true)
        {
            _canBeBaseType = canBeBaseType;
        }

        public MemberType(System.Type type, bool canBeBaseType = true)
        : base(type)
        {
            _canBeBaseType = canBeBaseType;
        }
        
        public MemberType(string inAssemblyQualifiedString, bool canBeBaseType = true)
            : base(inAssemblyQualifiedString)
        {
            _canBeBaseType = canBeBaseType;
        }
        
        public override bool CanTypeBeSet(System.Type type, out string error)
        {
            System.Type baseType = typeof(T);

            // If the types match, check if we allow matching the base type.
            if (type == baseType)
            {
                error = _canBeBaseType ? null : $"{type} matches the base type {baseType}, which is not allowed!";
                return string.IsNullOrEmpty(error);
            }
            
            // If the base type is an interface, check if the given type implements it.
            if (baseType.IsInterface)
            {
                error = type.ImplementsInterface(typeof(T)) ? null : $"{type} does not inherit {baseType}!";
                return string.IsNullOrEmpty(error);
            }
            
            // If the base type is a class, check if the given type inherits it.
            if (type.IsClass)
            {
                error = type.IsSubclassOf(baseType) ? null : $"{type} does not inherit {baseType}!";
                return string.IsNullOrEmpty(error);
            }

            // At this point, there is no way to match.
            error = $"{type} does not match the {baseType}!";
            return false;
        }
    }

    /// <summary>
    /// The different ways to sort options of a <see cref="MemberType"/>.
    /// </summary>
    public enum MemberTypeGroup
    {
        /// <summary>There is no sorting. Everything is listed as is.</summary>
        None,
        /// <summary>Items are sorted by the class they inherit.</summary>
        ByInheritance,
        /// <summary>Items are sorted by the namespace they're apart of.</summary>
        ByNamespace,
        /// <summary>Items are sorted by type identity (class, struct, interface, enum)</summary>
        ByIdentity,
    }

    /// <summary>
    /// An attribute for <see cref="MemberType"/> and how to sort them.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MemberTypeGroupAttribute : System.Attribute
    {
#if UNITY_EDITOR
        /// <summary>The sort style to use.</summary>
        public MemberTypeGroup Grouping { get; private set; }
#endif

        /// <summary>
        /// The constructor for a <see cref="MemberTypeGroupAttribute"/>.
        /// </summary>
        /// <param name="grouping">The sort style to use.</param>
        public MemberTypeGroupAttribute(MemberTypeGroup grouping)
        {
            Grouping = grouping;
        }
    }

    /// <summary>
    /// A base class for filters for what can be selected for a <see cref="MemberType"/> in the editor.
    /// </summary>
    public abstract class MemberTypeFilterAttribute : System.Attribute
    {
        /// <summary>If true, negates the return of <see cref="DoesTypeSatisfyFilter"/>.</summary>
        private readonly bool _negate;

        /// <summary>
        /// The base constructor for a <see cref="MemberTypeFilterAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        protected MemberTypeFilterAttribute(bool negate)
        {
            _negate = negate;
        }

        /// <summary>
        /// Checks if the given <see cref="System.Type"/> satisfies this filter.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to check.</param>
        /// <returns>Returns if the <paramref name="type"/> is valid.</returns>
        public bool SatisfiesFilter(System.Type type)
        {
            if (type == null)
                return false;

            bool success = DoesTypeSatisfyFilter(type);
            return _negate ? !success : success;
        }

        /// <summary>
        /// Checks if the given <see cref="System.Type"/> satisfies this filter.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to check.</param>
        /// <returns>Returns if the <paramref name="type"/> is valid.</returns>
        protected abstract bool DoesTypeSatisfyFilter(System.Type type);
    }

    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows classes.
    /// </summary>
    public class MemberTypeClassAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeClassAttribute"/>.
        /// </summary>
        public MemberTypeClassAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeClassAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeClassAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsClass;
        }
    }
    
    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows value types.
    /// </summary>
    public class MemberTypeValueTypeAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeValueTypeAttribute"/>.
        /// </summary>
        public MemberTypeValueTypeAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeValueTypeAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeValueTypeAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsValueType;
        }
    }
    
    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows value types.
    /// </summary>
    public class MemberTypeStructAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeStructAttribute"/>.
        /// </summary>
        public MemberTypeStructAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeStructAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeStructAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsValueType && !type.IsEnum;
        }
    }
    
    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows enum types.
    /// </summary>
    public class MemberTypeEnumAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeEnumAttribute"/>.
        /// </summary>
        public MemberTypeEnumAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeEnumAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeEnumAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsEnum;
        }
    }
    
    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows interface types.
    /// </summary>
    public class MemberTypeInterfaceAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeInterfaceAttribute"/>.
        /// </summary>
        public MemberTypeInterfaceAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeInterfaceAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeInterfaceAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsInterface;
        }
    }
    
    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows abstract types.
    /// </summary>
    public class MemberTypeAbstractAttribute : MemberTypeFilterAttribute
    {
        /// <summary>
        /// The constructor for a <see cref="MemberTypeAbstractAttribute"/>.
        /// </summary>
        public MemberTypeAbstractAttribute()
            : this(false) { }
        
        /// <summary>
        /// The constructor for a <see cref="MemberTypeAbstractAttribute"/>.
        /// </summary>
        /// <param name="negate">Negates the filter. If a <see cref="System.Type"/> matches, it is excluded.</param>
        public MemberTypeAbstractAttribute(bool negate)
            : base(negate) { }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            return type != null && type.IsAbstract;
        }
    }

    /// <summary>
    /// A <see cref="MemberTypeFilterAttribute"/> that only allows types that inherit a certain type.
    /// </summary>
    public class MemberTypeInheritsAttribute : MemberTypeFilterAttribute
    {
        /// <summary>The base <see cref="System.Type"/> to filter with.</summary>
        private readonly System.Type _baseType;

        /// <summary>If true, the type can be the <see cref="_baseType"/>.</summary>
        private readonly bool _allowBaseType;

        public MemberTypeInheritsAttribute(System.Type baseType, bool allowBaseType = true, bool negate = false)
            : base(negate)
        {
            _baseType = baseType;
            _allowBaseType = allowBaseType;
        }

        protected override bool DoesTypeSatisfyFilter(System.Type type)
        {
            if (type == null || _baseType == null)
                return false;

            if (!_allowBaseType && type == _baseType)
                return false;
            
            return type.IsSubclassOrImplements(_baseType);
        }
    }
}
#endif