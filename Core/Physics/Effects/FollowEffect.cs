namespace EphemereGames.Core.Physics
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class FollowEffect : Effect<IPhysicalObject>
    {
        public IObjetPhysique ObjetSuivi    { get; set; }
        public float Vitesse                { get; set; }

        public static Pool<FollowEffect> Pool = new Pool<FollowEffect>();


        protected override void InitializeLogic()
        {

        }


        protected override void LogicLinear()
        {
            Vector3 direction = ObjetSuivi.Position - Obj.Position;
            direction.Normalize();

            Obj.Position += direction * Vitesse;
        }


        protected override void LogicAfter()
        {
            throw new Exception("TODO");
        }


        protected override void LogicNow()
        {
            throw new Exception("TODO");
        }


        internal override void Return()
        {
            Pool.Return((FollowEffect) this);
        }
    }
}
