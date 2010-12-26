namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using ProjectMercury.Modifiers;

    class CorpsCeleste : DrawableGameComponent, IObjetVivant, IObjetPhysique
    {
        public String Nom;
        public List<Emplacement> Emplacements = new List<Emplacement>();
        protected Simulation Simulation;
        public IVisible representation;
        public ParticuleEffectWrapper representationParticules;
        private ParticuleEffectWrapper toucherTerre;
        private ParticuleEffectWrapper bouleMeurt;
        private ParticuleEffectWrapper anneauMeurt;
        public int Priorite;
        protected Vector3 AnciennePosition;

        public Vector3 position;
        public Vector3 Position                                     { get { return position; } set { this.AnciennePosition = position; position = value; } }

        public float Vitesse                                        { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Forme Forme                                          { get; set; }
        public Cercle Cercle                                        { get; set; }
        public Ligne Ligne                                          { get; set; }
        public RectanglePhysique Rectangle                          { get; set; }

        public float PointsVie                                      { get; set; }
        public float PointsAttaque                                  { get; set; }
        public bool EstVivant                                       { get { return PointsVie > 0; } }

        public bool PeutAvoirCollecteur;
        public bool PeutAvoirDoItYourself;
        public bool PeutAvoirTheResistance;
        public bool PeutDetruire;
        public bool Selectionnable;
        public bool Invincible;
        public bool EstDernierRelais;

        public int PrixDestruction;
        public int PrixCollecteur;
        public int PrixDoItYourself;
        public int PrixTheResistance;
        public Cercle ZoneImpactDestruction;

        public Vector3 Offset;

        public Vector3[] SousPoints;

        public List<Tourelle> TourellesPermises;

        public List<Lune> Lunes;

        public float PrioriteAffichage
        {
            get
            {
                if (this.representation != null)
                    return this.representation.VisualPriority;

                else
                    return this.representationParticules.VisualPriority;
            }

            set
            {
                PrioriteAffichageBackup = (this.representation != null) ? this.representation.VisualPriority : this.representationParticules.VisualPriority;

                if (this.representation != null)
                    this.representation.VisualPriority = value;

                if (this.representationParticules != null)
                    this.representationParticules.VisualPriority = value;

                for (int i = 0; i < Emplacements.Count; i++)
                    Emplacements[i].PrioriteAffichage = value;
            }
        }

        public float PrioriteAffichageBackup;

        protected double TempsRotation;
        protected double TempsRotationActuel;
        protected Vector3 PositionBase;

        private Matrix MatriceRotation;

        public CorpsCeleste(
            Simulation simulation,
            String nom,
            Vector3 positionBase,
            Vector3 offset,
            float rayon,
            double tempsRotation,
            IVisible representation,
            int pourcDepart,
            float prioriteAffichage,
            bool enBackground,
            int rotation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Nom = nom;
            this.PointsVie = float.MaxValue;
            this.Priorite = 0;
            this.Selectionnable = true;
            this.Invincible = false;
            this.EstDernierRelais = false;

            this.representation = representation;
            this.representation.Origine = representation.Centre;

            if (enBackground)
            {
                this.representation.VisualPriority = Preferences.PrioriteFondEcran - 0.07f;
                this.representation.Couleur.A = 60;
            }
            else
                this.representation.VisualPriority = prioriteAffichage;

            PrioriteAffichageBackup = prioriteAffichage;

            this.Offset = offset;
            this.Position = this.AnciennePosition = this.PositionBase = positionBase;
            this.TempsRotation = tempsRotation;
            this.TempsRotationActuel = TempsRotation * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out MatriceRotation);

            if (TempsRotation != 0)
                deplacer();

            this.Forme = Forme.Cercle;
            this.Cercle = new Cercle(Position, rayon);

            this.SousPoints = new Vector3[4];

            this.TourellesPermises = new List<Tourelle>();
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Base));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Gravitationnelle));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.LaserMultiple));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.LaserSimple));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Missile));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.SlowMotion));

            this.PeutAvoirDoItYourself = true;
            this.PeutDetruire = true;
            this.PrixDestruction = 500;
            this.PrixCollecteur = 0;
            this.PrixDoItYourself = 50;
            this.ZoneImpactDestruction = new Cercle(this.Position, 300);
            this.PointsAttaque = 50000;
            this.PeutAvoirCollecteur = true;
            this.PeutAvoirTheResistance = true;
            this.PrixTheResistance = 250;

            initLunes();
        }

        private void initLunes()
        {
            Lunes = new List<Lune>();
            int nbLunes = Main.Random.Next(0, 3);

            for (int i = 0; i < nbLunes; i++)
            {
                Lune lune;

                if ((Main.Random.Next(0, 2) == 0))
                    lune = new LuneMatrice(Simulation, this);
                else
                    lune = new LuneTrajet(Simulation, this);

                Lunes.Add(lune);
            }
        }


        public CorpsCeleste(
            Simulation simulation,
            String nom,
            Vector3 positionBase,
            Vector3 offset,
            float rayon,
            double tempsRotation,
            ParticuleEffectWrapper representation,
            int pourcDepart,
            float prioriteAffichage,
            bool enBackground,
            int rotation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Nom = nom;
            this.PointsVie = float.MaxValue;
            this.Priorite = 0;
            this.Selectionnable = true;
            this.Invincible = false;
            this.EstDernierRelais = false;

            this.representationParticules = representation;

            if (enBackground)
            {
                this.representationParticules.VisualPriority = Preferences.PrioriteFondEcran - 0.07f;

                foreach (var emetteur in this.representationParticules.ParticleEffect)
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
                this.representationParticules.VisualPriority = prioriteAffichage;

            PrioriteAffichageBackup = prioriteAffichage;

            this.Offset = offset;
            this.Position = this.AnciennePosition = this.PositionBase = positionBase;
            this.TempsRotation = tempsRotation;
            this.TempsRotationActuel = TempsRotation * (pourcDepart / 100.0f);

            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation), out MatriceRotation);

            if (TempsRotation != 0)
                deplacer();

            this.Forme = Forme.Cercle;
            this.Cercle = new Cercle(Position, rayon);

            this.SousPoints = new Vector3[4];

            this.TourellesPermises = new List<Tourelle>();
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Base));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Gravitationnelle));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.LaserMultiple));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.LaserSimple));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.Missile));
            this.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, TypeTourelle.SlowMotion));

            this.PeutAvoirDoItYourself = true;
            this.PeutDetruire = true;
            this.PrixDestruction = 500;
            this.PrixCollecteur = 0;
            this.PrixDoItYourself = 50;
            this.ZoneImpactDestruction = new Cercle(this.Position, 300);
            this.PointsAttaque = 50000;
            this.PeutAvoirCollecteur = true;
            this.PeutAvoirTheResistance = true;
            this.PrixTheResistance = 250;

            Lunes = new List<Lune>();
        }

        public bool ContientTourelleGravitationnelleByPass;
        public bool ContientTourelleGravitationnelle
        {
            get
            {
                if (ContientTourelleGravitationnelleByPass)
                    return true;

                for (int i = 0; i < Emplacements.Count; i++)
                    if (Emplacements[i].EstOccupe && Emplacements[i].Tourelle.Type == TypeTourelle.Gravitationnelle)
                        return true;

                return false;
            }
        }


        public bool ContientTourelleGravitationnelleNiveau2
        {
            get
            {
                for (int i = 0; i < Emplacements.Count; i++)
                    if (Emplacements[i].EstOccupe && Emplacements[i].Tourelle.Type == TypeTourelle.Gravitationnelle && Emplacements[i].Tourelle.Niveau >= 2)
                        return true;

                return false;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (TempsRotation != 0)
            {
                TempsRotationActuel = (TempsRotationActuel + gameTime.ElapsedGameTime.TotalMilliseconds) % TempsRotation;

                deplacer();
            }

            this.Cercle.Position = this.Position;
            this.ZoneImpactDestruction.Position = this.Position;

            if (this.representationParticules != null)
            {
                Vector3 deplacement;
                Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);
                this.representationParticules.Deplacer(ref deplacement);
                this.representationParticules.Emettre(ref this.position);
            }

            for (int i = 0; i < Emplacements.Count; i++)
                Emplacements[i].Update(gameTime);

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Update(gameTime);
        }

        private void deplacer()
        {
            this.AnciennePosition = position;

            this.position.X = this.PositionBase.X * (float)Math.Cos((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);
            this.position.Y = this.PositionBase.Y * (float)Math.Sin((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);

            Vector3.Transform(ref position, ref MatriceRotation, out position);
            Vector3.Add(ref this.position, ref this.Offset, out this.position);
        }

        public static void Deplacer(double tempsRotation, double tempsRotationActuel, ref Vector3 positionBase, ref Vector3 offset, ref Vector3 resultat)
        {

            resultat.X = positionBase.X * (float)Math.Cos((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Y = positionBase.Y * (float)Math.Sin((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Z = 0;

            Vector3.Add(ref resultat, ref offset, out resultat);
        }


        public override void Draw(GameTime gameTime)
        {
            if (this.PointsVie <= 0)
                return;

            if (representation != null)
            {
                representation.position = this.Position;
                Simulation.Scene.ajouterScenable(representation);
            }

            for (int i = 0; i < Emplacements.Count; i++)
                Emplacements[i].Draw(gameTime);

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Draw(gameTime);
        }


        public void doTouche(IObjetVivant par)
        {
            toucherTerre = Simulation.Scene.Particules.recuperer("toucherTerre");

            if (this.representation != null)
                toucherTerre.VisualPriority = this.representation.VisualPriority - 0.001f;
            else
                toucherTerre.VisualPriority = this.representationParticules.VisualPriority - 0.001f;

            toucherTerre.Emettre(ref this.position);
            Simulation.Scene.Particules.retourner(toucherTerre);

            if (Invincible)
                return;

            this.PointsVie -= par.PointsAttaque;
        }


        public void doMeurt()
        {
            if (Invincible)
                return;

            this.PointsVie = Math.Min(this.PointsVie, 0);

            bouleMeurt = Simulation.Scene.Particules.recuperer("bouleTerreMeurt");
            anneauMeurt = Simulation.Scene.Particules.recuperer("anneauTerreMeurt");

            if (this.representation != null)
            {
                bouleMeurt.VisualPriority = this.representation.VisualPriority - 0.001f;
                anneauMeurt.VisualPriority = this.representation.VisualPriority - 0.001f;
            }
            else
            {
                bouleMeurt.VisualPriority = this.representationParticules.VisualPriority - 0.001f;
                anneauMeurt.VisualPriority = this.representationParticules.VisualPriority - 0.001f;
            }

            bouleMeurt.Emettre(ref this.position);
            anneauMeurt.Emettre(ref this.position);
            Simulation.Scene.Particules.retourner(bouleMeurt);
            Simulation.Scene.Particules.retourner(anneauMeurt);
        }
    }
}
