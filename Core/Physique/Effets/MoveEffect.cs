namespace EphemereGames.Core.Physique
{
    using Microsoft.Xna.Framework;


    public class MoveEffect : EffetPhysique
    {
        public Vector3 PositionStart { get; set; }
        private Vector3 PositionEnd { get; set; }

        private Vector3 Displacement = Vector3.Zero;


        protected override void InitializeLogic()
        {
            PositionEnd = Objet.Position;

            Displacement = PositionStart - PositionEnd;
        }


        protected override void LogicLinear()
        {
            Objet.Position =
                new Vector3(
                    (float)(PositionEnd.X + (Displacement.X * (ElaspedTime / Length))),
                    (float)(PositionEnd.Y + (Displacement.Y * (ElaspedTime / Length))),
                    (float)(PositionEnd.Z + (Displacement.Z * (ElaspedTime / Length))));
        }


        protected override void LogicAfter()
        {
            Objet.Position = PositionStart;
        }


        protected override void LogicNow()
        {
            Objet.Position = PositionStart;
        }
    }
}
