namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LocationAnimation
    {
        private Text Where;
        private Scene Scene;
        private double TimeIn;


        public LocationAnimation(Scene scene)
        {
            Scene = scene;

            Where = new Text("Far far away, in a tiny colony", @"Pixelite", new Vector3(-600, 260, 0))
            {
                SizeX = 3,
                Alpha = 0,
                VisualPriority = VisualPriorities.Cutscenes.IntroLocation
            };

            TimeIn = IntroCutscene.Timing["LocationIn"];

            Scene.VisualEffects.Add(Where, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn, 2000));
            Scene.VisualEffects.Add(Where, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 4000, 2000));
        }


        public void Draw()
        {
            Scene.Add(Where);
        }
    }
}
