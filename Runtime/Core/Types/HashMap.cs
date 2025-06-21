// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A copy of <see cref="KeyValuePair"/>, for <see cref="HashMap{TKey,TValue}"/>s.
    /// This is to be able to serialize data, which the normal pair cannot do in Unity.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public struct HashMapPair<TKey, TValue>
    {
        /// <summary>See: <see cref="Key"/></summary>
        [SerializeField] private TKey key;

        /// <summary>See: <see cref="Value"/></summary>
        [SerializeField] private TValue value;

        /// <summary>The key of the entry.</summary>
        public readonly TKey Key { get { return key; } }

        /// <summary>The associated value of the entry.</summary>
        public readonly TValue Value { get { return value; } }

        public HashMapPair(TKey inKey, TValue inValue)
        {
            key = inKey;
            value = inValue;
        }

        public static implicit operator KeyValuePair<TKey, TValue>(HashMapPair<TKey, TValue> pair)
        {
            return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }

        public static implicit operator HashMapPair<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        {
            return new HashMapPair<TKey, TValue>(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// A serializable <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public class HashMap<TKey, TValue> : IDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        /// <summary>The serializable data of the <see cref="_dictionary"/>.</summary>
        [SerializeField] private List<HashMapPair<TKey, TValue>> data = new List<HashMapPair<TKey, TValue>>();

#if UNITY_EDITOR
        /// <summary>Editor-only data of the <see cref="_dictionary"/>. This allows duplicates for a better editing experience.</summary>
        [SerializeField] private List<HashMapPair<TKey, TValue>> editorData = new List<HashMapPair<TKey, TValue>>();
#endif
        /// <summary>The internal <see cref="Dictionary{TKey,TValue}"/>.</summary>
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public virtual bool IsFixedSize { get { return false; } }

        public virtual bool IsReadOnly { get { return false; } }

        public virtual int Count { get { return _dictionary.Count; } }

        public virtual bool IsSynchronized { get { return false; } }

        public virtual object SyncRoot { get { return _dictionary; } }

        ICollection IDictionary.Keys { get { return _dictionary.Keys; } }

        ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return _dictionary.Values; } }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return _dictionary.Keys; } }

        ICollection IDictionary.Values { get { return _dictionary.Values; } }

        public ICollection<TKey> Keys { get { return _dictionary.Keys; } }

        public ICollection<TValue> Values { get { return _dictionary.Values; } }

        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), $"{nameof(array)} is null!");

            if (index > array.Length)
                throw new IndexOutOfRangeException($"{nameof(index)} of {index} is out of bounds on array of length {array.Length}!");

            if (array.Length - index < _dictionary.Count)
                throw new ArgumentException("Array too small to copy to!", nameof(array));

            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                array.SetValue(pair, index);
            }
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!_dictionary.TryGetValue(item.Key, out TValue value))
                return false;

            if (EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                _dictionary.Remove(item.Key);
                return true;
            }

            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            _dictionary.Clear();
        }

        public virtual void Add(object key, object value)
        {
            if (key is not TKey tKey || value is not TValue tValue)
                throw new ArgumentException($"Incorrect data given to dictionary! Key: [{key}, {key.GetTypeSafe()}, needs {typeof(TKey)}], Value: [{value}, {value.GetTypeSafe()}, needs {typeof(TValue)}]");

            Add(new KeyValuePair<TKey, TValue>(tKey, tValue));
        }

        public virtual bool Contains(object key)
        {
            return key is TKey tKey && _dictionary.ContainsKey(tKey);
        }

        public virtual void Remove(object key)
        {
            if (key is TKey tKey)
                _dictionary.Remove(tKey);
        }

        void IDictionary.Clear()
        {
            _dictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public virtual void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public virtual bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public virtual bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public virtual void OnBeforeSerialize()
        {
            data.Clear();
            data.Capacity = _dictionary.Count;

            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                data.Add(pair);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            _dictionary.Clear();
            _dictionary.EnsureCapacity(data.Count);

            List<HashMapPair<TKey, TValue>> targetList = data;

#if UNITY_EDITOR
            targetList = editorData;
#endif

            foreach (HashMapPair<TKey, TValue> pair in targetList)
            {
                if (!_dictionary.ContainsKey(pair.Key))
                    _dictionary.Add(pair.Key, pair.Value);
            }

            data.Clear();
        }

        public virtual object this[object key]
        {
            get
            {
                if (key is TKey tKey)
                {
                    if (_dictionary.TryGetValue(tKey, out TValue value))
                        return value;
                }

                return null;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key), "Key is null!");

                if (default(TKey) != null && value == null)
                    throw new ArgumentNullException(nameof(value), "Value is null, when the type is not nullable!");

                if (key is not TKey tKey)
                    throw new ArgumentException(nameof(key), $"{nameof(key)} is not of type {typeof(TKey)}!");

                if (value is not TValue tValue)
                    throw new ArgumentException(nameof(value), $"{nameof(value)} is not of type {typeof(TValue)}!");

                _dictionary[tKey] = tValue;
            }
        }

        public virtual TValue this[TKey key] { get { return _dictionary[key]; } set { _dictionary[key] = value; } }
    }
}
