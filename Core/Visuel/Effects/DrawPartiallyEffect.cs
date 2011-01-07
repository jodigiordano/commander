﻿namespace EphemereGames.Core.Visuel
{
    using System;
    using Microsoft.Xna.Framework;


    public class DrawPartiallyEffect : VisualEffect, ICloneable
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


        object ICloneable.Clone()
        {
            DrawPartiallyEffect dpe = (DrawPartiallyEffect)base.Clone();
            dpe.DrawPartially = this.DrawPartially;
            dpe.VisiblePart = this.VisiblePart;

            return dpe;
        }
    }
}