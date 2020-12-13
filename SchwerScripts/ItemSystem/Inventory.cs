﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Schwer.ItemSystem {
    public class Inventory : IDictionary<Item, int> {
        /// <summary>
        /// Invoked when the number of an item is changed through the `Inventory[]` property,
        /// `Remove()` is successful, or when `Add()` or `Clear()` is called. Passes in the `Item` and the its new amount.
        /// </summary>
        /// <remarks>
        /// Subscribers to `OnContentsChanged` should unsubscribe during their life cycle to avoid memory leaks.
        /// </remarks>
        public event Action<Item, int> OnContentsChanged;

        // Reference for custom `Dictionary`-like behaviour:
        // https://stackoverflow.com/questions/6250706/override-dictionary-add
        private IDictionary<Item, int> backingDictionary;

        #region IDictionary properties
        /// <summary>
        /// Gets or sets the count of the specified `Item`.
        /// <para/> Set removes the `Item` if its value is set to 0 or below.
        /// <para/> Get returns 0 if the `Item` is not found.
        /// </summary>
        public int this[Item key] {
            get {
                // https://stackoverflow.com/questions/14150508/how-to-get-null-instead-of-the-keynotfoundexception-accessing-dictionary-value-b
                return backingDictionary.TryGetValue(key, out int result) ? result : 0;
                // return backingDictionary[key];
            }
            set {
                var prevValue = this[key];

                backingDictionary[key] = value;
                if (backingDictionary[key] <= 0) {
                    backingDictionary.Remove(key);
                }

                if (this[key] != prevValue) {
                    OnContentsChanged?.Invoke(key, this[key]);
                }
            }
        }

        #region IDictionary properties (default behaviour)
        public ICollection<Item> Keys => backingDictionary.Keys;
        public ICollection<int> Values => backingDictionary.Values;
        public int Count => backingDictionary.Count;
        public bool IsReadOnly => backingDictionary.IsReadOnly;
        #endregion
        #endregion

        public Inventory() => backingDictionary = new Dictionary<Item, int>();
        public Inventory(Inventory inventory) => backingDictionary = new Dictionary<Item, int>(inventory);

        /// <summary>
        /// Returns this `Inventory` as a `SerializableInventory` for saving.
        /// <para>
        /// Depends on `Item.id`s to be deserialized, so `Item.id`s should not be changed after a build has been released.
        /// </para>
        /// </summary>
        public SerializableInventory Serialize() {
            var result = new SerializableInventory();
            foreach (var entry in this) {
                if (entry.Value > 0) {
                    result[entry.Key.id] = entry.Value;
                }
            }
            return result;
        }

        #region IDictionary methods
        public void Add(Item key, int value) {
            backingDictionary.Add(key, value);
            OnContentsChanged?.Invoke(key, value);
        }

        public void Add(KeyValuePair<Item, int> item) {
            backingDictionary.Add(item);
            OnContentsChanged?.Invoke(item.Key, item.Value);
        }

        public bool Remove(Item key) {
            var removeSuccessful = backingDictionary.Remove(key);
            if (removeSuccessful) {
                OnContentsChanged?.Invoke(key, this[key]);
            }
            return removeSuccessful;
        }

        public bool Remove(KeyValuePair<Item, int> item) {
            var removeSuccessful = backingDictionary.Remove(item.Key);
            if (removeSuccessful) {
                OnContentsChanged?.Invoke(item.Key, item.Value);
            }
            return removeSuccessful;
        }

        public void Clear() {
            backingDictionary.Clear();
            OnContentsChanged?.Invoke(null, 0);
        }

        #region IDictionary methods (default behaviour)
        public bool ContainsKey(Item key) => backingDictionary.ContainsKey(key);
        public bool TryGetValue(Item key, out int value) => backingDictionary.TryGetValue(key, out value);
        public bool Contains(KeyValuePair<Item, int> item) => backingDictionary.Contains(item);
        public void CopyTo(KeyValuePair<Item, int>[] array, int arrayIndex) => backingDictionary.CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<Item, int>> GetEnumerator() => backingDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => backingDictionary.GetEnumerator();
        #endregion
        #endregion
    }

    [Serializable]
    public class SerializableInventory : Dictionary<int, int> {
        public SerializableInventory() {}
        private SerializableInventory(SerializationInfo information, StreamingContext context) : base(information,context) {}
        // ^ Need to add special constructor for deserialization (since `Dictionary` implements `ISerializable`) to avoid
        // SerializationException : The constructor to deserialize an object of type [...] was not found
        // (Life-saver!) https://www.limilabs.com/blog/dictionary-serializationexception

        /// <summary>
        /// Returns this `SerializableInventory` as an `Inventory` using an `ItemDatabase` for deserialization.
        /// <para>
        /// Deserialization depends on `Item.id`s, so `Item.id`s should not be changed after a build has been released.
        /// </para>
        /// </summary>
        public Inventory Deserialize(ItemDatabase itemDatabase) {
            var result = new Inventory();
            foreach (var entry in this) {
                var itemSO = itemDatabase.GetItem(entry.Key);
                if (itemSO != null) {
                    result[itemSO] = entry.Value;
                }
            }
            return result;
        }
    }
}
