namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;

    class AnimationEtoileFilante : Animation
    {
        private Trajet3D Trajet;
        private Matrix MatriceRotation;
        private ParticuleEffectWrapper EffetEtoileFilante;
        private Simulation Simulation;
        private Vector3 PositionEmission;
        private Vector3 DirectionTrainee;

        private static Vector3[] Couleurs = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0.5f, 0)
        };

        public AnimationEtoileFilante(Simulation simulation)
            : base ()
        {
            Simulation = simulation;

            Vector3 pointDepart = new Vector3(simulation.Terrain.Width / 2 + 100, 0, 0);
            Vector3 pointArrivee;
            Vector3 pointMilieu;

            float rotation = MathHelper.ToRadians(Main.Random.Next(-360, 360));

            Matrix.CreateRotationZ(rotation, out MatriceRotation);
            Vector3.Transform(ref pointDepart, ref MatriceRotation, out pointDepart);

            pointArrivee = -pointDepart;

            pointMilieu = pointDepart;
            rotation = (Main.Random.Next(0, 2) == 1) ? -MathHelper.PiOver2 : MathHelper.PiOver2;
            Matrix.CreateRotationZ(rotation, out MatriceRotation);
            Vector3.Transform(ref pointMilieu, ref MatriceRotation, out pointMilieu);
            pointMilieu.Normalize();
            Vector3.Multiply(ref pointMilieu, Main.Random.Next(200, 400), out pointMilieu);

            double temps = Main.Random.Next(800, 2000);

            Trajet = new Trajet3D
            (
                new Vector3[]
                {
                    pointDepart,
                    pointMilieu,
                    pointArrivee
                },

                new double[]
                {
                    0,
                    temps,
                    temps * 2,
                }
            );

            EffetEtoileFilante = Simulation.Scene.Particules.recuperer("etoileFilante");
            EffetEtoileFilante.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            base.Duree = temps * 2;

            ((ColourInterpolatorModifier) EffetEtoileFilante.ParticleEffect[0].Modifiers[1]).InitialColour = Couleurs[Main.Random.Next(0, Couleurs.Length)];
        }

        public override void suivant(GameTime gameTime)
        {
            base.suivant(gameTime);

            Trajet.Direction(base.TempsRelatif, out DirectionTrainee);
            ((ConeEmitter)EffetEtoileFilante.ParticleEffect[0]).Direction = (float)Math.Atan2(DirectionTrainee.Y, DirectionTrainee.X) - MathHelper.Pi;

            Trajet.Position(base.TempsRelatif, ref PositionEmission);
            EffetEtoileFilante.Emettre(ref PositionEmission);
        }

        public override void Draw(SpriteBatch spriteBatch) { }
    }
}
