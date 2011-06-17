namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physics;
    using System;
    using EphemereGames.Core.Visual;
    using ProjectMercury.Emitters;


    class BoosterTurret : Turret
    {
        private Particle Glow;

        public BoosterTurret( Simulator simulation )
            : base( simulation )
        {
            Type = TurretType.Booster;
            Name = "Booster";
            Color = new Color( 71, 190, 255 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 50, Int16.MaxValue, 1, 0, BulletType.Aucun, "", "", 0, 0, 0),
                new TurretLevel(1, 400, 200, 75, Int16.MaxValue, 1, 5000, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, 0, 0),
                new TurretLevel(2, 600, 500, 125, Int16.MaxValue, 1, 10000, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, 0, 0),
                new TurretLevel(3, 800, 1000, 175, Int16.MaxValue, 1, 20000, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, 0, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Update()
        {
            base.Update();

            if (Glow == null)
            {
                Glow = Simulation.Scene.Particles.Get( "boosterTurret" );
                setGlow();
            }

            Vector3 pos = this.Position;
            Glow.Trigger( ref pos );
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                if (Glow != null)
                    setGlow();

                return true;
            }

            return false;
        }


        private void setGlow()
        {
            Glow.VisualPriority = this.VisualPriorityBackup + 0.006f;

            CircleEmitter emitter = (CircleEmitter) Glow.ParticleEffect[0];

            emitter.Radius = this.Range;
            emitter.Term = this.Range / 300f;
            emitter.ReleaseColour = this.Color.ToVector3();
        }
    }
}
