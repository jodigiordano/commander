namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;


    class CameraController
    {
        private enum EffectState
        {
            ZoomingIn,
            ZoomingOut,
            None
        }

        private Simulator Simulator;
        private float MaxZoomIn;
        private float MaxZoomOut;

        private EffectState State;
        private int CurrentZoomEffectId;

        private float MaxCameraSpeed;


        public CameraController(Simulator simulator)
        {
            Simulator = simulator;

            CurrentZoomEffectId = -1;
            State = EffectState.None;

            MaxCameraSpeed = 10;
        }


        public void Initialize()
        {
            MaxZoomIn = Preferences.BackBufferZoom;
            MaxZoomOut = Math.Max(
                Simulator.Scene.CameraView.Width / (float) Simulator.Battlefield.Width,
                Simulator.Scene.CameraView.Height / (float) Simulator.Battlefield.Height);
        }


        public void Update()
        {
            var player = (Commander.Player) Inputs.MasterPlayer;

            if (player == null || player.State != PlayerState.Connected)
                return;

            if (Simulator.EditorMode && Simulator.EditorState == Simulation.EditorState.Editing)
            {
                Simulator.Scene.Camera.Position = player.Position;
            }

            else
            {
                var newPosition = new Vector3(
                    MathHelper.Clamp(player.Position.X, Simulator.Battlefield.Left + Simulator.Scene.CameraView.Width / 2, Simulator.Battlefield.Right - Simulator.Scene.CameraView.Width / 2),
                    MathHelper.Clamp(player.Position.Y, Simulator.Battlefield.Top + Simulator.Scene.CameraView.Height / 2, Simulator.Battlefield.Bottom - Simulator.Scene.CameraView.Height / 2),
                    0);

                var delta = newPosition - Simulator.Scene.Camera.Position;

                delta.X = MathHelper.Clamp(delta.X, -MaxCameraSpeed, MaxCameraSpeed);
                delta.Y = MathHelper.Clamp(delta.Y, -MaxCameraSpeed, MaxCameraSpeed);

                Simulator.Scene.Camera.Position += delta;
            }
        }


        public void ZoomIn()
        {
            if (State == EffectState.ZoomingIn)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, MaxZoomIn, 0, 500), EffectTerminated);

            State = EffectState.ZoomingIn;
        }


        public void ZoomOut()
        {
            if (State == EffectState.ZoomingOut)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, MaxZoomOut, 0, 500), EffectTerminated);

            State = EffectState.ZoomingOut;
        }


        public Vector3 ClampToCamera(Vector3 input, Vector3 padding)
        {
            return new Vector3(
                MathHelper.Clamp(input.X, Simulator.Scene.CameraView.Left - padding.X, Simulator.Scene.CameraView.Right - padding.X),
                MathHelper.Clamp(input.Y, Simulator.Scene.CameraView.Top - padding.Y, Simulator.Scene.CameraView.Bottom - padding.Y),
                0);
        }


        private void EffectTerminated(int id)
        {
            CurrentZoomEffectId = -1;
            State = EffectState.None;
        }
    }
}
