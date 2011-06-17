namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class StoryScene : Scene
    {
        private string WorldToTransiteTo;
        private Animation StoryAnimation;

        private Text SkipText;
        private double SkipCounter;
        private bool Skipping;

        private bool TransitionOutInProgress;

        private static double SkippingTime = 3000;


        public StoryScene(string name, string transiteTo, Animation storyAnimation) :
            base(Vector2.Zero, 1280, 720)
        {
            Name = name;
            ClearColor = Color.White;
            WorldToTransiteTo = transiteTo;
            StoryAnimation = storyAnimation;

            SkipText = new Text(
                "Maintain pressed to skip.",
                "Pixelite",
                Color.Transparent,
                new Vector3(0, 300, 0)) { SizeX = 3 }.CenterIt();

            Initialize();
        }


        protected void Initialize()
        {
            SkipText.Alpha = 0;
            SkipCounter = 0;
            Skipping = false;

            TransitionOutInProgress = false;

            Animations.Clear();
            Animations.Add(StoryAnimation);
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Skip();
            UpdateAnimation(gameTime);
        }


        protected override void UpdateVisual()
        {
            SkipText.Alpha = (byte) Math.Min(((SkipCounter / (SkippingTime / 2)) * 255), 255);

            Add(SkipText);
        }


        public override void OnFocus()
        {
            Initialize();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (!p.Master)
                return;

            DoAction();
        }


        public override void DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            if (!p.Master)
                return;

            DoCancel();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (!p.Master)
                return;

            DoAction();
        }


        public override void DoKeyReleased(Core.Input.Player p, Keys key)
        {
            if (!p.Master)
                return;

            DoCancel();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (!p.Master)
                return;

            DoAction();
        }


        public override void DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            if (!p.Master)
                return;

            DoCancel();
        }


        public override void DoPlayerDisconnected(Core.Input.Player p)
        {
            if (p.Master)
                TransiteTo("Chargement");
        }


        private void DoAction()
        {
            Skipping = true;
        }


        private void DoCancel()
        {
            Skipping = false;
        }


        private void UpdateAnimation(GameTime gameTime)
        {
            StoryAnimation.Update(gameTime);

            if (StoryAnimation.IsFinished)
                TransiteToWorld();
        }


        private void Skip()
        {
            if (Skipping)
                SkipCounter = Math.Min(SkipCounter + Preferences.TargetElapsedTimeMs, SkippingTime);
            else
                SkipCounter = Math.Max(SkipCounter - Preferences.TargetElapsedTimeMs, 0);

            if (SkipCounter >= SkippingTime)
                TransiteToWorld();
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
