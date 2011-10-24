﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;


    abstract class CommanderScene : Scene
    {
        public PhysicalRectangle CameraView;


        public CommanderScene(string name)
            : base(Preferences.BackBuffer)
        {
            Name = name;
            CameraView = new PhysicalRectangle();
            Camera.Changed += new Core.CameraHandler(DoCameraChanged);
            Camera.Zoom = Preferences.BackBufferZoom;

            DoCameraChanged(Camera);
        }


        private void DoCameraChanged(Camera c)
        {
            var deadZone = Preferences.DeadZoneV2;

            CameraView.X = (int) (c.Rectangle.X + deadZone.X);
            CameraView.Y = (int) (c.Rectangle.Y + deadZone.Y);
            CameraView.Width = (int) (c.Rectangle.Width - (deadZone.X * 2));
            CameraView.Height = (int) (c.Rectangle.Height - (deadZone.Y * 2));
        }
    }
}
