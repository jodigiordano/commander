namespace EphemereGames.Core.Utilities
{
    using Microsoft.Xna.Framework;


    public class ColorInterpolator
    {
        private Color From;
        private Color To;

        private float DiffR, DiffG, DiffB;


        public ColorInterpolator(Color from, Color to)
        {
            From = from;
            To = to;

            DiffR = To.R - From.R;
            DiffG = To.G - From.G;
            DiffB = To.B - From.B;
        }


        public void GetPerc(float perc, ref Color result)
        {
            var percOk = MathHelper.Clamp(perc, 0, 1);

            result.R = (byte) (From.R + (DiffR * percOk));
            result.G = (byte) (From.G + (DiffG * percOk));
            result.B = (byte) (From.B + (DiffB * percOk));
        }
    }
}
