namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class CameraController
    {
        public CameraData CameraData;

        private enum EffectState
        {
            ZoomingIn,
            ZoomingOut,
            None
        }

        private Simulator Simulator;

        private EffectState State;
        private int CurrentZoomEffectId;

        private float MaxCameraMovingSpeed;
        private float MaxCameraZoomSpeed;
        private List<SimPlayer> Players;

        private bool UsePausePlayer;


        public CameraController(Simulator simulator)
        {
            Simulator = simulator;

            CurrentZoomEffectId = -1;
            State = EffectState.None;

            MaxCameraMovingSpeed = 10;
            MaxCameraZoomSpeed = 0.001f;

            Players = new List<SimPlayer>();
            CameraData = new CameraData();
        }


        public void Initialize()
        {
            CameraData.MaxZoomIn = Preferences.BackBufferZoom;
            CameraData.MaxZoomOut = Math.Min(
                Simulator.Scene.CameraView.Width / (float) Simulator.Battlefield.Width,
                Simulator.Scene.CameraView.Height / (float) Simulator.Battlefield.Height);

            Players.Clear();
            UsePausePlayer = false;
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            Players.Add(p);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Players.Remove(p);
        }


        public void DoPanelOpened()
        {
            UsePausePlayer = true;
        }


        public void DoPanelClosed()
        {
            UsePausePlayer = false;
        }


        public void Update()
        {
            if (Players.Count == 0)
                return;

            ComputeNewCameraPosition();
            ComputeNewCameraZoom();
            CameraData.Zoom = Simulator.Scene.Camera.Zoom;
            CameraData.Update();
        }


        public void ZoomIn()
        {
            if (State == EffectState.ZoomingIn)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, CameraData.MaxZoomIn, 0, 500), EffectTerminated);

            State = EffectState.ZoomingIn;
        }


        public void ZoomOut()
        {
            if (State == EffectState.ZoomingOut)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, CameraData.MaxZoomOut, 0, 500), EffectTerminated);

            State = EffectState.ZoomingOut;
        }


        public Vector3 ClampToCamera(Vector3 input, Vector3 padding)
        {
            return new Vector3(
                MathHelper.Clamp(input.X, Simulator.Scene.CameraView.Left + padding.X, Simulator.Scene.CameraView.Right - padding.X),
                MathHelper.Clamp(input.Y, Simulator.Scene.CameraView.Top + padding.Y, Simulator.Scene.CameraView.Bottom - padding.Y),
                0);
        }


        public void DoFocusGained()
        {
            CameraData.ZoomChangedOverride = true;
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
                newPosition += UsePausePlayer ? p.PausePlayer.Position : p.Position;

            Vector3.Divide(ref newPosition, Players.Count, out newPosition);

            var delta = newPosition - Simulator.Scene.Camera.Position;

            delta.X = MathHelper.Clamp(delta.X, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);
            delta.Y = MathHelper.Clamp(delta.Y, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);

            Simulator.Scene.Camera.Position += delta;
        }


        private void ComputeNewCameraZoom()
        {
            var boundaries = UsePausePlayer ? GetPausePlayersBoundaries() : GetPlayersBoundaries();

            float width = Math.Abs(boundaries.Y - boundaries.X) + 100; //padding
            float height = Math.Abs(boundaries.W - boundaries.Z) + 100; //padding
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


        private Vector4 GetPlayersBoundaries()
        {
            var result = new Vector4(
                Players[0].Position.X,
                Players[0].Position.X,
                Players[0].Position.Y,
                Players[0].Position.Y);

            foreach (var p in Players)
            {
                if (p.Position.X < result.X)
                    result.X = p.Position.X;

                if (p.Position.X > result.Y)
                    result.Y = p.Position.X;

                if (p.Position.Y < result.Z)
                    result.Z = p.Position.Y;

                if (p.Position.Y > result.W)
                    result.W = p.Position.Y;
            }

            return result;
        }


        private Vector4 GetPausePlayersBoundaries()
        {
            var result = new Vector4(
                Players[0].PausePlayer.Position.X,
                Players[0].PausePlayer.Position.X,
                Players[0].PausePlayer.Position.Y,
                Players[0].PausePlayer.Position.Y);

            foreach (var p in Players)
            {
                if (p.PausePlayer.Position.X < result.X)
                    result.X = p.PausePlayer.Position.X;

                if (p.PausePlayer.Position.X > result.Y)
                    result.Y = p.PausePlayer.Position.X;

                if (p.PausePlayer.Position.Y < result.Z)
                    result.Z = p.PausePlayer.Position.Y;

                if (p.PausePlayer.Position.Y > result.W)
                    result.W = p.PausePlayer.Position.Y;
            }

            return result;
        }
    }
}
