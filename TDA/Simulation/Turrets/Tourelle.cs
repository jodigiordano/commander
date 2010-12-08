namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    using ProjectMercury.Emitters;


    abstract class Tourelle : DrawableGameComponent, IObjetPhysique
    {
        public static List<List<int>> SourcesProjectiles = new List<List<int>>()
        {
            new List<int>(new int[] { 0 }),
            new List<int>(new int[] { -10, 10 }),
            new List<int>(new int[] { -10, 0, 10 }),
            new List<int>(new int[] { -20, -10, 10, 20 }),
            new List<int>(new int[] { -20, -10, 0, 10, 20 })
        };

        public static List<List<int>> SourcesProjectiles2 = new List<List<int>>()
        {
            new List<int>(new int[] { 0 }),
            new List<int>(new int[] { -5, 5 }),
            new List<int>(new int[] { -5, 0, 5 }),
            new List<int>(new int[] { -10, -5, 5, 10 }),
            new List<int>(new int[] { -10, -5, 0, 5, 10 })
        };

        private Vector3 AnciennePosition                                    { get; set; }
        private Vector3 position;
        public Vector3 Position                                             { get { return position; } set { this.AnciennePosition = position; position = value; } }
        public IVisible representation                                      { get; set; }
        private bool InactiveOverride;
        public bool Inactive                                                { get { return InactiveOverride && CompteurInactivite > 0; } set { InactiveOverride = value; } }
        public virtual Ennemi EnnemiAttaque                                 { get; set; }
        public double TempsDernierProjectileLance                           { get; set; }
        public TypeTourelle Type                                            { get; protected set; }
        public String Nom                                                   { get; protected set; }
        public bool PeutVendre                                              { get; set; }


        public bool RetourDeInactiveCeTick                                  { get; private set; }

        private bool PeutMettreAJourOverride;
        public bool PeutMettreAJour                                         { get { return PeutMettreAJourOverride && (!Inactive && !NiveauActuel.Equals(Niveaux.Last)); } set { PeutMettreAJourOverride = value; } }
        public int PrixAchat                                                { get { return NiveauActuel.Value.PrixAchat; } }
        public int PrixMiseAJour                                            { get { return (NiveauActuel.Equals(Niveaux.Last)) ? NiveauActuel.Value.PrixAchat : NiveauActuel.Next.Value.PrixAchat; } }
        public int PrixVente                                                { get { return NiveauActuel.Value.PrixVente; } }
        public Cercle ZoneActivation                                        { get { return NiveauActuel.Value.ZoneActivation; } }
        public double CadenceTir                                            { get { return NiveauActuel.Value.CadenceTir; } }
        public int NombreCanons                                             { get { return NiveauActuel.Value.NombreCanons; } }
        public double TempsConstruction                                     { get { return NiveauActuel.Value.TempsConstruction; } }
        public TypeProjectile ProjectileLance                               { get { return NiveauActuel.Value.ProjectileLance; } }
        public bool Spectateur;
        public Color Couleur;


        private float RotationWander = 0;

        private double TempsDerniereQuoteEnnuiLancee;

        public int Niveau
        {
            get { return NiveauActuel.Value.Niveau; }

            set
            {
                if (value <= NiveauActuel.Value.Niveau || value > 10)
                    return;

                PeutMettreAJour = true;
                InactiveOverride = false;

                for (int i = value - NiveauActuel.Value.Niveau; i > 0; i--)
                    miseAJour();

                CompteurInactivite = 0;
                CompteurAnnonciationActiveDeNouveau = float.NaN;
                InactiveOverride = true;
            }
        }

        private LinkedList<NiveauTourelle> niveaux;
        public LinkedList<NiveauTourelle> Niveaux                           { get { return niveaux; } set { niveaux = value; NiveauActuel = this.Niveaux.First; } }

        private LinkedListNode<NiveauTourelle> niveauActuel;
        protected LinkedListNode<NiveauTourelle> NiveauActuel
        {
            get { return niveauActuel; }
            set
            {
                niveauActuel = value;
                CompteurInactivite = (float) value.Value.TempsConstruction;
                CompteurAnnonciationActiveDeNouveau = (float)value.Value.TempsConstruction;
            }
        }

        private Simulation Simulation;
        protected Scene Scene;
        private float CompteurInactivite;
        private float CompteurAnnonciationActiveDeNouveau;
        public bool AnnonciationActiveDeNouveauOverride;
        private IVisible ProgressionBarreInactivite;
        private IVisible BarreInactivite;
        public IVisible representationBase;
        private float PrioriteAffichageBackup;

        protected String SfxTir;
        private List<Projectile> projectiles = new List<Projectile>();

        public Tourelle(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Scene = simulation.Scene;
            this.TempsDernierProjectileLance = 0;
            this.TempsDerniereQuoteEnnuiLancee = 0;
            this.Type = TypeTourelle.Inconnu;
            this.Nom = "Unknown";
            this.PeutVendre = true;
            this.CompteurInactivite = 0;
            this.PeutMettreAJourOverride = true;
            this.InactiveOverride = true;

            this.ProgressionBarreInactivite = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero);
            this.ProgressionBarreInactivite.TailleVecteur = new Vector2(40, 8);
            this.ProgressionBarreInactivite.Couleur = new Color(255, 0, 220, 255);

            this.BarreInactivite = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("BarreInactivite"), Vector3.Zero);
            this.BarreInactivite.Origine = this.BarreInactivite.Centre;

            this.Spectateur = true;

            this.PrioriteAffichageBackup = Preferences.PrioriteSimulationTourelle;

            this.CompteurAnnonciationActiveDeNouveau = float.NaN;
            this.AnnonciationActiveDeNouveauOverride = false;
            this.RetourDeInactiveCeTick = false;
        }

        public virtual float PrioriteAffichage
        {
            set
            {
                this.PrioriteAffichageBackup = value;

                representationBase.PrioriteAffichage = value - 0.002f;
                representation.PrioriteAffichage = value - 0.001f;

                BarreInactivite.PrioriteAffichage = value - 0.003f;
                ProgressionBarreInactivite.PrioriteAffichage = value - 0.004f;
            }
        }

        public void doMeurt() { }

        public override void Update(GameTime gameTime)
        {
            this.RetourDeInactiveCeTick = false;

            this.ZoneActivation.Position = this.position;

            this.CompteurInactivite = MathHelper.Clamp(CompteurInactivite - (float) gameTime.ElapsedGameTime.TotalMilliseconds, 0, float.MaxValue);
            this.CompteurAnnonciationActiveDeNouveau -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            TempsDernierProjectileLance -= gameTime.ElapsedGameTime.TotalMilliseconds;
            TempsDerniereQuoteEnnuiLancee += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Type != TypeTourelle.SlowMotion && Type != TypeTourelle.Gravitationnelle && Type != TypeTourelle.GravitationnelleAlien)
                doWanderRotation(gameTime);

            if (CompteurAnnonciationActiveDeNouveau < 0 && !Simulation.ModeDemo)
            {
                if (!this.AnnonciationActiveDeNouveauOverride)
                    Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMiseAJour");

                CompteurAnnonciationActiveDeNouveau = float.NaN;
                RetourDeInactiveCeTick = true;
            }
        }


        private void doWanderRotation(GameTime gameTime)
        {
            if (EnnemiAttaque == null)
            {
                if (RotationWander > 0)
                    RotationWander = Math.Max(0, RotationWander - 0.001f);
                else if (RotationWander < 0)
                    RotationWander = Math.Min(0, RotationWander + 0.001f);
                else
                    RotationWander = Main.Random.Next(-10, 11) / 100.0f;
            }
        }

        public List<Projectile> ProjectilesCeTick(GameTime gameTime)
        {
            projectiles.Clear();

            if (Inactive || EnnemiAttaque == null || Spectateur)
                return projectiles;

            if (TempsDernierProjectileLance <= 0)
            {
                Vector3 direction = EnnemiAttaque.Position - this.Position;
                Matrix matriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(direction, matriceRotation);
                directionUnitairePerpendiculaire.Normalize();
                
                switch (ProjectileLance)
                {
                    case TypeProjectile.Base:

                        for (int i = 0; i < NombreCanons; i++)
                        {
                            Vector3 translation = directionUnitairePerpendiculaire * SourcesProjectiles[NombreCanons - 1][i];

                            ProjectileBase p = Projectile.PoolProjectilesBase.recuperer();

                            p.Scene = Simulation.Scene;
                            p.Position = this.Position + translation;
                            p.Direction = direction;
                            p.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                            p.PrioriteAffichage = this.representation.PrioriteAffichage;
                            p.Initialize();

                            projectiles.Add(p);
                        }
                        break;

                    case TypeProjectile.Missile:
                        ProjectileMissile pm = Projectile.PoolProjectilesMissile.recuperer();
                        pm.Scene = Simulation.Scene;
                        pm.Position = this.Position;
                        pm.Direction = EnnemiAttaque.Position - this.Position;
                        pm.Cible = EnnemiAttaque;
                        pm.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                        pm.PrioriteAffichage = this.representation.PrioriteAffichage;
                        pm.Vitesse = NiveauActuel.Value.ProjectileVitesse;
                        pm.ZoneImpact = NiveauActuel.Value.ProjectileZoneImpact;
                        pm.Initialize();
                        pm.RepresentationVivant.Texture = Core.Persistance.Facade.recuperer<Texture2D>("ProjectileMissile1");
                        pm.RepresentationVivant.Taille = 1;
                        pm.Rectangle.Width = pm.RepresentationVivant.Rectangle.Width;
                        pm.Rectangle.Height = pm.RepresentationVivant.Rectangle.Height;
                        projectiles.Add(pm);
                        break;

                    case TypeProjectile.Missile2:
                        ProjectileMissile p2 = Projectile.PoolProjectilesMissile.recuperer();
                        p2.Scene = Simulation.Scene;
                        p2.Position = this.Position;
                        p2.Direction = EnnemiAttaque.Position - this.Position;
                        p2.Cible = EnnemiAttaque;
                        p2.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                        p2.PrioriteAffichage = this.representation.PrioriteAffichage;
                        p2.Vitesse = NiveauActuel.Value.ProjectileVitesse;
                        p2.ZoneImpact = NiveauActuel.Value.ProjectileZoneImpact;
                        p2.Initialize();
                        p2.RepresentationVivant.Texture = Core.Persistance.Facade.recuperer<Texture2D>("ProjectileMissile2");
                        p2.RepresentationVivant.Taille = 2;
                        p2.Rectangle.Width = p2.RepresentationVivant.Rectangle.Width;
                        p2.Rectangle.Height = p2.RepresentationVivant.Rectangle.Height;
                        projectiles.Add(p2);
                        break;

                    case TypeProjectile.LaserMultiple:
                        for (int i = 0; i < NombreCanons; i++)
                        {
                            ProjectileLaserMultiple pLM = Projectile.PoolProjectilesLaserMultiple.recuperer();
                            pLM.Scene = Simulation.Scene;
                            pLM.TourelleEmettrice = this;
                            pLM.CibleOffset = directionUnitairePerpendiculaire * SourcesProjectiles[NombreCanons - 1][i];
                            pLM.Cible = EnnemiAttaque;
                            pLM.Direction = EnnemiAttaque.Position - this.Position;
                            pLM.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                            pLM.PrioriteAffichage = this.representation.PrioriteAffichage;
                            pLM.Initialize();

                            projectiles.Add(pLM);
                        }
                        break;

                    case TypeProjectile.LaserSimple:
                        ProjectileLaserSimple pLS = Projectile.PoolProjectilesLaserSimple.recuperer();
                        pLS.Scene = Simulation.Scene;
                        pLS.TourelleEmettrice = this;
                        pLS.Cible = EnnemiAttaque;
                        pLS.Direction = EnnemiAttaque.Position - this.Position;
                        pLS.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                        pLS.PrioriteAffichage = this.representation.PrioriteAffichage;
                        pLS.Initialize();

                        ((TourelleLasersSimples)this).ProjectileEnCours = pLS;

                        projectiles.Add(pLS);
                        break;

                    case TypeProjectile.SlowMotion:
                        ProjectileSlowMotion pSM = Projectile.PoolProjectilesSlowMotion.recuperer();
                        pSM.Scene = Simulation.Scene;
                        pSM.Position = this.Position;
                        pSM.Rayon = NiveauActuel.Value.ZoneActivation.Rayon;
                        pSM.PointsAttaque = NiveauActuel.Value.ProjectilePointsAttaque;
                        pSM.PrioriteAffichage = this.representation.PrioriteAffichage;
                        pSM.Initialize();

                        projectiles.Add(pSM);
                        break;
                }

                TempsDernierProjectileLance = CadenceTir;
            }

            if (projectiles.Count != 0)
                Core.Audio.Facade.jouerEffetSonore("Partie", SfxTir);

            return projectiles;
        }

        public override void Draw(GameTime gameTime)
        {
            representation.position = this.position;
            representationBase.position = this.position;

            if (EnnemiAttaque != null)
            {
                Vector3 direction = EnnemiAttaque.Position - this.Position;

                representation.Rotation = MathHelper.PiOver2 + (float)Math.Atan2(direction.Y, direction.X);
            }

            else
                representation.Rotation += RotationWander;

            Scene.ajouterScenable(representation);
            Scene.ajouterScenable(representationBase);


            if (Inactive && !Simulation.ModeDemo)
            {
                BarreInactivite.Position = this.Position;

                float pourcTemps = (float)(CompteurInactivite / NiveauActuel.Value.TempsConstruction);

                ProgressionBarreInactivite.TailleVecteur = new Vector2(pourcTemps * 40, 8);
                ProgressionBarreInactivite.Position = BarreInactivite.Position - new Vector3(16, 4, 0);
                Scene.ajouterScenable(ProgressionBarreInactivite);
                Scene.ajouterScenable(BarreInactivite);
            }
        }


        public virtual bool miseAJour()
        {
            if (!PeutMettreAJour)
                return false;

            if (NiveauActuel.Value.RepresentationBase != NiveauActuel.Next.Value.RepresentationBase)
            {
                representationBase = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(NiveauActuel.Next.Value.RepresentationBase), Vector3.Zero);
                representationBase.Origine = representationBase.Centre;
            }

            if (NiveauActuel.Value.Representation != NiveauActuel.Next.Value.Representation)
            {
                representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(NiveauActuel.Next.Value.Representation), Vector3.Zero);
                representation.Origine = representation.Centre;
            }

            PrioriteAffichage = this.PrioriteAffichageBackup;

            NiveauActuel = NiveauActuel.Next;

            return true;
        }


        //useless
        #region IObjetPhysique Members
        public float Vitesse { get; set; }
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public Forme Forme { get; set; }
        public Cercle Cercle { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Ligne { get; set; }
        #endregion
    }
}
