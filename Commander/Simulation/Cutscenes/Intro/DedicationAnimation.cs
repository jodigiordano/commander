namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class DedicationAnimation
    {
        private Text Text;
        private Scene Scene;
        private double TimeIn;

        private static List<string> AvailableDedications = new List<string>()
        {
            "\"To dedication: you\n\nadded drama where I\n\nneeded it. Thank you.\"",
            "\"To big pixels: you\n\nreduced my canvas and\n\ngave me artistic wings.\n\nThank you.\"",
            "Based on a true fiction.",
            "This product is made from\n\nrecycled materials."
        };


        public DedicationAnimation(Scene scene)
        {
            Scene = scene;

            Text = new Text("", @"Pixelite", new Vector3(200, 100, 0))
            {
                Data = AvailableDedications[Main.Random.Next(0, AvailableDedications.Count)],
                Alpha = 0,
                SizeX = 2,
                VisualPriority = VisualPriorities.Cutscenes.IntroDedication
            };

            TimeIn = IntroCutscene.Timing["DedicationIn"];

            Scene.VisualEffects.Add(Text, Core.Visual.VisualEffects.FadeInFrom0(255, TimeIn, 4000));
            Scene.VisualEffects.Add(Text, Core.Visual.VisualEffects.FadeOutTo0(255, TimeIn + 5000, 2000));
        }


        public void Draw()
        {
            Scene.Add(Text);
        }
    }
}
