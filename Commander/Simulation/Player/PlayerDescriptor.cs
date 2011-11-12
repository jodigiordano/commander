namespace EphemereGames.Commander.Simulation
{
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    [XmlRoot(ElementName = "Player")]
    public class PlayerDescriptor
    {
        public int Money;
        public int Lives;
        public Vector3 StartingPosition;
        public double BulletDamage;


        public PlayerDescriptor()
        {
            Money = 0;
            Lives = 0;
            StartingPosition = Vector3.Zero;
            BulletDamage = -1;
        }
    }
}
