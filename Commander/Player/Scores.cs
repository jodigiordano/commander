namespace EphemereGames.Commander
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using EphemereGames.Core.Utilities;


    public class HighScores
    {
        [ContentSerializer(Optional = false)]
        public int Level { get; set; }

        [ContentSerializer(Optional = false)]
        [XmlArrayItem("Scores")]
        public List<KeyAndValue<string, int>> Scores { get; set; }

        private IComparer<KeyAndValue<string, int>> Comparer;


        public HighScores()
        {
            Level = -1;
            Scores = new List<KeyAndValue<string, int>>();
            Comparer = new ComparerKeyValuePairs();
        }


        public HighScores(int id)
        {
            Level = id;
            Scores = new List<KeyAndValue<string, int>>();
            Comparer = new ComparerKeyValuePairs();
        }


        public void Add(string playerName, int score)
        {
            Scores.Add(new KeyAndValue<string, int>(playerName, score));
            Scores.Sort(Comparer);

            if (Scores.Count > 10)
                Scores.RemoveAt(9);
        }


        private class ComparerKeyValuePairs : IComparer<KeyAndValue<string, int>>
        {
            public int Compare(KeyAndValue<string, int> x, KeyAndValue<string, int> y)
            {
                if (x.Value < y.Value)
                    return 1;

                if (x.Value > y.Value)
                    return -1;

                return 0;
            }
        }
    }
}
