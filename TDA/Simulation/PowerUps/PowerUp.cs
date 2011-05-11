namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;

    interface PowerUp
    {
        PowerUpType Type    { get; }
        String BuyImage     { get; }
        int BuyPrice        { get; }
        Vector3 BuyPosition { get; set; }
    }
}
