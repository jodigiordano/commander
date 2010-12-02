namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;

    class JoueurCommun
    {
        public int Pointage;
        public int ReserveUnites;
        public int PointsDeVie;
        public Vector3 Position;


        public JoueurCommun(int reserveUnites)
        {
            this.Pointage = 0;
            this.ReserveUnites = reserveUnites;
        }


        public JoueurCommun()
        {
            this.Pointage = 0;
            this.ReserveUnites = 0;
            this.PointsDeVie = 0;
        }

        public void VerifyFrame(int width, int height)
        {
            Position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + width / 2, 640 - Preferences.DeadZoneXbox.X - width / 2);
            Position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + height / 2, 370 - Preferences.DeadZoneXbox.Y - height / 2);
        }
    }
}