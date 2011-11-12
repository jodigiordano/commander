namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
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
            var highest = 0;

            foreach (var l in Levels)
                if (l > highest)
                    highest = l;

            return highest + 1;
        }
    }
}
