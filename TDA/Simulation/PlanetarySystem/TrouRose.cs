namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class TrouRose : CorpsCeleste
    {
        public TrouRose(
            Simulation simulation,
            String nom,
            Vector3 positionBase,
            Vector3 offset,
            float rayon,
            double tempsRotation,
            Particle representation,
            int pourcDepart,
            float prioriteAffichage,
            bool enBackground,
            int rotation)
            : base (simulation, nom, positionBase, offset, rayon, tempsRotation, representation, pourcDepart, prioriteAffichage, enBackground, rotation)
        {
            Lunes = new List<Lune>();
        }

        public Color Couleur
        {
            set
            {
                ((ColourInterpolatorModifier)ParticulesRepresentation.ParticleEffect[0].Modifiers[2]).MiddleColour = value.ToVector3();
            }
        }

        public TypeBlend Melange
        {
            set
            {
                ParticulesRepresentation.Blend = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.AnciennePosition = this.Position;
            ((RadialGravityModifier)ParticulesRepresentation.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            base.Update(gameTime);
        }


        public override void Draw()
        {

        }
    }
}
