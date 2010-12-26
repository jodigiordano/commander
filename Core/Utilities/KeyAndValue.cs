namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework.Content;


    [Serializable]
    public class KeyAndValue<K, V>
    {
        [ContentSerializer(Optional = false)]
        public K Key { get; set; }

        [ContentSerializer(Optional = false)]
        public V Value { get; set; }


        public KeyAndValue(K key, V value)
        {
            Key = key;
            Value = value;
        }


        public KeyAndValue()
        {

        }
    }
}