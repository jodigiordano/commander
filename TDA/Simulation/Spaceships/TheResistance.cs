namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class TheResistance : Spaceship
    {
        public double TempsActif;

        private List<Spaceship> Vaisseaux;
        private List<Ennemi> Ennemis;


        public TheResistance(Simulation simulation)
            : base(simulation)
        {
            Vaisseaux = new List<Spaceship>();

            Vaisseaux.Add(new Spaceship(simulation)
            {
                ShootingFrequency = 100,
                BulletHitPoints = 10,
                RotationMaximaleRad = 0.15f,
                Image = new Image("Resistance1")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            Vaisseaux.Add(new Spaceship(simulation)
            {
                ShootingFrequency = 200,
                BulletHitPoints = 30,
                RotationMaximaleRad = 0.05f,
                Image = new Image("Resistance2")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            Vaisseaux.Add(new Spaceship(simulation)
            {
                ShootingFrequency = 500,
                BulletHitPoints = 100,
                RotationMaximaleRad = 0.2f,
                Image = new Image("Resistance3")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            SfxGoHome = "sfxPowerUpResistanceOut";
            SfxIn = "sfxPowerUpResistanceIn";
        }


        public void Initialize(List<Ennemi> ennemis)
        {
            Ennemis = ennemis;

            foreach (var spaceship in Vaisseaux)
            {
                spaceship.StartingObject = StartingObject;

                if (StartingObject != null)
                    spaceship.Position = StartingObject.Position;
            }
        }


        public byte AlphaChannel
        {
            set
            {
                foreach (var spaceship in Vaisseaux)
                    spaceship.Image.Color.A = value;
            }
        }


        public override bool Active
        {
            get { return TempsActif > 0; }
        }


        public override bool TargetReached
        {
            get { return Vaisseaux[0].TargetReached && Vaisseaux[1].TargetReached && Vaisseaux[2].TargetReached; }
        }


        public override void Update()
        {
            TempsActif -= 16.66f;

            for (int i = 0; i < Vaisseaux.Count; i++)
            {
                if (!Vaisseaux[i].InCombat)
                {
                    Vector3 direction = ((Ennemis.Count > i) ? Ennemis[i].Position : StartingObject.Position) - Vaisseaux[i].Position;
                    direction.Normalize();

                    Vaisseaux[i].TargetPosition = ((Ennemis.Count > i) ? Ennemis[i].Position : StartingObject.Position) + direction * 100;
                }
            }

            foreach (var vaisseau in Vaisseaux)
            {

                vaisseau.DoAutomaticMode();
                vaisseau.Update();
            }
        }


        private List<Projectile> projectilesCeTick = new List<Projectile>();
        public override List<Projectile> BulletsThisTick()
        {
            projectilesCeTick.Clear();

            foreach (var vaisseau in Vaisseaux)
                projectilesCeTick.AddRange(vaisseau.BulletsThisTick());

            return projectilesCeTick;
        }


        public override void DoHide()
        {

            foreach (var vaisseau in Vaisseaux)
            {
                vaisseau.TargetPosition = StartingObject.Position;
                vaisseau.DoHide();
            }
        }


        public override void Draw()
        {
            foreach (var vaisseau in Vaisseaux)
                vaisseau.Draw();
        }
    }
}
