namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Persistance;
    using EphemereGames.Core.Physique;
    using ProjectMercury.Emitters;


    class PulseBullet : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            Image = null;

            Rectangle = new RectanglePhysique((int) Position.X - 10, (int) Position.Y - 10, 20, 20);

            MovingEffect = Scene.Particles.Get("pulseEffect");
            ExplodingEffect = null;
        }


        public override void DoHit(ILivingObject by) {}
    }
}