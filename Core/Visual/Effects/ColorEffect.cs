namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class ColorEffect : Effect<IVisual>
    {
        public Color FinalColor;
        private Color InitialColor;
        private Color IntermediateColor;

        public static Pool<ColorEffect> Pool = new Pool<ColorEffect>();


        protected override void InitializeLogic()
        {
            InitialColor = Obj.Color;
            IntermediateColor = InitialColor;
        }


        protected override void LogicLinear()
        {
            double pourc = ElaspedTime / Length;

            IntermediateColor.R = (byte) (InitialColor.R + (FinalColor.R - InitialColor.R) * pourc);
            IntermediateColor.G = (byte) (InitialColor.G + (FinalColor.G - InitialColor.G) * pourc);
            IntermediateColor.B = (byte) (InitialColor.B + (FinalColor.B - InitialColor.B) * pourc);

            Obj.Color = IntermediateColor;
        }


        protected override void LogicAfter()
        {
            Obj.Color = FinalColor;
        }


        protected override void LogicNow()
        {
            Obj.Color = FinalColor;
        }


        internal override void Return()
        {
            Pool.Return((ColorEffect) this);
        }
    }
}
