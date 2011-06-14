namespace EphemereGames.Core.Physics
{
    using EphemereGames.Core.Utilities;


    public class RotationEffect : Effect<IPhysicalObject>
    {
        public float Quantity;

        private float QuantityApplied;
        private float QuantityPerTick;

        public static Pool<RotationEffect> Pool = new Pool<RotationEffect>();


        protected override void InitializeLogic()
        {
            QuantityPerTick = (float)(Quantity / Length);
        }


        protected override void LogicLinear()
        {
            Obj.Rotation += (float) (TimeOneTick * QuantityPerTick);
            QuantityApplied -= (float)(TimeOneTick * QuantityPerTick);
        }


        protected override void LogicAfter()
        {
            Obj.Rotation += (Quantity - QuantityApplied);
        }


        protected override void LogicNow()
        {
            Obj.Rotation += Quantity;
        }


        internal override void Return()
        {
            Pool.Return((RotationEffect) this);
        }
    }
}
