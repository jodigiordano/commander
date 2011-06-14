namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;


    public class DrawPartiallyEffect : VisualEffect
    {
        public bool DrawPartially { get; set; }
        public Rectangle VisiblePart { get; set; }


        protected override void LogicLinear()
        {
            throw new Exception("Pas logique!");
        }


        protected override void LogicAfter()
        {
            Object.DessinerPartie = DrawPartially;
            Object.partieVisible = VisiblePart;
        }


        protected override void LogicNow()
        {
            Object.DessinerPartie = DrawPartially;
            Object.partieVisible = VisiblePart;
        }


        object Clone()
        {
            DrawPartiallyEffect dpe = (DrawPartiallyEffect)base.Clone();
            dpe.DrawPartially = this.DrawPartially;
            dpe.VisiblePart = this.VisiblePart;

            return dpe;
        }
    }
}
