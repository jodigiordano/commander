namespace EphemereGames.Core.Physics
{
    using EphemereGames.Core.Utilities;


    public class SpeedPathEffect : Effect<IPhysicalObject>
    {
        public Path2D InnerPath;
        public float StartingSpeed;
        public float EndingSpeed;
        private float DeltaSpeed;

        public static Pool<SpeedPathEffect> Pool = new Pool<SpeedPathEffect>();


        protected override void LogicLinear()
        {
            Obj.Speed = StartingSpeed + DeltaSpeed * InnerPath.GetPosition(ElaspedTime).Y;
        }


        protected override void LogicAfter()
        {
            LogicLinear();
        }


        protected override void LogicNow()
        {
            Obj.Speed = EndingSpeed;
        }


        protected override void InitializeLogic()
        {
            DeltaSpeed = EndingSpeed - StartingSpeed;

            Obj.Speed = StartingSpeed;
        }


        protected override void LogicEnd()
        {
            Obj.Speed = EndingSpeed;
        }


        internal override void Return()
        {
            Pool.Return((SpeedPathEffect) this);
        }
    }
}
