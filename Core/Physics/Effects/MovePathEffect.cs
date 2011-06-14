namespace EphemereGames.Core.Physics
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class MovePathEffect : Effect<IPhysicalObject>
    {
        public Path2D InnerPath     { private get; set; }
        public float Rotation       { get; private set; }

        public static Pool<MovePathEffect> Pool = new Pool<MovePathEffect>();


        protected override void LogicLinear()
        {
            Obj.Position = new Vector3(InnerPath.position(ElaspedTime), Obj.Position.Z);
            Rotation = InnerPath.rotation(ElaspedTime);
        }


        protected override void LogicAfter()
        {
            LogicLinear();
        }


        protected override void LogicNow()
        {
            Obj.Position = new Vector3(InnerPath.position(Length), Obj.Position.Z);
            Rotation = InnerPath.rotation(Length);
        }


        protected override void InitializeLogic()
        {
            Obj.Position = new Vector3(InnerPath.positionDepart(), Obj.Position.Z);
            Rotation = InnerPath.rotation(0);
        }


        internal override void Return()
        {
            Pool.Return((MovePathEffect) this);
        }
    }
}
