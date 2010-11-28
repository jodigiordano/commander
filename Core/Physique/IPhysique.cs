namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public interface IPhysique
    {
        Vector3 Position { get; set; }
        float Vitesse { get; set; }
        float Rotation { get; set; }
    }
}
