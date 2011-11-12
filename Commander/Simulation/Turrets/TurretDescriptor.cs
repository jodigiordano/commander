namespace EphemereGames.Commander.Simulation
{
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    [XmlRoot(ElementName = "Turret")]
    public class TurretDescriptor
    {
        public TurretType Type;
        public int Level;
        public Vector3 Position;
        public bool Visible;
        public bool CanSell;
        public bool CanUpgrade;
        public bool CanSelect;


        public TurretDescriptor()
        {
            Type = TurretType.Basic;
            Level = 1;
            Position = Vector3.Zero;
            Visible = true;
            CanSell = true;
            CanUpgrade = true;
            CanSelect = true;
        }
    }
}
