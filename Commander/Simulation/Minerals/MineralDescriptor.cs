namespace EphemereGames.Commander.Simulation
{
    using System.Xml.Serialization;


    [XmlRoot(ElementName = "Minerals")]
    public class MineralsDescriptor
    {
        public int Cash;
        public int LifePacks;


        public MineralsDescriptor()
        {
            Cash = 0;
            LifePacks = 0;
        }
    }
}
