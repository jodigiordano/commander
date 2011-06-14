namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;
    using System;


    public class ImpulseEffect : PhysicalEffect
    {
        public Vector3 Direction;
        public float Speed;


        protected override void InitializeLogic()
        {

        }


        protected override void LogicLinear()
        {
            Objet.Position += Direction * Speed;
        }


        protected override void LogicLogarithmic()
        {
            //todo
        }


        protected override void LogicAfter()
        {
            //todo
        }


        protected override void LogicNow()
        {
            //todo
        }
    }
}
