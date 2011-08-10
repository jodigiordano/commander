namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class BulletsController
    {
        public event PhysicalObjectHandler ObjectDestroyed;
        public List<Bullet> Bullets { get; private set; }

        private Simulator Simulator;
        private Matrix RotationMatrix;


        public BulletsController(Simulator simulator)
        {
            Simulator = simulator;

            Bullets = new List<Bullet>();

            RotationMatrix = new Matrix();
        }


        public void Initialize()
        {
            Bullets.Clear();
        }


        public void Update()
        {
            for (int i = Bullets.Count - 1; i > -1; i--)
                if (!Bullets[i].Alive)
                {
                    Bullet b = Bullets[i];

                    if (b.OutOfBounds)
                        b.DoDieSilent();
                    else
                        b.DoDie();
                    
                    NotifyObjectDestroyed(b);

                    Simulator.BulletsFactory.Return(b);

                    Bullets.RemoveAt(i);
                }

            foreach (var bullet in Bullets)
                bullet.Update();
        }


        public void Draw()
        {
            foreach (var bullet in Bullets)
                bullet.Draw();
        }


        public void DoObjectCreated(ICollidable objet)
        {
            Bullet b = objet as Bullet;

            if (b == null)
                return;

            b.Initialize();

            Bullets.Add(b);
        }


        public void DoObjectHit(ICollidable obj, ICollidable by)
        {
            Bullet b = by as Bullet;

            if (b == null)
                return;

            b.DoHit((ILivingObject) obj);
        }


        public void DoObjectOutOfBounds(ICollidable obj)
        {
            Bullet b = obj as Bullet;

            if (b == null)
                return;

            b.OutOfBounds = true;
        }


        private void NotifyObjectDestroyed(Bullet bullet)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(bullet);
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                Bullets.Add(((PowerUpPulse) powerUp).Bullet);
            else if (powerUp.Type == PowerUpType.Shield)
                Bullets.Add(((PowerUpShield) powerUp).Bullet);
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                ((PowerUpPulse) powerUp).Bullet.LifePoints = 0;
            else if (powerUp.Type == PowerUpType.Shield)
                ((PowerUpShield) powerUp).Bullet.LifePoints = 0;
        }


        public void DoBulletDeflected(Enemy enemy, Bullet bullet)
        {
            Vector3 direction = enemy.Position - bullet.Position;

            float rotation = Math.Max(0.8f, 0.08f * bullet.Speed) * (1 - direction.Length() / bullet.DeflectZone);

            rotation = Main.Random.Next(0, 2) == 0 ? rotation : -rotation;

            Matrix.CreateRotationZ(rotation, out RotationMatrix);

            bullet.Direction = Vector3.Transform(bullet.Direction, RotationMatrix);
            bullet.Direction.Normalize();

            bullet.Image.Rotation = MathHelper.PiOver2 + (float) Math.Atan2(bullet.Direction.Y, bullet.Direction.X);

            if (bullet is MissileBullet)
                ((MissileBullet) bullet).Wander = true;
        }
    }
}
