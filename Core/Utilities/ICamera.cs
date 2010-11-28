namespace Core.Utilities
{
    using System;
    using Microsoft.Xna.Framework;

    public interface ICamera
    {
        float calculerFacteurOpacite(ref Vector3 position);
        Vector2 calculerFacteurDeplacement(ref Vector3 position);
        float calculerFacteurTaille(ref Vector3 position);
    }
}
