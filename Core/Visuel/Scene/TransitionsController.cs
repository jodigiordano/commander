namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class TransitionsController
    {
        public virtual event NoneHandler TransitionStarted;
        public virtual event NoneHandler TransitionTerminated;

        public bool InTransition { get; private set; }

        private Dictionary<String, Transition> Transitions;
        private Transition CurrentTransition;
        private Dictionary<string, Scene> ScenesInTransition;
        private double RemainingTime;


        public TransitionsController()
        {
            InTransition = false;
            Transitions = new Dictionary<string, Transition>();
            ScenesInTransition = new Dictionary<string, Scene>();
        }


        public void AddTransition(Transition transition)
        {
            Transitions[transition.Name] = transition;
        }


        public void Transite(String transitionName)
        {
            #if DEBUG
            if (InTransition)
                throw new Exception("Already in transition.");
            #endif

            CurrentTransition = Transitions[transitionName];
            InTransition = true;
            RemainingTime = CurrentTransition.Length;
            ScenesInTransition.Clear();

            ScenesInTransition.Add(CurrentTransition.NameSceneFrom, Facade.ScenesController.GetScene(CurrentTransition.NameSceneFrom));
            ScenesInTransition.Add(CurrentTransition.NameSceneTo, Facade.ScenesController.GetScene(CurrentTransition.NameSceneTo));

            ScenesInTransition[CurrentTransition.NameSceneFrom].OnFocusLost();
            ScenesInTransition[CurrentTransition.NameSceneFrom].OnTransitionTowardFocus();

            OnTransitionStarted();
            StartTransition();
        }


        public void Update(GameTime gameTime)
        {
            RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (RemainingTime <= 0)
            {
                InTransition = false;
                EndTransition();

                OnTransitionTerminee();
            }
        }


        private void StartTransition()
        {
            ScenesInTransition[CurrentTransition.NameSceneFrom].Active = true;
            ScenesInTransition[CurrentTransition.NameSceneTo].Active = false;
        }


        private void EndTransition()
        {
            ScenesInTransition[CurrentTransition.NameSceneFrom].Active = false;
            ScenesInTransition[CurrentTransition.NameSceneFrom].Hide();

            ScenesInTransition[CurrentTransition.NameSceneTo].Active = true;
            ScenesInTransition[CurrentTransition.NameSceneTo].Show();

            ScenesInTransition[CurrentTransition.NameSceneTo].OnFocus();
        }


        private void OnTransitionStarted()
        {
            if (TransitionStarted != null)
                TransitionStarted();
        }


        private void OnTransitionTerminee()
        {
            if (TransitionTerminated != null)
                TransitionTerminated();
        }
    }
}
