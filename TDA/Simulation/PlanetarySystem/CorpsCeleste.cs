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
        public List<Turret> Turrets = new List<Turret>();
        public IVisible Representation;
        public ParticuleEffectWrapper ParticulesRepresentation;
        public int Priorite;
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
        public bool Selectionnable;
        public bool Invincible;
        public bool EstDernierRelais;
        public Vector3 Offset;
        public List<Lune> Lunes;
        public bool ContientTourelleGravitationnelleByPass;
        public float PrioriteAffichageBackup;
        public Cercle TurretsZone;
        public bool ShowTurretsZone;
        public Cercle ZoneImpactDestruction;

        protected Simulation Simulation;
        protected Vector3 AnciennePosition;
        protected double TempsRotation;
        protected double TempsRotationActuel;
        protected Vector3 PositionBase;

        private ParticuleEffectWrapper toucherTerre;
        private ParticuleEffectWrapper bouleMeurt;
        private ParticuleEffectWrapper anneauMeurt;
        private Matrix MatriceRotation;
        private Image TurretsZoneImage;


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
            Simulation = simulation;
            Nom = nom;
            PointsVie = float.MaxValue;
            Priorite = 0;
            Selectionnable = true;
            Invincible = false;
            EstDernierRelais = false;

            Representation = representation;
            Representation.Taille = 6;
            Representation.Origine = representation.Centre;

            if (enBackground)
            {
                Representation.VisualPriority = Preferences.PrioriteFondEcran - 0.07f;
                Representation.Couleur.A = 60;
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

            Forme = Forme.Cercle;
            Cercle = new Cercle(Position, rayon);
            TurretsZone = new Cercle(Position, rayon * 2);

            initLunes();

            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = Color.White;
            TurretsZoneImage.Color.A = 100;
            TurretsZoneImage.VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f;
            ShowTurretsZone = false;

            ZoneImpactDestruction = new Cercle(this, 1);
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

            this.Forme = Forme.Cercle;
            this.Cercle = new Cercle(Position, rayon);
            TurretsZone = new Cercle(Position, rayon * 2);

            this.PointsAttaque = 50000;

            Lunes = new List<Lune>();

            ZoneImpactDestruction = new Cercle(this, 1);
        }


        public float PrioriteAffichage
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


        public override void Update(GameTime gameTime)
        {
            if (TempsRotation != 0)
            {
                TempsRotationActuel = (TempsRotationActuel + gameTime.ElapsedGameTime.TotalMilliseconds) % TempsRotation;

                deplacer();
            }

            Cercle.Position = Position;
            //ZoneImpactDestruction.Position = Position;
            TurretsZone.Position = Position;

            if (ParticulesRepresentation != null)
            {
                Vector3 deplacement;
                Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);
                ParticulesRepresentation.Deplacer(ref deplacement);
                ParticulesRepresentation.Emettre(ref position);
            }

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (this.PointsVie <= 0)
                return;

            if (Representation != null)
            {
                Representation.position = this.Position;
                Simulation.Scene.ajouterScenable(Representation);
            }

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Draw(gameTime);

            if (ShowTurretsZone)
            {
                TurretsZoneImage.Position = TurretsZone.Position;
                TurretsZoneImage.SizeX = (TurretsZone.Radius / 100) * 2;
                Simulation.Scene.ajouterScenable(TurretsZoneImage);
            }
        }


        public void doTouche(IObjetVivant par)
        {
            toucherTerre = Simulation.Scene.Particules.recuperer("toucherTerre");

            if (this.Representation != null)
                toucherTerre.VisualPriority = this.Representation.VisualPriority - 0.001f;
            else
                toucherTerre.VisualPriority = this.ParticulesRepresentation.VisualPriority - 0.001f;

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

            bouleMeurt.Emettre(ref this.position);
            anneauMeurt.Emettre(ref this.position);
            Simulation.Scene.Particules.retourner(bouleMeurt);
            Simulation.Scene.Particules.retourner(anneauMeurt);
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

                lune.Representation.Couleur.A = 50;

                Lunes.Add(lune);
            }
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
    }
}
