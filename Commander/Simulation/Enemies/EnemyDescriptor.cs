namespace EphemereGames.Commander.Simulation
{
    using System.Xml.Serialization;
    using EphemereGames.Core.Utilities;


    [XmlRoot(ElementName = "Enemy")]
    public class EnemyDescriptor
    {
        [XmlIgnore]
        public static Pool<EnemyDescriptor> Pool = new Pool<EnemyDescriptor>();

        public EnemyType Type;
        public int SpeedLevel;
        public int LivesLevel;
        public int CashValue;
        public double StartingTime;

        [XmlIgnore]
        public double StartingPosition;
    }
}
