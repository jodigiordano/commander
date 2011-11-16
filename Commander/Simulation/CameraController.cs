﻿namespace EphemereGames.Commander.Simulation
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

        private float MaxZoomWidth;
        private float MaxZoomHeight;
        private float MaxCameraMovingSpeed;
        private float MaxCameraZoomSpeed;
        private List<SimPlayer> Players;

        private int MaxZoomLevel;
        private int ZoomLevel;


        public CameraController(Simulator simulator)
        {
            Simulator = simulator;

            CurrentZoomEffectId = -1;
            State = EffectState.None;

            MaxZoomWidth = Preferences.BattlefieldBoundaries.X * Preferences.BackBufferZoom;
            MaxZoomHeight = Preferences.BattlefieldBoundaries.Y * Preferences.BackBufferZoom;
            MaxCameraMovingSpeed = 10;
            MaxCameraZoomSpeed = 0.001f;

            Players = new List<SimPlayer>();
            CameraData = new CameraData();

            MaxZoomLevel = 2;
            ZoomLevel = 2;
        }


        public void Initialize()
        {
            CameraData.MaxZoomIn = Preferences.BackBufferZoom;
            CameraData.MaxZoomOut = Math.Min(
                MaxZoomWidth / Simulator.Data.Battlefield.Inner.Width,
                MaxZoomHeight / Simulator.Data.Battlefield.Inner.Height);
            CameraData.MaxDelta = Math.Abs(CameraData.MaxZoomIn - CameraData.MaxZoomOut);

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

            if (!CameraData.ManualZoom)
                ComputeNewCameraZoom();

            CameraData.Zoom = Simulator.Scene.Camera.Zoom;
            CameraData.Update();
        }


        public void ZoomIn()
        {
            if (ZoomLevel >= MaxZoomLevel)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            ZoomLevel++;
            var zoom = CameraData.MaxZoomIn - (ZoomLevel >= MaxZoomLevel ? 0 : (CameraData.MaxDelta * ZoomLevel / MaxZoomLevel));

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, zoom, 0, 500), EffectTerminated);

            State = EffectState.ZoomingIn;
            
            if (ZoomLevel >= MaxZoomLevel)
                CameraData.ManualZoom = false;
        }


        public void ZoomOut()
        {
            if (ZoomLevel <= 0)
                return;

            Simulator.Scene.VisualEffects.CancelEffect(CurrentZoomEffectId);

            ZoomLevel--;
            var zoom = CameraData.MaxZoomOut + (ZoomLevel <= 0 ? 0 : (CameraData.MaxDelta * ZoomLevel / MaxZoomLevel));

            CurrentZoomEffectId = Simulator.Scene.VisualEffects.Add(
                Simulator.Scene.Camera,
                Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, zoom, 0, 500), EffectTerminated);

            State = EffectState.ZoomingOut;
            CameraData.ManualZoom = true;
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
                newPosition += p.Position;

            Vector3.Divide(ref newPosition, Players.Count, out newPosition);

            var delta = newPosition - Simulator.Scene.Camera.Position;

            delta.X = MathHelper.Clamp(delta.X, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);
            delta.Y = MathHelper.Clamp(delta.Y, -MaxCameraMovingSpeed, MaxCameraMovingSpeed);

            Simulator.Scene.Camera.Position += delta;
        }


        private void ComputeNewCameraZoom()
        {
            var boundaries = GetPlayersBoundaries();

            float width = Math.Abs(boundaries.Y - boundaries.X) + 100; //padding
            float height = Math.Abs(boundaries.W - boundaries.Z) + 100; //padding

            float newZoom = MathHelper.Clamp(Math.Min(MaxZoomWidth / width, MaxZoomHeight / height), CameraData.MaxZoomOut, CameraData.MaxZoomIn);

            // no need toLocal zoom the camera
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
    }
}
