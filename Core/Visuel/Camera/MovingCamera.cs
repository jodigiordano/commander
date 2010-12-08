using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Visuel
{
    public class MovingCamera : Camera
    {
        public virtual Vector3 Vitesse              { get; set; }
        private static double TargetGameTime = (1.0 / 60) * 1000;


        public MovingCamera(
            Vector3 positionInitiale,
            Vector3 vitesse,
            Vector2 origine,
            Camera ancienneCamera)
            : base(ancienneCamera)
        {
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
        }


        public MovingCamera(
            FollowingCamera ancienneCamera,
            Vector3 positionInitiale,
            Vector3 vitesse,
            Vector2 origine)
            : base(ancienneCamera)
        {
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
        }


        public override void Update(GameTime gameTime)
        {
            float multiplicateur = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / TargetGameTime);

            Vector3 deplacement = new Vector3(
                Vitesse.X * multiplicateur,
                Vitesse.Y * multiplicateur,
                Vitesse.Z * multiplicateur);

            this.Manuelle = true;

            Vector3 pos = Position;
            this.Position = new Vector3(pos.X + deplacement.X, pos.Y + deplacement.Y, pos.Z + deplacement.Z);
            this.Manuelle = false;
        }
    }
}