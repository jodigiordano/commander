namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class CelestialBody : ILivingObject, IObjetPhysique, IComparable<CelestialBody>
    {
        public string Name;
        public List<Turret> Turrets = new List<Turret>();
        public Image Representation;
        public Particle ParticulesRepresentation;
        public int Priorite;
        public Vector3 position;
        public Vector3 Position                                 { get { return position; } set { this.AnciennePosition = position; position = value; } }
        public float Speed                                      { get; set; }
        public float Rotation                                   { get; set; }
        public Vector3 Direction                                { get; set; }
        public Shape Shape                                      { get; set; }
        public Circle Circle                                    { get; set; }
        public Line Line                                       { get; set; }
        public RectanglePhysique Rectangle                      { get; set; }
        public float LifePoints                                 { get; set; }
        public float AttackPoints                               { get; set; }
        public bool Alive                                       { get { return LifePoints > 0; } }
        public bool Selectionnable;
        public bool Invincible;
        public bool LastOnPath;
        public bool FirstOnPath;
        public Vector3 Offset;
        public List<Moon> Lunes;
        public bool ContientTourelleGravitationnelleByPass;
        public double PrioriteAffichageBackup;
        public Circle InnerTurretZone;
        public Circle OuterTurretZone;
        public bool ShowTurretsZone;
        public float ZoneImpactDestruction;
        public bool DarkSide;

        protected Simulator Simulation;
        protected Vector3 AnciennePosition;
        protected double RotationTime;
        protected double ActualRotationTime;
        protected Vector3 PositionBase;

        private Particle DieEffect1;
        private Particle DieEffect2;
        private Particle DarkSideEffect;
        private Matrix RotationMatrix;
        private Image TurretsZoneImage;


        public CelestialBody(
            Simulator simulator,
            string nom,
            Vector3 positionBase,
            Vector3 offset,
            float rayon,
            double tempsRotation,
            Image representation,
            int pourcDepart,
            float prioriteAffichage,
            bool enBackground,
            int rotation)
        {
            Simulation = simulator;
            Name = nom;
            LifePoints = float.MaxValue;
            Priorite = 0;
            Selectionnable = true;
            Invincible = false;
            LastOnPath = false;
            FirstOnPath = false;
            DarkSide = false;

            Representation = representation;
            Representation.SizeX = 6;

            if (enBackground)
            {
                Representation.VisualPriority = Preferences.PrioriteFondEcran - 0.07f;
                Representation.Color.A = 60;
            }
            else
                Representation.VisualPriority = prioriteAffichage;

            PrioriteAffichageBackup = prioriteAffichage;

            Offset = offset;
            Position = AnciennePosition = PositionBase = positionBase;
            RotationTime = tempsRotation;
            ActualRotationTime = RotationTime * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out RotationMatrix);

            if (RotationTime != 0)
                Move();

            Shape = Shape.Circle;
            Circle = new Circle(Position, rayon);
            InnerTurretZone = new Circle(Position, 0);
            OuterTurretZone = new Circle(Position, 0);

            if (rayon <= (int) Size.Small)
            {
                InnerTurretZone.Radius = rayon;
                OuterTurretZone.Radius = rayon * 1.4f;
            }

            else if (rayon <= (int) Size.Normal)
            {
                InnerTurretZone.Radius = rayon * 0.8f;
                OuterTurretZone.Radius = rayon * 1.2f;
            }

            else
            {
                InnerTurretZone.Radius = rayon * 0.5f;
                OuterTurretZone.Radius = rayon * 1.0f;
            }

            InitMoons();

            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = new Color(255, 255, 255, 100);
            TurretsZoneImage.VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f;
            ShowTurretsZone = false;

            ZoneImpactDestruction = 0;

            DarkSideEffect = Simulation.Scene.Particles.Get(@"darkSideEffect");
            DarkSideEffect.VisualPriority = VisualPriority + 0.0001f;
            ((CircleEmitter) DarkSideEffect.ParticleEffect[0]).Radius = Circle.Radius;
        }


        public CelestialBody(
            Simulator simulator,
            string nom,
            Vector3 positionBase,
            Vector3 offset,
            float rayon,
            double tempsRotation,
            Particle representation,
            int pourcDepart,
            float prioriteAffichage,
            bool enBackground,
            int rotation)
        {
            this.Simulation = simulator;
            this.Name = nom;
            this.LifePoints = float.MaxValue;
            this.Priorite = 0;
            this.Selectionnable = true;
            this.Invincible = false;
            this.LastOnPath = false;
            this.FirstOnPath = false;
            this.DarkSide = false;

            this.ParticulesRepresentation = representation;

            if (enBackground)
            {
                this.ParticulesRepresentation.VisualPriority = Preferences.PrioriteFondEcran - 0.07f;

                foreach (var emetteur in this.ParticulesRepresentation.ParticleEffect)
                {
                    emetteur.ReleaseOpacity.Value = 0.23f;

                    foreach (var modifieur in emetteur.Modifiers)
                    {
                        if (modifieur is OpacityModifier)
                        {
                            OpacityModifier om = modifieur as OpacityModifier;
                            om.Initial = 0.23f;
                            om.Ultimate = 0f;
                        }
                    }
                }
            }
            else
                this.ParticulesRepresentation.VisualPriority = prioriteAffichage;

            PrioriteAffichageBackup = prioriteAffichage;

            this.Offset = offset;
            this.Position = this.AnciennePosition = this.PositionBase = positionBase;
            this.RotationTime = tempsRotation;
            this.ActualRotationTime = RotationTime * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out RotationMatrix);

            if (RotationTime != 0)
                Move();

            this.Shape = Shape.Circle;
            this.Circle = new Circle(Position, rayon);
            InnerTurretZone = new Circle(Position, rayon * (rayon < (int) Size.Normal ? 1.0f : 0.8f));
            OuterTurretZone = new Circle(Position, rayon * 1.3f);

            this.AttackPoints = 50000;

            Lunes = new List<Moon>();

            ZoneImpactDestruction = 0;
        }


        public double VisualPriority
        {
            get
            {
                if (this.Representation != null)
                    return this.Representation.VisualPriority;

                else
                    return this.ParticulesRepresentation.VisualPriority;
            }

            set
            {
                PrioriteAffichageBackup = (this.Representation != null) ? this.Representation.VisualPriority : this.ParticulesRepresentation.VisualPriority;

                if (this.Representation != null)
                    this.Representation.VisualPriority = value;

                if (this.ParticulesRepresentation != null)
                    this.ParticulesRepresentation.VisualPriority = value;

                for (int i = 0; i < Turrets.Count; i++)
                    Turrets[i].VisualPriority = value;
            }
        }


        public bool ContientTourelleGravitationnelle
        {
            get
            {
                if (ContientTourelleGravitationnelleByPass)
                    return true;

                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational)
                        return true;

                return false;
            }
        }


        public bool ContientTourelleGravitationnelleNiveau2
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational && Turrets[i].Level >= 2)
                        return true;

                return false;
            }
        }


        public virtual void Update()
        {
            if (RotationTime != 0)
            {
                ActualRotationTime = (ActualRotationTime + Preferences.TargetElapsedTimeMs) % RotationTime;

                Move();
            }

            Circle.Position = Position;
            InnerTurretZone.Position = Position;
            OuterTurretZone.Position = Position;

            if (ParticulesRepresentation != null)
            {
                Vector3 deplacement;
                Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);
                ParticulesRepresentation.Move(ref deplacement);
                ParticulesRepresentation.Trigger(ref position);
            }

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Update();
        }


        //public virtual void Show()
        //{
        //    if (Representation != null)
        //        Simulation.Scene.Add(Representation);

        //    foreach (var lune in Lunes)
        //        lune.Show();

        //    Simulation.Scene.Add(TurretsZoneImage);
        //}


        //public virtual void Hide()
        //{

        //    if (Representation != null)
        //        Simulation.Scene.Remove(Representation);

        //    foreach (var lune in Lunes)
        //        lune.Hide();

        //    Simulation.Scene.Remove(TurretsZoneImage);
        //}


        public virtual void Draw()
        {
            if (this.LifePoints <= 0)
                return;

            if (Representation != null)
            {
                Representation.position = this.Position;
                Simulation.Scene.Add(Representation);
            }

            foreach (var lune in Lunes)
                lune.Draw();

            if (ShowTurretsZone)
            {
                TurretsZoneImage.Position = OuterTurretZone.Position;
                TurretsZoneImage.SizeX = (OuterTurretZone.Radius / 100) * 2;

                Simulation.Scene.Add(TurretsZoneImage);
            }

            if (DarkSide)
            {
                DarkSideEffect.Trigger(ref position);
            }
        }


        public void DoHit(ILivingObject par)
        {
            Particle toucherTerre = Simulation.Scene.Particles.Get(@"toucherTerre");

            if (toucherTerre != null)
            {
                if (this.Representation != null)
                    toucherTerre.VisualPriority = this.Representation.VisualPriority - 0.001f;
                else
                    toucherTerre.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;

                toucherTerre.Trigger(ref this.position);
                Simulation.Scene.Particles.Return(toucherTerre);
            }

            if (Invincible)
                return;

            this.LifePoints -= par.AttackPoints;
        }


        public void DoDie()
        {
            if (Invincible)
                return;

            this.LifePoints = Math.Min(this.LifePoints, 0);

            DieEffect1 = Simulation.Scene.Particles.Get(@"bouleTerreMeurt");
            DieEffect2 = Simulation.Scene.Particles.Get(@"anneauTerreMeurt");

            if (this.Representation != null)
            {
                DieEffect1.VisualPriority = this.Representation.VisualPriority - 0.001f;
                DieEffect2.VisualPriority = this.Representation.VisualPriority - 0.001f;
            }
            else
            {
                DieEffect1.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;
                DieEffect2.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;
            }

            DieEffect1.Trigger(ref this.position);
            DieEffect2.Trigger(ref this.position);
            Simulation.Scene.Particles.Return(DieEffect1);
            Simulation.Scene.Particles.Return(DieEffect2);
            Simulation.Scene.Particles.Return(DarkSideEffect);
        }


        public int CompareTo(CelestialBody other)
        {
            if (Priorite > other.Priorite)
                return 1;

            if (Priorite < other.Priorite)
                return -1;

            return 0;
        }


        private void InitMoons()
        {
            Lunes = new List<Moon>();
            int nbLunes = Main.Random.Next(0, 3);

            for (int i = 0; i < nbLunes; i++)
            {
                Moon lune;

                if ((Main.Random.Next(0, 2) == 0))
                    lune = new MoonMatrix(Simulation, this, 50);
                else
                    lune = new MoonPath(Simulation, this, 50);

                Lunes.Add(lune);
            }
        }


        private void Move()
        {
            this.AnciennePosition = position;

            this.position.X = this.PositionBase.X * (float) Math.Cos((MathHelper.TwoPi / RotationTime) * ActualRotationTime);
            this.position.Y = this.PositionBase.Y * (float) Math.Sin((MathHelper.TwoPi / RotationTime) * ActualRotationTime);

            Vector3.Transform(ref position, ref RotationMatrix, out position);
            Vector3.Add(ref this.position, ref this.Offset, out this.position);
        }


        public static void Move(double tempsRotation, double tempsRotationActuel, ref Vector3 positionBase, ref Vector3 offset, ref Vector3 resultat)
        {
            resultat.X = positionBase.X * (float) Math.Cos((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Y = positionBase.Y * (float) Math.Sin((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Z = 0;

            Vector3.Add(ref resultat, ref offset, out resultat);
        }
    }
}
