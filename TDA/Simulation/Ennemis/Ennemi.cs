namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Persistance;
    using Core.Physique;

    class Ennemi : IObjetPhysique, IObjetVivant
    {
        private static int NEXT_ID = 0;
        public static int NextID { get { return NEXT_ID++; } }

        public delegate void RelaisAtteintHandler(Ennemi ennemi);
        public event RelaisAtteintHandler RelaisAtteint;

        public int Id;

        private Vector3 position;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Vitesse                                        { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Forme Forme                                          { get; set; }
        public Cercle Cercle                                        { get; set; }
        public Ligne Ligne                                          { get; set; }
        public RectanglePhysique Rectangle                          { get; set; }

        public float PointsVie                                      { get; set; }
        public float PointsVieDepart                                { get; set; }
        public float PointsAttaque                                  { get; set; }

        public int ValeurUnites;
        public int ValeurPoints;

        public float Resistance;

        public bool EstVivant                                      { get { return PointsVie > 0; } }
        public bool FinCheminProjection                            { get { return Deplacement > CheminProjection.Longueur; } }

        public IVisible RepresentationVivant;
        public IVisible RepresentationVivantProjection;
        public IVisible RepresentationMort;
        public IVisible RepresentationMortProjection;
        public ParticuleEffectWrapper RepresentationExplose;
        public ParticuleEffectWrapper RepresentationDeplacement;

        public float VitesseRotation;
        public Chemin Chemin;
        public Chemin CheminProjection;
        public double Deplacement;
        public Simulation Simulation;
        public String Nom;
        public TypeEnnemi Type;
        public List<Mineral> Mineraux;
        public Vector3 Translation;
        public Vector3 PositionDernierProjectileTouche;
        public Color Couleur;

        private ParticuleEffectWrapper EtincellesLaserMultiple, EtincellesMissile, EtincellesLaserSimple, EtincellesSlowMotion;


        public Ennemi()
        {
            Mineraux = new List<Mineral>();

            RepresentationDeplacement = null;

            PointsAttaque = 1;
            ValeurPoints = 1;

            Forme = Forme.Rectangle;
            Rectangle = new RectanglePhysique(0, 0, 1, 1);
            Cercle = new Cercle(Vector3.Zero, 1);
            Type = TypeEnnemi.Inconnu;
            Id = NextID;
        }


        public void Initialize()
        {
            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(Nom), Vector3.Zero, Simulation.Scene);
                RepresentationVivant.Origine = RepresentationVivant.Centre;

                RepresentationMort = RepresentationVivant;

                RepresentationVivantProjection = (IVisible)RepresentationVivant.Clone();
                RepresentationMortProjection = (IVisible)RepresentationMort.Clone();
            }

            RepresentationVivant.Scene = Simulation.Scene;
            RepresentationMort.Scene = Simulation.Scene;
            RepresentationVivantProjection.Scene = Simulation.Scene;
            RepresentationMortProjection.Scene = Simulation.Scene;

            EtincellesLaserMultiple = Simulation.Scene.Particules.recuperer("etincelleLaser");
            EtincellesMissile = Simulation.Scene.Particules.recuperer("etincelleMissile");
            EtincellesLaserSimple = Simulation.Scene.Particules.recuperer("etincelleLaserSimple");
            EtincellesSlowMotion = Simulation.Scene.Particules.recuperer("etincelleSlowMotionTouche");
            RepresentationExplose = Simulation.Scene.Particules.recuperer("explosionEnnemi");

            Resistance = 0;
            Deplacement = 0;

            Rectangle.Width = Rectangle.Height = FactoryEnnemis.Instance.getTaille(Type);
            Cercle.Rayon = Rectangle.Width / 2 - 3;

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Cercle.Position.X = Position.X - Cercle.Rayon;
            Cercle.Position.Y = Position.Y - Cercle.Rayon;

            EtincellesLaserMultiple.PrioriteAffichage = RepresentationVivant.PrioriteAffichage - 0.001f;
            EtincellesMissile.PrioriteAffichage = RepresentationVivant.PrioriteAffichage - 0.001f;
            EtincellesLaserSimple.PrioriteAffichage = RepresentationVivant.PrioriteAffichage - 0.001f;
            EtincellesSlowMotion.PrioriteAffichage = RepresentationVivant.PrioriteAffichage - 0.001f;

            ProjectMercury.VariableFloat3 float3 = new ProjectMercury.VariableFloat3();
            float3.Value = Couleur.ToVector3();

            RepresentationExplose.ParticleEffect[0].ReleaseColour = float3;
            RepresentationDeplacement = null;

            VitesseRotation = Main.Random.Next(-5, 6) / 100.0f;

            RepresentationVivant.PrioriteAffichage = Preferences.PrioriteSimulationEnnemi;
            RepresentationMort.PrioriteAffichage = Preferences.PrioriteSimulationEnnemi;
            RepresentationExplose.PrioriteAffichage = Preferences.PrioriteSimulationEnnemi - 0.001f;

            Mineraux.Clear();
        }


        public void Update(GameTime gameTime)
        {
            Resistance = Math.Max(Resistance - 0.02f, 0);

            Deplacement += Math.Max(this.Vitesse - this.Resistance, 0);

            Chemin.Position(Deplacement, ref position);
            Vector3.Add(ref position, ref this.Translation, out position);


            if (Deplacement > Chemin.Longueur)
                notifyRelaisAtteint(this);

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Cercle.Position.X = Position.X;
            Cercle.Position.Y = Position.Y;

            RepresentationVivant.Rotation += VitesseRotation;

            if (RepresentationDeplacement != null && EstVivant)
                RepresentationDeplacement.Emettre(ref this.position);
        }



        public void Draw(GameTime gameTime)
        {
            if (EstVivant)
                RepresentationVivant.Position = Position;
            else
                RepresentationMort.Position = Position;

            Simulation.Scene.ajouterScenable(EstVivant ? RepresentationVivant : RepresentationMort);
        }


        public void DrawProjection(GameTime gameTime)
        {
            if (EstVivant)
            {
                RepresentationVivantProjection.Couleur = new Color(Color.White, 25);
                RepresentationVivantProjection.Melange = TypeMelange.Soustraire;
                CheminProjection.Position(Deplacement, ref RepresentationVivantProjection.position);
            }

            else
            {
                RepresentationMortProjection.Couleur = new Color(RepresentationMort.Couleur, 100);
                CheminProjection.Position(Deplacement, ref RepresentationMortProjection.position);
            }

            Simulation.Scene.ajouterScenable(EstVivant ? RepresentationVivantProjection : RepresentationMortProjection);
        }


        public void doTouche(IObjetVivant par)
        {
            if (!(par is ProjectileSlowMotion))
                this.PointsVie -= par.PointsAttaque;

            Projectile p = par as Projectile;

            if (p != null)
            {
                PositionDernierProjectileTouche = p.Position;

                if (p is ProjectileLaserMultiple)
                {
                    EtincellesLaserMultiple.Emettre(ref this.position);
                }
                else if (p is ProjectileLaserSimple)
                {
                    EtincellesLaserSimple.Emettre(ref this.position);
                }
                else if (p is ProjectileMissile)
                {
                    EtincellesMissile.Emettre(ref this.position);
                }
                else if (p is ProjectileSlowMotion)
                {
                    float pointsAttaqueEffectif = (this.Type == TypeEnnemi.Comet) ? p.PointsAttaque * 3 : p.PointsAttaque;

                    this.Resistance = (float)Math.Min(this.Resistance + pointsAttaqueEffectif, 0.75 * this.Vitesse);
                    EtincellesSlowMotion.Emettre(ref this.position);
                }

                return;
            }

            CorpsCeleste c = par as CorpsCeleste;

            if (c != null)
            {
                PositionDernierProjectileTouche = c.Position;
            }
        }


        public void doMeurt()
        {
            PointsVie = 0;

            if (RepresentationExplose != null && PositionDernierProjectileTouche != null)
            {
                Vector3 direction = this.Position - PositionDernierProjectileTouche;
                direction.Normalize();
                direction *= 150;

                RepresentationExplose.ParticleEffect[0].ReleaseImpulse = new Vector2(direction.X, direction.Y);

                RepresentationExplose.Emettre(ref this.position);
            }

            Simulation.Scene.Particules.retourner(EtincellesLaserMultiple);
            Simulation.Scene.Particules.retourner(EtincellesMissile);
            Simulation.Scene.Particules.retourner(EtincellesLaserSimple);
            Simulation.Scene.Particules.retourner(EtincellesSlowMotion);
            Simulation.Scene.Particules.retourner(RepresentationExplose);

            RelaisAtteint = null;
            FactoryEnnemis.Instance.retournerEnnemi(this);
        }


        /// <summary>
        /// Cr�e un evenement qui dit que l'ennemi � atteint son relais
        /// </summary>
        protected virtual void notifyRelaisAtteint(Ennemi ennemi)
        {
            if (RelaisAtteint != null)
                RelaisAtteint(ennemi);
        }


        public override int GetHashCode()
        {
            return Id;
        }
    }
}