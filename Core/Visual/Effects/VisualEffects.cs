namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public static class VisualEffects
    {
        private static Random Random = new Random();


        public static SizeEffect SizeNow(float size)
        {
            SizeEffect se = SizeEffect.Pool.Get();

            se.Progress = Effect<IVisual>.ProgressType.Now;
            se.Length = 500;
            se.Path =
                new Path2D(new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(size, size)
                }, new List<double>
                {
                    0,
                    500
                });

            return se;
        }


        public static SizeEffect OriginalSizeNow()
        {
            SizeEffect se = SizeEffect.Pool.Get();

            se.Progress = Effect<IVisual>.ProgressType.Now;
            se.Length = 500;
            se.Path =
                new Path2D(new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 1)
                }, new List<double>
                {
                    0,
                    500
                });

            return se;
        }


        public static FadeColorEffect FadeInFrom0(int to, double delay, double length)
        {
            return Fade(0, to, delay, length);
        }


        public static FadeColorEffect FadeOutTo0(int from, double delay, double length)
        {
            return Fade(from, 0, delay, length);
        }


        public static FadeColorEffect Fade(int from, int to, double delay, double length)
        {
            FadeColorEffect e = FadeColorEffect.Pool.Get();

            e.Path =
                new Path2D(new List<Vector2>
                {
                    new Vector2(from / 255.0f, from / 255.0f),
                    new Vector2((from > to ? from : to) / 255.0f / 2.0f, (from > to ? from : to) / 255.0f / 2.0f),
                    new Vector2(to / 255.0f, to / 255.0f)
                }, new List<double>
                {
                    0,
                    length / 2.0f,
                    length
                });

            e.Progress = Effect<IVisual>.ProgressType.Linear;
            e.Delay = delay;
            e.Length = length;

            return e;
        }


        public static SizeEffect ChangeSize(float from, float to, double delay, double length)
        {
            SizeEffect se = SizeEffect.Pool.Get();

            se.Path =
                new Path2D(new List<Vector2>
                {
                    new Vector2(from, from),
                    new Vector2((from + to) / 2, (from + to) / 2),
                    new Vector2(to, to)
                }, new List<double>
                {
                    0,
                    length / 2,
                    length
                });

            se.Progress = Effect<IVisual>.ProgressType.Linear;
            se.Delay = delay;
            se.Length = length;

            return se;
        }


        public static ColorEffect ChangeColor(Color to, double delay, double length)
        {
            ColorEffect ce = ColorEffect.Pool.Get();

            ce.FinalColor = to;
            ce.Progress = Effect<IVisual>.ProgressType.Linear;
            ce.Delay = delay;
            ce.Length = length;

            return ce;
        }
    }
}
