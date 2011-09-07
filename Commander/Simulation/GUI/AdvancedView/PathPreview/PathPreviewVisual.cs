namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;


    class PathPreviewVisual
    {
        private enum PPVState
        {
            ShowingStarted,
            ShowingEnded,
            HidingStarted,
            HidingEnded
        }

        private Path NewPath;
        private Path ActualPath;
        private EffectsController<IVisual> EffectsController;
        private NoneHandler HideCompletedCallback;
        private PPVState State;


        public PathPreviewVisual(Path newPath, Path actualPath, NoneHandler hideCompletedCallback)
        {
            NewPath = newPath;
            ActualPath = actualPath;
            HideCompletedCallback = hideCompletedCallback;
            EffectsController = new EffectsController<IVisual>();
            State = PPVState.HidingEnded;
        }


        public void Update()
        {
            EffectsController.Update(Preferences.TargetElapsedTimeMs);
        }


        public void Cancel()
        {
            EffectsController.StopAllKeepCurrentState();
        }


        public void Show()
        {
            if (State == PPVState.ShowingStarted || State == PPVState.ShowingEnded)
                return;

            var effect = VisualEffects.Fade(ActualPath.Alpha, 25, 0, 500);
            EffectsController.Add(ActualPath, effect, ShowCompleted);
            effect = VisualEffects.Fade(NewPath.Alpha, 100, 0, 500);
            EffectsController.Add(NewPath, effect);

            State = PPVState.ShowingStarted;
        }


        public void Hide()
        {
            if (State == PPVState.HidingStarted || State == PPVState.HidingEnded)
                return;

            var effect = VisualEffects.Fade(ActualPath.Alpha, 100, 0, 500);
            EffectsController.Add(ActualPath, effect, HideCompleted);
            effect = VisualEffects.Fade(NewPath.Alpha, 0, 0, 500);
            EffectsController.Add(NewPath, effect);

            State = PPVState.HidingStarted;
        }


        private void ShowCompleted(int id)
        {
            State = PPVState.ShowingEnded;
        }


        private void HideCompleted(int id)
        {
            HideCompletedCallback();
            State = PPVState.HidingEnded;
        }
    }
}
