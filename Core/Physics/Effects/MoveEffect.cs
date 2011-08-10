namespace EphemereGames.Core.Physics
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class MoveEffect : Effect<IPhysical>
    {
        public Vector3 PositionEnd      { get; set; }
        private Vector3 PositionStart   { get; set; }

        private Vector3 Displacement = Vector3.Zero;

        public static Pool<MoveEffect> Pool = new Pool<MoveEffect>();


        protected override void InitializeLogic()
        {
            PositionStart = Obj.Position;

            Displacement = PositionEnd - PositionStart;
        }


        protected override void LogicLinear()
        {
            Obj.Position =
                new Vector3(
                    (float)(PositionStart.X + (Displacement.X * (ElaspedTime / Length))),
                    (float)(PositionStart.Y + (Displacement.Y * (ElaspedTime / Length))),
                    (float)(PositionStart.Z + (Displacement.Z * (ElaspedTime / Length))));
        }


        protected override void LogicLogarithmic()
        {
            Obj.Position =
                new Vector3(
                    (float) (PositionStart.X + (Displacement.X * (Math.Log(ElaspedTime) / Math.Log(Length)))),
                    (float) (PositionStart.Y + (Displacement.Y * (Math.Log(ElaspedTime) / Math.Log(Length)))),
                    (float) (PositionStart.Z + (Displacement.Z * (Math.Log(ElaspedTime) / Math.Log(Length)))));
        }


        protected override void LogicAfter()
        {
            Obj.Position = PositionEnd;
        }


        protected override void LogicNow()
        {
            Obj.Position = PositionEnd;
        }


        internal override void Return()
        {
            Pool.Return((MoveEffect) this);
        }
    }
}
