namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;
    using System;
    using EphemereGames.Core.Visuel;
    using ProjectMercury.Emitters;


    class BoosterTurret : Turret
    {
        private ParticuleEffectWrapper Glow;

        public BoosterTurret( Simulation simulation )
            : base( simulation )
        {
            Type = TurretType.Booster;
            Name = "Booster";
            Color = new Color( 71, 190, 255 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, new Cercle(Vector3.Zero, 50), Int16.MaxValue, 1, 0, BulletType.Aucun, "", "", 0, null, 0),
                new TurretLevel(1, 200, 500, new Cercle(Vector3.Zero, 50), Int16.MaxValue, 1, 500, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, null, 0),
                new TurretLevel(2, 100, 750, new Cercle(Vector3.Zero, 75), Int16.MaxValue, 1, 500, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, null, 0),
                new TurretLevel(2, 100, 750, new Cercle(Vector3.Zero, 125), Int16.MaxValue, 1, 500, BulletType.Aucun, "PixelBlanc", "tourelleBooster", 0, null, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if (Glow == null)
            {
                Glow = Simulation.Scene.Particules.recuperer( "boosterTurret" );
                setGlow();
            }

            Vector3 pos = this.Position;
            Glow.Emettre( ref pos );
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

            emitter.Radius = this.Range.Radius;
            emitter.Term = this.Range.Radius / 300f;
            emitter.ReleaseColour = this.Color.ToVector3();
        }
    }
}
