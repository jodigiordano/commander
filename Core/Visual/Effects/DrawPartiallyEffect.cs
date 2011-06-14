namespace EphemereGames.Core.Visual
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class DrawPartiallyEffect : Effect<IVisual>
    {
        public bool DrawPartially       { get; set; }
        public Rectangle VisiblePart    { get; set; }

        public static Pool<DrawPartiallyEffect> Pool = new Pool<DrawPartiallyEffect>();


        protected override void LogicLinear()
        {
            throw new Exception("Not defined.");
        }


        protected override void LogicAfter()
        {
            Obj.VisiblePart = VisiblePart;
        }


        protected override void LogicNow()
        {
            Obj.VisiblePart = VisiblePart;
        }


        internal override void Return()
        {
            Pool.Return((DrawPartiallyEffect) this);
        }
    }
}
