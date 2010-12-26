namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public static class PredefinedEffects
    {
        private static Random Random = new Random();


        public static SizeEffect SizeNow(float size)
        {
            SizeEffect se = new SizeEffect();
            se.Progress = AbstractEffect.ProgressType.Now;
            se.Length = 500;
            se.Path =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(size, size)
                }, new double[]
                {
                    0,
                    500
                });

            return se;
        }


        public static SizeEffect OriginalSizeNow()
        {
            SizeEffect se = new SizeEffect();
            se.Progress = AbstractEffect.ProgressType.Now;
            se.Length = 500;
            se.Path =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 1)
                }, new double[]
                {
                    0,
                    500
                });

            return se;
        }


        public static List<AbstractEffect> DrawPartiallyNow(Rectangle visiblePart, Vector2 newOrigin)
        {
            List<AbstractEffect> effets = new List<AbstractEffect>();

            DrawPartiallyEffect dpe = new DrawPartiallyEffect();
            dpe.DrawPartially = true;
            dpe.VisiblePart = visiblePart;
            dpe.Progress = AbstractEffect.ProgressType.Now;
            dpe.Length = 500;
            effets.Add(dpe);

            RecenterEffect re = new RecenterEffect();
            re.OriginStart = newOrigin;
            re.Length = 500;
            effets.Add(re);

            return effets;
        }


        public static FadeColorEffect FadeInFrom0(int finalAlpha, double delay, double length)
        {
            FadeColorEffect e = new FadeColorEffect();

            e.Path =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(finalAlpha / 255.0f / 2.0f, finalAlpha / 255.0f / 2.0f),
                    new Vector2(finalAlpha / 255.0f, finalAlpha / 255.0f)
                }, new double[]
                {
                    0,
                    length / 2.0f,
                    length
                });

            e.Progress = AbstractEffect.ProgressType.Linear;
            e.Delay = delay;
            e.Length = length;

            return e;
        }


        public static FadeColorEffect FadeOutTo0(int alphaDepart, double delai, double duree)
        {
            FadeColorEffect e = new FadeColorEffect();

            e.Path =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(alphaDepart / 255.0f, alphaDepart / 255.0f),
                    new Vector2(alphaDepart / 255.0f / 2.0f, alphaDepart / 255.0f / 2.0f),
                    new Vector2(0, 0)
                }, new double[]
                {
                    0,
                    duree / 2.0f,
                    duree
                });

            e.Delay = delai;
            e.Length = duree;

            return e;
        }
    }
}
