namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public static class PhysicalEffects
    {
        private static Random random = new Random();


        public static MovePathEffect FollowPath(Path2D path, double delay)
        {
            var e = MovePathEffect.Pool.Get();

            e.Delay = delay;
            e.InnerPath = path;
            e.Length = path.Length;
            e.PointAt = true;
            e.Progress = Effect<IPhysical>.ProgressType.Linear;

            return e;
        }


        public static MoveEffect Move(Vector3 positionEnd, double delay, double length)
        {
            var e = MoveEffect.Pool.Get();

            e.Delay = delay;
            e.Length = length;
            e.PositionEnd = positionEnd;
            e.Progress = Effect<IPhysical>.ProgressType.Linear;

            return e;
        }


        public static MoveEffect Arrival(Vector3 positionEnd, double delay, double length)
        {
            var e = MoveEffect.Pool.Get();

            e.Delay = delay;
            e.Length = length;
            e.PositionEnd = positionEnd;
            e.Progress = Effect<IPhysical>.ProgressType.Logarithmic;

            return e;
        }


        public static MoveEffect MoveNow(Vector3 position)
        {
            var e = MoveEffect.Pool.Get();

            e.Delay = 0;
            e.PositionEnd = position;
            e.Progress = Effect<IPhysical>.ProgressType.Now;
            e.Length = 500;

            return e;
        }


        public static MovePathEffect MoveTextFromUpToBottom(Vector2 startingPosition, Vector2 endPosition, double timeStart, double length)
        {
            var eDt = MovePathEffect.Pool.Get();

            var temps = new List<double> {
                0,
                (random.Next(10, 40) / 100.0) * length,
                length
            };

            var positions = new List<Vector2> {
                startingPosition,
                new Vector2(endPosition.X + 20, (startingPosition.Y + endPosition.Y) / 2f),
                endPosition
            };

            var t = new Path2D(positions, temps);

            eDt.PointAt = false;
            eDt.StartAt = 0;
            eDt.InnerPath = t;
            eDt.Progress = Effect<IPhysical>.ProgressType.Linear;
            eDt.Delay = timeStart;
            eDt.Length = length;

            return eDt;
        }


        public static ImpulseEffect RandomImpulse(float impulse, double time)
        {
            Vector3 direction = new Vector3(
                random.Next(-100, 100),
                random.Next(-100, 100),
                0
            );

            direction.Normalize();

            ImpulseEffect e = ImpulseEffect.Pool.Get();

            e.Delay = 0;
            e.Direction = direction;
            e.Length = time;
            e.Speed = (float) (time / impulse);
            e.Progress = Effect<IPhysical>.ProgressType.Linear;

            return e;
        }


        public static List<KeyValuePair<MovePathEffect, Text>> FollowText(Text text, Path2D path, int centerDistPx, float rotationRad, bool rightToLeft)
        {
            //Separate each letter in it's own Text object
            List<Text> letters = new List<Text>();

            for (int i = 0; i < text.Data.Length; i++)
            {
                var copy = text.Clone();

                copy.Data = text.Data[i].ToString();
                copy.Origin = copy.Center;

                letters.Add(copy);
            }

            //A letter size in time
            var letterSize = centerDistPx;


            //Create the effects
            List<KeyValuePair<MovePathEffect, Text>> result = new List<KeyValuePair<MovePathEffect, Text>>();

            for (int i = 0; i < letters.Count; i++)
            {
                MovePathEffect mpe = MovePathEffect.Pool.Get();

                if (rightToLeft)
                    mpe.StartAt = (letters.Count - i) * letterSize;
                else
                    mpe.StartAt = i * letterSize;

                if (rightToLeft)
                    mpe.Length = path.Length - i * letterSize;
                else
                    mpe.Length = path.Length - ((letters.Count - i) * letterSize);


                mpe.PointAt = true;
                mpe.InnerPath = path;
                mpe.RotationRad = rotationRad;
                mpe.Delay = 0;
                mpe.Progress = Effect<IPhysical>.ProgressType.Linear;

                result.Add(new KeyValuePair<MovePathEffect, Text>(mpe, letters[i]));
            }


            return result;
        }
    }
}
