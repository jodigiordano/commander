namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Ennemi : IObjetPhysique, ILivingObject
    {
        public static int NextID { get { return NEXT_ID++; } }
        public delegate void RelaisAtteintHandler(Ennemi ennemi);
        public event RelaisAtteintHandler RelaisAtteint;
        public int Id;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Vitesse                                        { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Forme Forme                                          { get; set; }
        public Cercle Cercle                                        { get; set; }
        public Ligne Ligne                                          { get; set; }
        public RectanglePhysique Rectangle                          { get; set; }
        public float LifePoints                                     { get; set; }
        public float PointsVieDepart                                { get; set; }
        public float AttackPoints                                   { get; set; }
        public int ValeurUnites;
        public int ValeurPoints;
        public float Resistance;
        public bool Alive                                           { get { return LifePoints > 0; } }
        public bool FinCheminProjection                             { get { return Deplacement > CheminProjection.Longueur; } }
        public IVisible RepresentationVivant;
        public IVisible RepresentationMort;
        public ParticuleEffectWrapper RepresentationExplose;
        public ParticuleEffectWrapper RepresentationDeplacement;
        public float VitesseRotation;
        public Path Path;
        public Path CheminProjection;
        public double Deplacement;
        public Simulation Simulation;
        public String Nom;
        public EnemyType Type;
        public List<Mineral> Mineraux;
        public Vector3 Translation;
        public Vector3 PositionDernierProjectileTouche;
        public Color Couleur;

        public float NanobotsInfectionTime;
        public float NanobotsInfectionHitPoints;
        public ParticuleEffectWrapper NanobotsInfectionEffect;
        private Vector3 NanobotsInfectionLastPosition;

        private ParticuleEffectWrapper EtincellesLaserMultiple;
        private ParticuleEffectWrapper EtincellesMissile;
        private ParticuleEffectWrapper EtincellesLaserSimple;
        private ParticuleEffectWrapper EtincellesSlowMotion;
        private static int NEXT_ID = 0;
        private Vector3 position;
        private float VisualPriority;


        public Ennemi()
        {
            Mineraux = new List<Mineral>();

            RepresentationDeplacement = null;

            AttackPoints = 1;
            ValeurPoints = 1;

            Forme = Forme.Rectangle;
            Rectangle = new RectanglePhysique(0, 0, 1, 1);
            Cercle = new Cercle(Vector3.Zero, 1);
            Type = EnemyType.Inconnu;
            Id = NextID;
        }


        public void Initialize()
        {
            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(Nom), Vector3.Zero);
                RepresentationVivant.Origine = RepresentationVivant.Centre;
                RepresentationVivant.Taille = 3;
                RepresentationMort = RepresentationVivant;
            }

            EtincellesLaserMultiple = Simulation.Scene.Particules.recuperer("etincelleLaser");
            EtincellesMissile = Simulation.Scene.Particules.recuperer("etincelleMissile");
            EtincellesLaserSimple = Simulation.Scene.Particules.recuperer("etincelleLaserSimple");
            EtincellesSlowMotion = Simulation.Scene.Particules.recuperer("etincelleSlowMotionTouche");
            RepresentationExplose = Simulation.Scene.Particules.recuperer("explosionEnnemi");
            NanobotsInfectionEffect = Simulation.Scene.Particules.recuperer("nanobots");
            NanobotsInfectionEffect.ParticleEffect[0].ReleaseColour = Color.Red.ToVector3();

            VisualPriority = EnemiesFactory.GetVisualPriority(Type, 0);
            RepresentationVivant.VisualPriority = VisualPriority;
            RepresentationExplose.VisualPriority = VisualPriority - 0.001f;
            EtincellesLaserMultiple.VisualPriority = VisualPriority - 0.001f;
            EtincellesMissile.VisualPriority = VisualPriority - 0.001f;
            EtincellesLaserSimple.VisualPriority = VisualPriority - 0.001f;
            EtincellesSlowMotion.VisualPriority = VisualPriority - 0.001f;
            NanobotsInfectionEffect.VisualPriority = VisualPriority - 0.001f;

            Resistance = 0;
            Deplacement = 0;

            Rectangle.Width = Rectangle.Height = EnemiesFactory.GetSize(Type);
            Cercle.Radius = Rectangle.Width / 2 - 3;

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Cercle.Position.X = Position.X - Cercle.Radius;
            Cercle.Position.Y = Position.Y - Cercle.Radius;

            RepresentationExplose.ParticleEffect[0].ReleaseColour = Couleur.ToVector3();
            RepresentationDeplacement = null;

            VitesseRotation = Main.Random.Next(-5, 6) / 100.0f;

            Mineraux.Clear();

            NanobotsInfectionTime = 0;
            NanobotsInfectionHitPoints = 0;
            NanobotsInfectionLastPosition = Position;
        }


        public void Update(GameTime gameTime)
        {
            Resistance = Math.Max(Resistance - 0.02f, 0);

            Deplacement += Math.Max(this.Vitesse - this.Resistance, 0);

            Path.Position(Deplacement, ref position);
            Vector3.Add(ref position, ref this.Translation, out position);

            if (Deplacement > Path.Longueur)
                notifyRelaisAtteint(this);

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Cercle.Position.X = Position.X;
            Cercle.Position.Y = Position.Y;

            RepresentationVivant.Rotation += VitesseRotation;

            if (NanobotsInfectionTime > 0 && LifePoints > 0)
            {
                LifePoints = Math.Max(0, LifePoints - NanobotsInfectionHitPoints);
                NanobotsInfectionTime -= 16.66f;
            }
        }


        public void Draw(GameTime gameTime)
        {
            float pourcPath = Path.Pourc(Deplacement);

            if (pourcPath > 0.95f)
                VisualPriority = EnemiesFactory.GetVisualPriority(Type, pourcPath);

            if (Alive)
            {
                RepresentationVivant.Position = Position;
                RepresentationVivant.VisualPriority = VisualPriority + pourcPath / 1000f;

                if (RepresentationDeplacement != null)
                    RepresentationDeplacement.Emettre(ref this.position);

                if (NanobotsInfectionTime > 0)
                {
                    Vector3 deplacement;
                    Vector3.Subtract(ref this.position, ref NanobotsInfectionLastPosition, out deplacement);

                    if (deplacement.X != 0 && deplacement.Y != 0)
                    {
                        NanobotsInfectionEffect.Deplacer(ref deplacement);
                        NanobotsInfectionLastPosition = this.position;
                    }

                    NanobotsInfectionEffect.Emettre(ref this.position);
                }
            }
            else
            {
                RepresentationMort.Position = Position;
                RepresentationMort.VisualPriority = VisualPriority + pourcPath / 1000f;
            }

            Simulation.Scene.ajouterScenable(Alive ? RepresentationVivant : RepresentationMort);
        }


        public void DoHit(ILivingObject par)
        {
            if (!(par is ProjectileSlowMotion))
                this.LifePoints -= par.AttackPoints;

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
                    float pointsAttaqueEffectif = (this.Type == EnemyType.Comet) ? p.AttackPoints * 3 : p.AttackPoints;

                    this.Resistance = (float)Math.Min(this.Resistance + pointsAttaqueEffectif, 0.75 * this.Vitesse);
                    EtincellesSlowMotion.Emettre(ref this.position);
                }
                else if (p is NanobotsBullet)
                {
                    NanobotsBullet nb = p as NanobotsBullet;

                    NanobotsInfectionTime = nb.InfectionTime;
                    NanobotsInfectionHitPoints = nb.AttackPoints;
                }

                return;
            }

            CorpsCeleste c = par as CorpsCeleste;

            if (c != null)
            {
                PositionDernierProjectileTouche = c.Position;
            }
        }


        public void DoDie()
        {
            LifePoints = 0;

            if (RepresentationExplose != null)
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
            Simulation.Scene.Particules.retourner(NanobotsInfectionEffect);

            RelaisAtteint = null;
            Simulation.EnemiesFactory.ReturnEnemy(this);
        }


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