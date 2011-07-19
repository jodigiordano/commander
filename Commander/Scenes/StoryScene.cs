namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class StoryScene : Scene
    {
        private string WorldToTransiteTo;
        private Text SkipText;
        private double SkipCounter;
        private bool Skipping;
        private bool TransitionOutInProgress;

        private static double SkippingTime = 1000;

        private Cutscene Cutscene;


        public StoryScene(string name, string transiteTo, Cutscene cutscene) :
            base(1280, 720)
        {
            Name = name;
            ClearColor = Color.White;
            WorldToTransiteTo = transiteTo;
            Cutscene = cutscene;

            SkipText = new Text(
                "Maintain pressed to skip.",
                "Pixelite",
                Color.Transparent,
                new Vector3(0, 300, 0)) { SizeX = 3 }.CenterIt();

            Cutscene.Scene = this;

            Initialize();
        }


        protected void Initialize()
        {
            SkipText.Alpha = 0;
            SkipCounter = 0;
            Skipping = false;

            TransitionOutInProgress = false;

            VisualEffects.Clear();
            PhysicalEffects.Clear();
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Skip();
            Cutscene.Update();

            if (Cutscene.Terminated)
                TransiteToWorld();
        }


        protected override void UpdateVisual()
        {
            SkipText.Alpha = (byte) Math.Min(((SkipCounter / (SkippingTime / 2)) * 255), 255);

            Add(SkipText);
            Cutscene.Draw();
        }


        public override void OnFocus()
        {
            Initialize();
            Cutscene.Initialize();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            DoAction();
        }


        public override void DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            DoCancel();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            DoAction();
        }


        public override void DoKeyReleased(Core.Input.Player p, Keys key)
        {
            DoCancel();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            DoAction();
        }


        public override void DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            DoCancel();
        }


        public override void DoPlayerDisconnected(Core.Input.Player p)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        private void DoAction()
        {
            Skipping = true;
        }


        private void DoCancel()
        {
            Skipping = false;
        }


        private void Skip()
        {
            if (Skipping)
                SkipCounter = Math.Min(SkipCounter + Preferences.TargetElapsedTimeMs, SkippingTime);
            else
                SkipCounter = Math.Max(SkipCounter - Preferences.TargetElapsedTimeMs, 0);

            if (SkipCounter >= SkippingTime)
            {
                Cutscene.Stop();
                TransiteToWorld();
            }
        }


        private void TransiteToWorld()
        {
            if (TransitionOutInProgress)
                return;

            TransiteTo(WorldToTransiteTo);
            TransitionOutInProgress = true;
        }
    }
}
