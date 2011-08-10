namespace EphemereGames.Core.Physics
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class ImpulseEffect : Effect<IPhysical>
    {
        public Vector3 Direction;
        public float Speed;

        public static Pool<ImpulseEffect> Pool = new Pool<ImpulseEffect>();


        protected override void InitializeLogic()
        {

        }


        protected override void LogicLinear()
        {
            Obj.Position += Direction * Speed;
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


        internal override void Return()
        {
            Pool.Return((ImpulseEffect) this);
        }
    }
}
