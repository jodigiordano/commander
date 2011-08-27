namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class StoryScene : Scene
    {
        private string WorldToTransiteTo;
        private double SkipCounter;
        private bool Skipping;
        private bool TransitionOutInProgress;

        private static double SkippingTime = 1000;

        private Cutscene Cutscene;
        private HelpBarPanel HelpBar;
        private InputType InputType;


        public StoryScene(string name, string transiteTo, Cutscene cutscene) :
            base(Preferences.BackBuffer)
        {
            Name = name;
            ClearColor = Color.White;
            WorldToTransiteTo = transiteTo;
            Cutscene = cutscene;

            Cutscene.Scene = this;

            Particles.Add(@"selectionCorpsCeleste");

            HelpBar = new HelpBarPanel(this, VisualPriorities.Cutscenes.HelpBar)
            {
                ShowOnForegroundLayer = true
            };
            HelpBar.ShowMessage(HelpBarMessage.HoldToSkip, InputType.Gamepad);
            InputType = InputType.Gamepad;

            Initialize();
        }


        protected void Initialize()
        {
            HelpBar.Alpha = 0;
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
            {
                Cutscene.Stop();
                TransiteToWorld();
            }
        }


        protected override void UpdateVisual()
        {
            HelpBar.Alpha = (byte) Math.Min(((SkipCounter / (SkippingTime / 2)) * 255), 255);

            HelpBar.Draw();

            Cutscene.Draw();
        }


        public override void OnFocus()
        {
            Initialize();
            Cutscene.Initialize();
        }


        public override void OnFocusLost()
        {
            Inputs.StopAllVibrators();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Select)
            {
                VerifyInputType(p.InputType);
                DoAction();
            }
        }


        public override void DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Select)
                DoCancel();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.MoveUp)
            {
                VerifyInputType(p.InputType);
                DoAction();
            }
        }


        public override void DoKeyReleased(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.MoveUp)
                DoCancel();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
            {
                VerifyInputType(p.InputType);
                DoAction();
            }
        }


        public override void DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
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


        private void VerifyInputType(InputType input)
        {
            if (InputType != input)
            {
                InputType = input;

                HelpBar.HideMessage(HelpBarMessage.HoldToSkip);
                HelpBar.ShowMessage(HelpBarMessage.HoldToSkip, input);
                HelpBar.Alpha = 0;
            }
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
