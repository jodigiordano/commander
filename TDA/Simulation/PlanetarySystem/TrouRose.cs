namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
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
            ParticuleEffectWrapper representation,
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
                ((ColourInterpolatorModifier)representationParticules.ParticleEffect[0].Modifiers[2]).MiddleColour = value.ToVector3();
            }
        }

        public TypeMelange Melange
        {
            set
            {
                representationParticules.Melange = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.AnciennePosition = this.Position;
            ((RadialGravityModifier)representationParticules.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {

        }
    }
}
