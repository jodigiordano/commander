namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;
    

    public class RecenterEffect : VisualEffect
    {
        public Vector2 OriginStart { get; set; }
        private Vector2 ObjectOrigin;
        private Vector2 Movement;


        protected override void InitializeLogic()
        {
            ObjectOrigin = Object.Origine;
            Movement = OriginStart - ObjectOrigin;
        }


        protected override void LogicLinear()
        {
            Vector2 newOrigin = new Vector2();

            newOrigin.X = (float)(ObjectOrigin.X + (Movement.X * (ElaspedTime / Length)));
            newOrigin.Y = (float)(ObjectOrigin.Y + (Movement.Y * (ElaspedTime / Length)));

            Object.Origine = newOrigin;
        }


        protected override void LogicAfter()
        {
            Object.Origine = OriginStart;
        }


        protected override void LogicNow()
        {
            Object.Origine = OriginStart;
        }
    }
}
