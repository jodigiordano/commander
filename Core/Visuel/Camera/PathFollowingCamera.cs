namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    public class PathFollowingCamera : FollowingCamera
    {
        new private IVisible ObjetSuivi          { get; set; }
        new private Vector3 Vitesse              { get; set; }
        public Trajet2D Trajet                     { get; private set; }
        private double TempsTotal                { get; set; }


        public PathFollowingCamera(
            Trajet2D trajet,
            float positionZ,
            Vector2 origine,
            Camera ancienneCamera)
            : base(null, new Vector3(0, 0, positionZ), Vector3.Zero, Vector3.Zero, true, origine, ancienneCamera)
        {
            this.Trajet = trajet;
            this.Position = new Vector3(Trajet.positionDepart(), this.Position.Z);
            this.Manuelle = true;
            this.TempsTotal = 0;
        }


        public PathFollowingCamera(PathFollowingCamera ancienneCamera, Trajet2D trajet) :
            base(ancienneCamera, ancienneCamera.Position, ancienneCamera.DistanceMaximale, ancienneCamera.Vitesse, ancienneCamera.Manuelle, ancienneCamera.Origine)
        {
            this.Trajet = trajet;
            this.TempsTotal = TempsTotal;
        }


        public bool DestinationAtteinte
        {
            get
            {
                return this.Trajet.position(TempsTotal) == this.Trajet.positionFin();
            }
        }


        protected override void deplacementManuel(GameTime gameTime)
        {
            this.TempsTotal += gameTime.ElapsedGameTime.TotalMilliseconds;

            Vector2 posTrajet = Trajet.position(this.TempsTotal);
            this.Position = new Vector3(posTrajet.X, posTrajet.Y, this.Position.Z);
        }
    }
}