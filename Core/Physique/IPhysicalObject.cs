namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public interface IPhysicalObject
    {
        Vector3 Position    { get; set; }
        float Speed         { get; set; }
        float Rotation      { get; set; }
    }
}
