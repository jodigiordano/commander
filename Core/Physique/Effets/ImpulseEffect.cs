namespace EphemereGames.Core.Physique
{
    using Microsoft.Xna.Framework;
    using System;


    public class ImpulseEffect : EffetPhysique
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
