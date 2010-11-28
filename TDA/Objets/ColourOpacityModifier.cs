namespace TDA
{
    using System;
    using System.Collections.Generic;
    using ProjectMercury;
    using ProjectMercury.Modifiers;
    using Microsoft.Xna.Framework;

    public sealed class ColourOpacityModifier : Modifier
    {
        public float InitialOpacity;
        public float FinalOpacity;
        public Vector3 InitialColour;
        public Vector3 FinalColour;

        public override unsafe void Process(float dt, Particle* particle, int count)
        {
            for (int i = 0; i < count; i++)
            {
                particle->Colour.W = (this.InitialOpacity + ((this.FinalOpacity - this.InitialOpacity) * particle->Age));
                particle->Colour.X = (this.InitialColour.X + ((this.FinalColour.X - this.InitialColour.X) * particle->Age));
                particle->Colour.Y = (this.InitialColour.Y + ((this.FinalColour.Y - this.InitialColour.Y) * particle->Age));
                particle->Colour.Z = (this.InitialColour.Z + ((this.FinalColour.Z - this.InitialColour.Z) * particle->Age));
            }
        }

        public override Modifier DeepCopy()
        {
            return new ColourOpacityModifier
            {
                InitialColour = this.InitialColour,
                InitialOpacity = this.InitialOpacity,
                FinalColour = this.FinalColour,
                FinalOpacity = this.FinalOpacity
            };
        }
    }

}
