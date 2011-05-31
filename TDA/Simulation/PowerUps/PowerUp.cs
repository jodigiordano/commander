namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    abstract class PowerUp
    {
        public PowerUpType Type             { get; protected set; }
        public PowerUpCategory Category     { get; protected set; }
        public string BuyImage              { get; protected set; }
        public int BuyPrice                 { get; protected set; }
        public Vector3 BuyPosition          { get; set; }
        public string BuyTitle              { get; protected set; }
        public string BuyDescription        { get; protected set; }
        public bool NeedInput               { get; protected set; }
        public Vector3 Position             { get; protected set; }
        public abstract bool Terminated     { get; }

        protected Simulation Simulation;


        public PowerUp(Simulation simulation)
        {
            Simulation = simulation;
            Type = PowerUpType.None;
            BuyImage = "";
            BuyPrice = 0;
            BuyPosition = Vector3.Zero;
            BuyTitle = "";
            BuyDescription = "";
            NeedInput = false;
            Position = Vector3.Zero;
            Category = PowerUpCategory.Other;
        }


        public virtual void Update() { }

        public virtual void DoInputMoved(Vector3 position) { }

        public virtual void DoInputReleased() { }

        public virtual void DoInputCanceled() { }

        public virtual void DoInputPressed() { }

        public abstract void Start();

        public virtual void DoInputMovedDelta(Vector3 delta) { }
    }
}
