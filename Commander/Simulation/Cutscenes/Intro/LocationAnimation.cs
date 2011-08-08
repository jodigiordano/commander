namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LocationAnimation
    {
        private Text Where;
        private Text Time;
        private Scene Scene;
        private double TimeIn;


        public LocationAnimation(Scene scene)
        {
            Scene = scene;

            Where = new Text("Far far away, in a tiny colony", "Pixelite", new Vector3(-600, 260, 0))
            {
                SizeX = 3,
                Alpha = 0,
                VisualPriority = VisualPriorities.Cutscenes.IntroLocation
            };


            Time = new Text("Date: Who cares?", "Pixelite", new Vector3(-600, 300, 0))
            {
                SizeX = 3,
                Alpha = 0,
                VisualPriority = VisualPriorities.Cutscenes.IntroLocation
            };

            TimeIn = IntroCutscene.Timing["LocationIn"];

            Scene.VisualEffects.Add(Where, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn, 3000));
            Scene.VisualEffects.Add(Where, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 8000, 3000));

            Scene.VisualEffects.Add(Time, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn + 1000, 3000));
            Scene.VisualEffects.Add(Time, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 8000, 3000));
        }


        public void Draw()
        {
            Scene.Add(Where);
        }
    }
}
