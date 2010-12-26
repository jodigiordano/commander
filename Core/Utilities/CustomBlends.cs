namespace EphemereGames.Core.Utilities
{
    using Microsoft.Xna.Framework.Graphics;


    static class CustomBlends
    {
        private static BlendState alpha;
        public static BlendState Alpha
        {
            get
            {
                if (alpha == null)
                {
                    alpha = new BlendState();
                    alpha.AlphaSourceBlend = Blend.One;
                    alpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                    alpha.ColorBlendFunction = BlendFunction.Add;
                    alpha.ColorSourceBlend = Blend.SourceAlpha;
                    alpha.ColorDestinationBlend = Blend.InverseSourceAlpha;
                }

                return alpha;
            }
        }


        private static BlendState substract;
        public static BlendState Substract
        {
            get
            {
                if (substract == null)
                {
                    substract = new BlendState();
                    substract.AlphaSourceBlend = Blend.One;
                    substract.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                    substract.ColorBlendFunction = BlendFunction.ReverseSubtract;
                    substract.ColorSourceBlend = Blend.SourceAlpha;
                    substract.ColorDestinationBlend = Blend.InverseSourceAlpha;
                }

                return substract;
            }
        }


        private static BlendState multiply;
        public static BlendState Multiply
        {
            get
            {
                if (multiply == null)
                {
                    multiply = new BlendState();
                    multiply.AlphaSourceBlend = Blend.One;
                    multiply.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                    multiply.ColorBlendFunction = BlendFunction.Max;
                    multiply.ColorSourceBlend = Blend.SourceAlpha;
                    multiply.ColorDestinationBlend = Blend.InverseSourceAlpha;
                }

                return multiply;
            }
        }
    }
}
