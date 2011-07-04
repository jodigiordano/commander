namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    abstract class PowerUp
    {
        public PowerUpType Type             { get; protected set; }
        public PowerUpCategory Category     { get; protected set; }
        public TurretType AssociatedTurret  { get; protected set; }
        public bool PayOnActivation         { get; protected set; }
        public bool PayOnUse                { get; protected set; }
        public string BuyImage              { get; protected set; }
        public int BuyPrice                 { get; protected set; }
        public int UsePrice                 { get; protected set; }
        public Vector3 BuyPosition          { get; set; }
        public string BuyTitle              { get; protected set; }
        public string BuyDescription        { get; protected set; }
        public bool NeedInput               { get; protected set; }
        public string Crosshair             { get; protected set; }
        public float CrosshairSize          { get; protected set; }
        public Vector3 Position             { get; protected set; }
        public abstract bool Terminated     { get; }
        public bool TerminatedOverride      { protected get; set; }
        public SimPlayer Owner              { get; set; }

        protected Simulator Simulation;


        public PowerUp(Simulator simulator)
        {
            Simulation = simulator;
            Type = PowerUpType.None;
            PayOnActivation = true;
            PayOnUse = false;
            BuyImage = "";
            BuyPrice = 0;
            UsePrice = 0;
            BuyPosition = Vector3.Zero;
            BuyTitle = "";
            BuyDescription = "";
            NeedInput = false;
            Crosshair = "";
            CrosshairSize = 1;
            Position = Vector3.Zero;
            Category = PowerUpCategory.Other;
            TerminatedOverride = false;
            Owner = null;
            AssociatedTurret = TurretType.None;
        }


        public virtual void Update() { }


        public virtual void DoInputMoved(Vector3 position)
        {
            Position = position;
        }


        public virtual void DoInputReleased() { }


        public virtual void DoInputCanceled() { }


        public virtual void DoInputPressed() { }


        public abstract void Start();


        public virtual void Stop() { }


        public virtual void DoInputMovedDelta(Vector3 delta) { }
    }
}
