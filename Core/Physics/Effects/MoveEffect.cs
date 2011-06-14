namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;
    using System;


    public class MoveEffect : PhysicalEffect
    {
        public Vector3 PositionEnd { get; set; }
        private Vector3 PositionStart { get; set; }

        private Vector3 Displacement = Vector3.Zero;


        protected override void InitializeLogic()
        {
            PositionStart = Objet.Position;

            Displacement = PositionEnd - PositionStart;
        }


        protected override void LogicLinear()
        {
            Objet.Position =
                new Vector3(
                    (float)(PositionStart.X + (Displacement.X * (ElaspedTime / Length))),
                    (float)(PositionStart.Y + (Displacement.Y * (ElaspedTime / Length))),
                    (float)(PositionStart.Z + (Displacement.Z * (ElaspedTime / Length))));
        }


        protected override void LogicLogarithmic()
        {
            Objet.Position =
                new Vector3(
                    (float) (PositionStart.X + (Displacement.X * (Math.Log(ElaspedTime) / Math.Log(Length)))),
                    (float) (PositionStart.Y + (Displacement.Y * (Math.Log(ElaspedTime) / Math.Log(Length)))),
                    (float) (PositionStart.Z + (Displacement.Z * (Math.Log(ElaspedTime) / Math.Log(Length)))));
        }


        protected override void LogicAfter()
        {
            Objet.Position = PositionEnd;
        }


        protected override void LogicNow()
        {
            Objet.Position = PositionEnd;
        }
    }
}
