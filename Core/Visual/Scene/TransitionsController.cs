namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class TransitionsController
    {
        public virtual event NoneHandler TransitionStarted;
        public virtual event NoneHandler TransitionTerminated;

        public bool InTransition                                { get; private set; }
        public List<ITransitionAnimation> TransitionAnimations  { private get; set; }

        private Transition CurrentTransition;
        private ITransitionAnimation CurrentTansitionAnimation;


        public TransitionsController()
        {
            InTransition = false;
        }


        public void Transite(string from, string to)
        {
            CurrentTransition = new Transition(from, to);
            CurrentTansitionAnimation = TransitionAnimations != null ? TransitionAnimations[Preferences.Random.Next(0, TransitionAnimations.Count)] : null;

            StartFromToTransition();
        }


        public void Update(GameTime gameTime)
        {
            if (CurrentTansitionAnimation == null)
            {
                StartToFromTransition();
                EndToFromTransition();
                return;
            }

            CurrentTansitionAnimation.Update(gameTime);

            if (CurrentTansitionAnimation.IsFinished && CurrentTransition.CurrentType == TransitionType.Out)
                StartToFromTransition();
            else if (CurrentTansitionAnimation.IsFinished && CurrentTransition.CurrentType == TransitionType.In)
                EndToFromTransition();
        }


        public void Draw()
        {
            if (InTransition && TransitionAnimations != null)
                CurrentTansitionAnimation.Draw();
        }


        private void StartFromToTransition()
        {
            InTransition = true;

            CurrentTransition.From.EnableVisuals = true;
            CurrentTransition.To.EnableVisuals = false;
            CurrentTransition.From.EnableInputs = false;
            CurrentTransition.To.EnableInputs = false;
            CurrentTransition.From.EnableUpdate = true;
            CurrentTransition.To.EnableUpdate = true;

            CurrentTransition.From.OnFocusLost();

            NotifyTransitionStarted();

            if (TransitionAnimations != null)
            {
                CurrentTransition.ActiveTransition = CurrentTransition.From;
                CurrentTransition.CurrentType = TransitionType.Out;
                CurrentTansitionAnimation.Scene = CurrentTransition.ActiveTransition;
                CurrentTansitionAnimation.Initialize(CurrentTransition.CurrentType);
            }
        }


        private void StartToFromTransition()
        {
            CurrentTransition.From.EnableVisuals = false;
            CurrentTransition.To.EnableVisuals = true;

            CurrentTransition.To.OnFocus();

            if (TransitionAnimations != null)
            {
                CurrentTransition.ActiveTransition = CurrentTransition.To;
                CurrentTransition.CurrentType = TransitionType.In;
                CurrentTansitionAnimation.Scene = CurrentTransition.ActiveTransition;
                CurrentTansitionAnimation.Initialize(CurrentTransition.CurrentType);
            }
        }


        private void EndToFromTransition()
        {
            InTransition = false;
            CurrentTransition.From.EnableInputs = false;
            CurrentTransition.To.EnableInputs = true;
            CurrentTransition.From.EnableUpdate = false;
            CurrentTransition.To.EnableUpdate = true;

            NotifyTransitionTerminated();
        }


        private void NotifyTransitionStarted()
        {
            if (TransitionStarted != null)
                TransitionStarted();
        }


        private void NotifyTransitionTerminated()
        {
            if (TransitionTerminated != null)
                TransitionTerminated();
        }
    }
}
