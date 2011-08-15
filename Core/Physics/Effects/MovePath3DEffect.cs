namespace EphemereGames.Core.Physics
{
    using System;
    using EphemereGames.Core.Utilities;


    public class MovePath3DEffect : Effect<IPhysical>
    {
        public Path3D InnerPath     { private get; set; }
        public bool PointAt         { private get; set; }
        public double StartAt;
        public float RotationRad    { private get; set; }

        public static Pool<MovePath3DEffect> Pool = new Pool<MovePath3DEffect>();


        protected override void InitializeLogic()
        {
            Obj.Position = InnerPath.GetPosition(StartAt);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(StartAt);
        }


        protected override void LogicLinear()
        {
            if (ElaspedTime + StartAt >= Length)
                return;

            Obj.Position = InnerPath.GetPosition(ElaspedTime + StartAt);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(Math.Min(ElaspedTime + StartAt, Length - Length * 0.02));
        }


        protected override void LogicAfter()
        {
            LogicLinear();
        }


        protected override void LogicNow()
        {
            Obj.Position = InnerPath.GetPosition(Length);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(Length - Length * 0.01);
        }


        protected override void LogicEnd()
        {

        }


        internal override void Return()
        {
            Pool.Return((MovePath3DEffect) this);
        }
    }
}
