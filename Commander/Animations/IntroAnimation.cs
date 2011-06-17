namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class IntroAnimation : Animation
    {
        private Text PlaceHolder;


        public IntroAnimation()
            : base(10000, 0.5f)
        {
            PlaceHolder = new Text(
                "Something epic will happen here.",
                "Pixelite",
                Color.Black,
                Vector3.Zero) { SizeX = 4 }.CenterIt();
        }


        public override void Draw()
        {
            Scene.Add(PlaceHolder);
        }
    }
}
