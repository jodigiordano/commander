namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework.Content;


    public class SerializableDictionaryProxy<K, V>
    {
        private Dictionary<K, V> Original;


        public SerializableDictionaryProxy()
        {
            Original = new Dictionary<K, V>();
            KeysAndValues = new List<KeyAndValue<K, V>>();
        }


        public void Initialize()
        {
            foreach (var kvp in KeysAndValues)
                Original.Add(kvp.Key, kvp.Value);
        }


        public void InitializeToSave()
        {
            KeysAndValues.Clear();

            foreach (var kvp in Original)
                KeysAndValues.Add(new KeyAndValue<K, V>(kvp.Key, kvp.Value));
        }


        [XmlArray("Entries")]
        [XmlArrayItem("Entry")]
        [ContentSerializer(Optional = false)]
        public List<KeyAndValue<K, V>> KeysAndValues { get; set; }

        //public List<KeyAndValue<K, V>> KeysAndValues
        //{
        //    get
        //    {
        //        var list = new List<KeyAndValue<K, V>>();

        //        foreach (var kvp in Original)
        //            list.Add(new KeyAndValue<K, V>(kvp.Key, kvp.Value));

        //        return list;
        //    }


        //    set
        //    {
        //        foreach (var kvp in value)
        //            Original.Add(kvp.Key, kvp.Value);
        //    }
        //}


        #region IDictionary<K,V> Membres

        public void Add(K key, V value)
        {
            Original.Add(key, value);
        }

        public bool ContainsKey(K key)
        {
            return Original.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get { return Original.Keys; }
        }

        public bool Remove(K key)
        {
            return Original.Remove(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return Original.TryGetValue(key, out value);
        }

        public ICollection<V> Values
        {
            get { return Original.Values; }
        }

        public V this[K key]
        {
            get { return Original[key]; }
            set { Original[key] = value; }
        }

        #endregion

        #region ICollection<KeyValuePair<K,V>> Membres

        public void Add(KeyValuePair<K, V> item)
        {
            Original.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Original.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            V value;

            return (Original.TryGetValue(item.Key, out value) &&
                    value.Equals(item.Value));
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { return Original.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            V value;

            return (Original.TryGetValue(item.Key, out value) &&
                    value.Equals(item.Value) &&
                    Original.Remove(item.Key));
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Membres

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return Original.GetEnumerator();
        }

        #endregion
    }
}
