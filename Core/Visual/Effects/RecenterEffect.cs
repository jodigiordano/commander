namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    

    public class RecenterEffect : Effect<IVisual>
    {
        public Vector2 OriginStart { get; set; }
        private Vector2 ObjectOrigin;
        private Vector2 Movement;

        public static Pool<RecenterEffect> Pool = new Pool<RecenterEffect>();


        protected override void InitializeLogic()
        {
            ObjectOrigin = Obj.Origin;
            Movement = OriginStart - ObjectOrigin;
        }


        protected override void LogicLinear()
        {
            Vector2 newOrigin = new Vector2();

            newOrigin.X = (float)(ObjectOrigin.X + (Movement.X * (ElaspedTime / Length)));
            newOrigin.Y = (float)(ObjectOrigin.Y + (Movement.Y * (ElaspedTime / Length)));

            Obj.Origin = newOrigin;
        }


        protected override void LogicAfter()
        {
            Obj.Origin = OriginStart;
        }


        protected override void LogicNow()
        {
            Obj.Origin = OriginStart;
        }


        internal override void Return()
        {
            Pool.Return((RecenterEffect) this);
        }
    }
}
