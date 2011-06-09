namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;
    using System;


    class BulletsController
    {
        public event PhysicalObjectHandler ObjectDestroyed;
        public List<Bullet> Bullets { get; private set; }

        private Simulation Simulation;
        private Matrix RotationMatrix;


        public BulletsController(Simulation simulation)
        {
            Simulation = simulation;

            Bullets = new List<Bullet>();

            RotationMatrix = new Matrix();
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
                    

                    Simulation.BulletsFactory.Return(b);

                    Bullets.RemoveAt(i);
                }

            for (int i = 0; i < Bullets.Count; i++)
                Bullets[i].Update();
        }


        public void Draw()
        {
            for (int i = 0; i < Bullets.Count; i++)
                Bullets[i].Draw();
        }


        public void DoObjectCreated(IObjetPhysique objet)
        {
            Bullet b = objet as Bullet;

            if (b == null)
                return;

            b.Initialize();

            Bullets.Add(b);
        }


        public void DoObjectHit(IObjetPhysique obj, IObjetPhysique by)
        {
            Bullet b = by as Bullet;

            if (b == null)
                return;

            b.DoHit((ILivingObject) obj);
        }


        public void DoObjectOutOfBounds(IObjetPhysique obj)
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


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                Bullets.Add(((PowerUpPulse) powerUp).Bullet);
            else if (powerUp.Type == PowerUpType.Shield)
                Bullets.Add(((PowerUpShield) powerUp).Bullet);
        }


        public void DoPowerUpStopped(PowerUp powerUp)
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
