namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class BoosterTurret : Turret
    {
        private Particle Glow;

        public BoosterTurret( Simulator simulator )
            : base( simulator )
        {
            Type = TurretType.Booster;
            Name = @"Booster";
            Description = @"Boost other turrets in it's range";
            Color = new Color( 71, 190, 255 );

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Update()
        {
            base.Update();

            if (Glow == null)
            {
                Glow = Simulator.Scene.Particles.Get( "boosterTurret" );
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

            CircleEmitter emitter = (CircleEmitter) Glow.Model[0];

            emitter.Radius = this.Range;
            emitter.Term = this.Range / 300f;
            emitter.ReleaseColour = this.Color.ToVector3();
        }
    }
}
