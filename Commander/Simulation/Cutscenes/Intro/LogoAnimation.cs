namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LogoAnimation
    {
        private Text Logo;
        private Text Presents;
        private Scene Scene;
        private double TimeIn;


        public LogoAnimation(Scene scene)
        {
            Scene = scene;

            Logo = new Text("Ephemere Games", "Pixelite")
            {
                Alpha = 0,
                VisualPriority = VisualPriorities.Cutscenes.IntroLogo,
                SizeX = 6
            };
            Logo.CenterIt();

            Presents = new Text("Presents", "Pixelite", new Vector3(0, 60, 0))
            {
                Alpha = 0,
                VisualPriority = VisualPriorities.Cutscenes.IntroLogo,
                SizeX = 4
            };
            Presents.CenterIt();

            TimeIn = IntroCutscene.Timing["LogoIn"];

            Scene.VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn, 4000));
            Scene.VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 6000, 2000));
            Scene.VisualEffects.Add(Logo, Core.Visual.VisualEffects.ChangeSize(6f, 7f, TimeIn, 10000));

            Scene.VisualEffects.Add(Presents, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn + 1000, 3000));
            Scene.VisualEffects.Add(Presents, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 6000, 2000));
            Scene.VisualEffects.Add(Presents, Core.Visual.VisualEffects.ChangeSize(4f, 5f, TimeIn, 10000));
        }


        public void Draw()
        {
            Scene.Add(Logo);
            Scene.Add(Presents);
        }
    }
}
