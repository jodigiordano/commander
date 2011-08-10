namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public interface IPhysical
    {
        Vector3 Position    { get; set; }
        float Speed         { get; set; }
        float Rotation      { get; set; }
    }
}
