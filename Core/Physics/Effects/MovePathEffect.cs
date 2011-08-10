namespace EphemereGames.Core.Physics
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class MovePathEffect : Effect<IPhysical>
    {
        public Path2D InnerPath     { private get; set; }
        public bool PointAt         { private get; set; }
        public double StartAt;
        public float RotationRad    { private get; set; }

        public static Pool<MovePathEffect> Pool = new Pool<MovePathEffect>();


        protected override void InitializeLogic()
        {
            //Length -= StartAt;
            //RemainingBeforeEnd -= StartAt;

            Obj.Position = new Vector3(InnerPath.GetPosition(StartAt), Obj.Position.Z);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(StartAt);
        }


        protected override void LogicLinear()
        {
            if (ElaspedTime + StartAt >= Length)
                return;

            Obj.Position = new Vector3(InnerPath.GetPosition(ElaspedTime + StartAt), Obj.Position.Z);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(Math.Min(ElaspedTime + StartAt, Length - Length * 0.02));
        }


        protected override void LogicAfter()
        {
            LogicLinear();
        }


        protected override void LogicNow()
        {
            Obj.Position = new Vector3(InnerPath.GetPosition(Length), Obj.Position.Z);

            if (PointAt)
                Obj.Rotation = RotationRad + InnerPath.GetRotation(Length - Length * 0.01);
        }


        protected override void LogicEnd()
        {
            //Obj.Position = new Vector3(InnerPath.GetPosition(Length), Obj.Position.Z);

            //if (PointAt)
            //    Obj.Rotation = InnerPath.GetRotation(Length - Length * 0.01);
        }


        internal override void Return()
        {
            Pool.Return((MovePathEffect) this);
        }
    }
}
