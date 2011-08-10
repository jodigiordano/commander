namespace EphemereGames.Core.Physics
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class MoveOffsetEffect : Effect<IPhysical>
    {
        public Vector3 Offset { get; set; }

        private Vector3 LastDisplacement;

        public static Pool<MoveOffsetEffect> Pool = new Pool<MoveOffsetEffect>();


        protected override void InitializeLogic()
        {
            LastDisplacement = Vector3.Zero;
        }


        protected override void LogicLinear()
        {

            Vector3 NewDisplacement = Offset * (float)(ElaspedTime / Length);

            Obj.Position -= LastDisplacement;
            Obj.Position += NewDisplacement;

            LastDisplacement = NewDisplacement;
        }


        protected override void LogicAfter()
        {
            Obj.Position += Offset;
        }


        protected override void LogicNow()
        {
            Obj.Position += Offset;
        }


        protected override void LogicEnd()
        {
            base.LogicEnd();

            Obj.Position -= LastDisplacement;
            Obj.Position += Offset;
            LastDisplacement = Offset;
        }


        internal override void Return()
        {
            Pool.Return((MoveOffsetEffect) this);
        }
    }
}
