﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldAnnunciationScene : Scene
    {
        private WorldDescriptor Descriptor;
        private Text WorldId;
        private Translator WorldName;

        private double Length;
        private bool TransitionInProgress;


        public WorldAnnunciationScene(WorldDescriptor descriptor) :
            base(1280, 720)
        {
            Name = "World" + descriptor.Id + "Annunciation";

            Descriptor = descriptor;

            WorldId = new Text("World " + Descriptor.Id, "Pixelite")
            {
                SizeX = 4,
                Color = Color.Black
            }.CenterIt();
        }


        public void Initialize()
        {
            Length = 2500;
            TransitionInProgress = false;

            WorldName = new Translator(this, new Vector3(0, 50, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", Color.Black, Descriptor.Name, 3, true, 1500, 100, Preferences.PrioriteGUIPanneauGeneral);
            WorldName.CenterText = true;

            VisualEffects.Add(WorldName.ToTranslate, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
            VisualEffects.Add(WorldName.Translated, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (TransitionInProgress)
                return;

            Length -= Preferences.TargetElapsedTimeMs;

            if (Length <= 0)
            {
                TransiteTo("World" + Descriptor.Id);
                TransitionInProgress = true;
            }

            WorldName.Update();
        }


        protected override void UpdateVisual()
        {
            Add(WorldId);

            WorldName.Draw();
        }


        public override void OnFocus()
        {
            Initialize();
        }


        public override void OnFocusLost()
        {

        }
    }
}