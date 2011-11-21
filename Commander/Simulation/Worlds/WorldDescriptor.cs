namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;


    [XmlRoot(ElementName = "World")]
    public class WorldDescriptor
    {
        public int Id;
        public string Author;
        public string Name;

        [XmlArrayItem("Level")]
        public List<int> Levels;

        [XmlArrayItem("World")]
        public List<int> Warps;

        public string Music;
        public string MusicEnd;
        public string SfxEnd;
        public string LastModification;

        private XmlSerializer Serializer;

        private int NextLevelId;


        public WorldDescriptor()
        {
            Id = -1;
            Author = "";
            Name = "";
            Levels = new List<int>();
            Warps = new List<int>();
            Music = "";
            MusicEnd = "";
            SfxEnd = "";
            LastModification = Main.GetCurrentTimestamp();

            Serializer = new XmlSerializer(this.GetType());

            NextLevelId = 0;
        }


        public void Initialize()
        {
            var highest = 0;

            foreach (var l in Levels)
                if (l > highest)
                    highest = l;

            NextLevelId = highest;
        }


        public bool ContainsLevel(int id)
        {
            foreach (var l in Levels)
                if (l == id)
                    return true;

            return false;
        }


        public int GetNextLevelId()
        {
            return ++NextLevelId;
        }


        public int GetNextNoWarpId()
        {
            var lowest = 0;

            foreach (var w in Warps)
                if (w < lowest)
                    lowest = w;

            return lowest - 1;
        }


        public string ToXML()
        {
            using (StringWriter writer = new StringWriter())
            {
                Serializer.Serialize(writer, this);

                return writer.ToString();
            }
        }
    }
}
