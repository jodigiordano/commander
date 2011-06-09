namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class CorpsCeleste : ILivingObject, IObjetPhysique
    {
        public String Nom;
        public List<Turret> Turrets = new List<Turret>();
        public Image Representation;
        public Particle ParticulesRepresentation;
        public int Priorite;
        public Vector3 position;
        public Vector3 Position                                     { get { return position; } set { this.AnciennePosition = position; position = value; } }
        public float Speed                                        { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Shape Shape                                          { get; set; }
        public Cercle Circle                                        { get; set; }
        public Ligne Line                                          { get; set; }
        public RectanglePhysique Rectangle                          { get; set; }
        public float LifePoints                                      { get; set; }
        public float AttackPoints                                  { get; set; }
        public bool Alive                                       { get { return LifePoints > 0; } }
        public bool Selectionnable;
        public bool Invincible;
        public bool EstDernierRelais;
        public Vector3 Offset;
        public List<Lune> Lunes;
        public bool ContientTourelleGravitationnelleByPass;
        public double PrioriteAffichageBackup;
        public Cercle TurretsZone;
        public bool ShowTurretsZone;
        public float ZoneImpactDestruction;

        protected Simulation Simulation;
        protected Vector3 AnciennePosition;
        protected double TempsRotation;
        protected double TempsRotationActuel;
        protected Vector3 PositionBase;

        private Particle toucherTerre;
        private Particle bouleMeurt;
        private Particle anneauMeurt;
        private Matrix MatriceRotation;
        private Image TurretsZoneImage;


        public CorpsCeleste(
            Simulation simulation,
            String nom,
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
            Simulation = simulation;
            Nom = nom;
            LifePoints = float.MaxValue;
            Priorite = 0;
            Selectionnable = true;
            Invincible = false;
            EstDernierRelais = false;

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
            TempsRotation = tempsRotation;
            TempsRotationActuel = TempsRotation * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out MatriceRotation);

            if (TempsRotation != 0)
                deplacer();

            Shape = Shape.Circle;
            Circle = new Cercle(Position, rayon);
            TurretsZone = new Cercle(Position, rayon * 2);

            initLunes();

            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = new Color(255, 255, 255, 100);
            TurretsZoneImage.VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f;
            ShowTurretsZone = false;

            ZoneImpactDestruction = 0;
        }


        public CorpsCeleste(
            Simulation simulation,
            String nom,
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
            this.Simulation = simulation;
            this.Nom = nom;
            this.LifePoints = float.MaxValue;
            this.Priorite = 0;
            this.Selectionnable = true;
            this.Invincible = false;
            this.EstDernierRelais = false;

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
            this.TempsRotation = tempsRotation;
            this.TempsRotationActuel = TempsRotation * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out MatriceRotation);

            if (TempsRotation != 0)
                deplacer();

            this.Shape = Shape.Circle;
            this.Circle = new Cercle(Position, rayon);
            TurretsZone = new Cercle(Position, rayon * 2);

            this.AttackPoints = 50000;

            Lunes = new List<Lune>();

            ZoneImpactDestruction = 0;
        }


        public double PrioriteAffichage
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


        public virtual void Update(GameTime gameTime)
        {
            if (TempsRotation != 0)
            {
                TempsRotationActuel = (TempsRotationActuel + gameTime.ElapsedGameTime.TotalMilliseconds) % TempsRotation;

                deplacer();
            }

            Circle.Position = Position;
            TurretsZone.Position = Position;

            if (ParticulesRepresentation != null)
            {
                Vector3 deplacement;
                Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);
                ParticulesRepresentation.Move(ref deplacement);
                ParticulesRepresentation.Trigger(ref position);
            }

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Update(gameTime);
        }


        public virtual void Show()
        {
            if (Representation != null)
                Simulation.Scene.Add(Representation);

            foreach (var lune in Lunes)
                lune.Show();

            Simulation.Scene.Add(TurretsZoneImage);
        }


        public virtual void Hide()
        {

            if (Representation != null)
                Simulation.Scene.Remove(Representation);

            foreach (var lune in Lunes)
                lune.Hide();

            Simulation.Scene.Remove(TurretsZoneImage);
        }


        public virtual void Draw()
        {
            if (this.LifePoints <= 0)
                return;

            if (Representation != null)
                Representation.position = this.Position;

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Draw();

            if (ShowTurretsZone)
            {
                TurretsZoneImage.Position = TurretsZone.Position;
                TurretsZoneImage.SizeX = (TurretsZone.Radius / 100) * 2;
                TurretsZoneImage.Color.A = 100;
            }

            else
            {
                TurretsZoneImage.Color.A = 0;
            }
        }


        public void DoHit(ILivingObject par)
        {
            toucherTerre = Simulation.Scene.Particles.Get("toucherTerre");

            if (this.Representation != null)
                toucherTerre.VisualPriority = this.Representation.VisualPriority - 0.001f;
            else
                toucherTerre.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;

            toucherTerre.Trigger(ref this.position);
            Simulation.Scene.Particles.Return(toucherTerre);

            if (Invincible)
                return;

            this.LifePoints -= par.AttackPoints;
        }


        public void DoDie()
        {
            if (Invincible)
                return;

            this.LifePoints = Math.Min(this.LifePoints, 0);

            bouleMeurt = Simulation.Scene.Particles.Get("bouleTerreMeurt");
            anneauMeurt = Simulation.Scene.Particles.Get("anneauTerreMeurt");

            if (this.Representation != null)
            {
                bouleMeurt.VisualPriority = this.Representation.VisualPriority - 0.001f;
                anneauMeurt.VisualPriority = this.Representation.VisualPriority - 0.001f;
            }
            else
            {
                bouleMeurt.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;
                anneauMeurt.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;
            }

            bouleMeurt.Trigger(ref this.position);
            anneauMeurt.Trigger(ref this.position);
            Simulation.Scene.Particles.Return(bouleMeurt);
            Simulation.Scene.Particles.Return(anneauMeurt);
        }


        private void initLunes()
        {
            Lunes = new List<Lune>();
            int nbLunes = Main.Random.Next(0, 3);

            for (int i = 0; i < nbLunes; i++)
            {
                Lune lune;

                if ((Main.Random.Next(0, 2) == 0))
                    lune = new LuneMatrice(Simulation, this, 50);
                else
                    lune = new LuneTrajet(Simulation, this, 50);

                Lunes.Add(lune);
            }
        }


        private void deplacer()
        {
            this.AnciennePosition = position;

            this.position.X = this.PositionBase.X * (float) Math.Cos((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);
            this.position.Y = this.PositionBase.Y * (float) Math.Sin((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);

            Vector3.Transform(ref position, ref MatriceRotation, out position);
            Vector3.Add(ref this.position, ref this.Offset, out this.position);
        }


        public static void Deplacer(double tempsRotation, double tempsRotationActuel, ref Vector3 positionBase, ref Vector3 offset, ref Vector3 resultat)
        {

            resultat.X = positionBase.X * (float) Math.Cos((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Y = positionBase.Y * (float) Math.Sin((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Z = 0;

            Vector3.Add(ref resultat, ref offset, out resultat);
        }
    }
}
