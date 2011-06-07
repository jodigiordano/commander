namespace EphemereGames.Core.Physique
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public static class PredefinedEffects
    {
        private static Random random = new Random();


        public static MoveEffect Move(Vector3 positionEnd, double delay, double length)
        {
            var e = new MoveEffect();
            e.Delay = delay;
            e.Length = length;
            e.PositionEnd = positionEnd;
            e.Progress = AbstractEffect.ProgressType.Linear;

            return e;
        }


        public static MoveEffect Arrival(Vector3 positionEnd, double delay, double length)
        {
            var e = new MoveEffect();
            e.Delay = delay;
            e.Length = length;
            e.PositionEnd = positionEnd;
            e.Progress = AbstractEffect.ProgressType.Logarithmic;

            return e;
        }


        public static MoveEffect MoveNow(Vector3 position)
        {
            var e = new MoveEffect();

            e.PositionEnd = position;
            e.Progress = AbstractEffect.ProgressType.Now;
            e.Length = 500;

            return e;
        }


        public static EffetDeplacementTrajet MoveTextFromUpToBottom(Vector2 startingPosition, Vector2 endPosition, double timeStart, double length)
        {
            var eDt = new EffetDeplacementTrajet();

            double[] temps = new double[] {
                0,
                (random.Next(10, 40) / 100.0) * length,
                length
            };

            Vector2[] positions = new Vector2[] {
                startingPosition,
                new Vector2(endPosition.X + 20, (startingPosition.Y + endPosition.Y) / 2f),
                endPosition
            };

            var t = new Trajet2D(positions, temps);

            eDt.Trajet = t;
            eDt.Progress = AbstractEffect.ProgressType.Linear;
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

            ImpulseEffect e = new ImpulseEffect()
            {
                Delay = 0,
                Direction = direction,
                Length = time,
                Speed = (float) (time / impulse),
                Progress = AbstractEffect.ProgressType.Linear
            };

            return e;
        }
    }
}
