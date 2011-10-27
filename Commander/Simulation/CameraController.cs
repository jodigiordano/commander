namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
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

        private float MaxCameraMovingSpeed;
        private float MaxCameraZoomSpeed;
        private List<SimPlayer> Players;


        public CameraController(Simulator simulator)
        {
            Simulator = simulator;

            CurrentZoomEffectId = -1;
            State = EffectState.None;

            MaxCameraMovingSpeed = 10;
            MaxCameraZoomSpeed = 0.002f;

            Players = new List<SimPlayer>();
        }


        public void Initialize()
        {
            MaxZoomIn = Preferences.BackBufferZoom;
            MaxZoomOut = Math.Min(
                Simulator.Scene.CameraView.Width / (float) Simulator.Battlefield.Width,
                Simulator.Scene.CameraView.Height / (float) Simulator.Battlefield.Height);

            Players.Clear();
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            Players.Add(p);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Players.Remove(p);
        }


        public void Update()
        {
            if (Players.Count == 0)
                return;

            ComputeNewCameraPosition();
            ComputeNewCameraZoom();
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
                MathHelper.Clamp(input.X, Simulator.Scene.CameraView.Left + padding.X, Simulator.Scene.CameraView.Right - padding.X),
                MathHelper.Clamp(input.Y, Simulator.Scene.CameraView.Top + padding.Y, Simulator.Scene.CameraView.Bottom - padding.Y),
                0);
        }


        private void EffectTerminated(int id)
        {
            CurrentZoomEffectId = -1;
            State = EffectState.None;
        }


        private void ComputeNewCameraPosition()
        {
            Vector3 newPosition = new Vector3();

            foreach (var p in Players)
                newPosition += p.Position;

            Vector3.Divide(ref newPosition, Players.Count, out newPosition);

            var delta = newPosition - Simulator.Scene.Camera.Position;

            delta.X = MathHelper.Clamp(delta.X, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);
            delta.Y = MathHelper.Clamp(delta.Y, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);

            Simulator.Scene.Camera.Position += delta;
        }


        private void ComputeNewCameraZoom()
        {
            float minX = Players[0].Position.X;
            float maxX = Players[0].Position.X;
            float minY = Players[0].Position.Y;
            float maxY = Players[0].Position.Y;

            foreach (var p in Players)
            {
                if (p.Position.X < minX)
                    minX = p.Position.X;

                if (p.Position.X > maxX)
                    maxX = p.Position.X;

                if (p.Position.Y < minY)
                    minY = p.Position.Y;

                if (p.Position.Y > maxY)
                    maxY = p.Position.Y;
            }

            float width = Math.Abs(maxX - minX) + 100; //padding
            float height = Math.Abs(maxY - minY) + 100; //padding
            float maxZoomWidth = Preferences.BattlefieldBoundaries.X * Preferences.BackBufferZoom;
            float maxZoomHeight = Preferences.BattlefieldBoundaries.Y * Preferences.BackBufferZoom;

            float newZoom = Math.Min(maxZoomWidth / width, maxZoomHeight / height);

            // no need to zoom the camera
            if (newZoom >= Preferences.BackBufferZoom)
                return;

            // no need to zoom the camera
            // reduce events
            if (newZoom == Simulator.Scene.Camera.Zoom)
                return;

            float delta = newZoom - Simulator.Scene.Camera.Zoom;

            delta = MathHelper.Clamp(delta, -MaxCameraZoomSpeed, MaxCameraZoomSpeed);

            Simulator.Scene.Camera.Zoom += delta;
        }
    }
}
