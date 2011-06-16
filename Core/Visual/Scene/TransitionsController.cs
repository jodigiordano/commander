namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class TransitionsController
    {
        public virtual event NoneHandler TransitionStarted;
        public virtual event NoneHandler TransitionTerminated;

        public bool InTransition                        { get; private set; }
        public ITransitionAnimation TransitionAnimation { private get; set; }

        private Dictionary<string, Transition> Transitions;
        private Transition CurrentTransition;


        public TransitionsController()
        {
            InTransition = false;
            Transitions = new Dictionary<string, Transition>();
        }


        public void AddTransition(Transition transition)
        {
            Transitions[transition.Name] = transition;
        }


        public void Transite(string transitionName)
        {
            CurrentTransition = Transitions[transitionName];

            StartFromToTransition();
        }


        public void Update(GameTime gameTime)
        {
            if (TransitionAnimation == null)
            {
                StartToFromTransition();
                EndToFromTransition();
                return;
            }

            TransitionAnimation.Update(gameTime);

            if (TransitionAnimation.IsFinished && CurrentTransition.CurrentType == TransitionType.Out)
                StartToFromTransition();
            else if (TransitionAnimation.IsFinished && CurrentTransition.CurrentType == TransitionType.In)
                EndToFromTransition();
        }


        public void Draw()
        {
            if (InTransition && TransitionAnimation != null)
                TransitionAnimation.Draw();
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

            if (TransitionAnimation != null)
            {
                CurrentTransition.ActiveTransition = CurrentTransition.From;
                CurrentTransition.CurrentType = TransitionType.Out;
                TransitionAnimation.Scene = CurrentTransition.ActiveTransition;
                TransitionAnimation.Initialize(CurrentTransition.CurrentType);
            }
        }


        private void StartToFromTransition()
        {
            CurrentTransition.From.EnableVisuals = false;
            CurrentTransition.To.EnableVisuals = true;
            CurrentTransition.From.EnableInputs = false;
            CurrentTransition.To.EnableInputs = false;
            CurrentTransition.From.EnableUpdate = true;
            CurrentTransition.To.EnableUpdate = true;

            CurrentTransition.To.OnFocus();

            if (TransitionAnimation != null)
            {
                CurrentTransition.ActiveTransition = CurrentTransition.To;
                CurrentTransition.CurrentType = TransitionType.In;
                TransitionAnimation.Scene = CurrentTransition.ActiveTransition;
                TransitionAnimation.Initialize(CurrentTransition.CurrentType);
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
