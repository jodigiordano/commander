namespace TDA.Objets
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    
    public class AnimationTransition : Animation
    {
        private List<VaisseauAlien> VaisseausAlien;
        private List<IVisible> Autres;
        private List<Trajet3D> Trajets;
        private bool ShowAliens;

        public bool In;

        private static int LastTimeChoice = 0;
        private static String LastTimeChoiceAutre = "";

        private static Random Random = new Random();

        private static List<KeyValuePair<Vector3, Vector3>> PositionsIn = new List<KeyValuePair<Vector3,Vector3>>()
        {
            new KeyValuePair<Vector3, Vector3>(new Vector3(0, 200, 0), new Vector3(0, 1000, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(500, 0, 0), new Vector3(500, 1200, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-100, -300, 0), new Vector3(-100, 900, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-500, 400, 0), new Vector3(-500, 1000, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-550, -200, 0), new Vector3(-550, 1000, 0)),
        };


        private static List<KeyValuePair<Vector3, Vector3>> PositionsOut = new List<KeyValuePair<Vector3, Vector3>>()
        {
            new KeyValuePair<Vector3, Vector3>(new Vector3(0, -1000, 0), new Vector3(0, 200, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(500, -800, 0), new Vector3(500, 0, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-100, -800, 0), new Vector3(-100, -300, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-500, -800, 0), new Vector3(-500, 400, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-550, -800, 0), new Vector3(-550, -200, 0)),
        };

        private static List<String> NomsAutres = new List<string>()
        {
            "planete21",
            "Asteroid",
            "Comet",
            "Plutoid",
            "Centaur",
            "Trojan",
            "Meteoroid",
            "tourelleBase3",
            "tourelleGravitationnelleAntenne",
            "tourelleLaserCanon",
            "tourelleLaserMultiple3",
            "tourelleMissileCanon"
        };

        public AnimationTransition()
            : base()
        {
            In = false;
            ShowAliens = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            VaisseausAlien = new List<VaisseauAlien>();
            Autres = new List<IVisible>();
            Trajets = new List<Trajet3D>();

            if (!In)
            {
                LastTimeChoice = Random.Next(0, 2);
                LastTimeChoiceAutre = NomsAutres[Random.Next(0, NomsAutres.Count)];
            }

            //ShowAliens = (LastTimeChoice == 0);
            ShowAliens = true;

            if (ShowAliens)
            {
                for (int i = 0; i < 5; i++)
                {
                    VaisseauAlien v = new VaisseauAlien(Scene, this.PrioriteAffichage);
                    v.Representation.Taille = 16;
                    v.Representation.Rotation = MathHelper.PiOver2;
                    v.Tentacules.Taille = 16;
                    v.Tentacules.Rotation = MathHelper.PiOver2;
                    v.Representation.Melange = TypeMelange.Soustraire;
                    v.Tentacules.Melange = TypeMelange.Soustraire;

                    VaisseausAlien.Add(v);

                    if (In)
                        Trajets.Add(new Trajet3D(new Vector3[] { PositionsIn[i].Key, PositionsIn[i].Value }, new double[] { 0, this.Duree }));
                    else
                        Trajets.Add(new Trajet3D(new Vector3[] { PositionsOut[i].Key, PositionsOut[i].Value }, new double[] { 0, this.Duree }));
                }
            }

            else
            {
                for (int i = 0; i < 5; i++)
                {
                    IVisible iv = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(LastTimeChoiceAutre), Vector3.Zero, Scene);
                    iv.Taille = 16;
                    iv.Melange = TypeMelange.Soustraire;
                    iv.Origine = iv.Centre;

                    Autres.Add(iv);

                    if (In)
                        Trajets.Add(new Trajet3D(new Vector3[] { PositionsIn[i].Key, PositionsIn[i].Value }, new double[] { 0, this.Duree }));
                    else
                        Trajets.Add(new Trajet3D(new Vector3[] { PositionsOut[i].Key, PositionsOut[i].Value }, new double[] { 0, this.Duree }));
                }
            }
        }

        public override void suivant(GameTime gameTime)
        {
            base.suivant(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            if (ShowAliens)
            {
                for (int i = 0; i < VaisseausAlien.Count; i++)
                {
                    Trajets[i].Position(TempsRelatif, ref VaisseausAlien[i].Representation.position);
                    VaisseausAlien[i].Draw(null);
                }
            }

            else
            {
                for (int i = 0; i < Autres.Count; i++)
                {
                    Trajets[i].Position(TempsRelatif, ref Autres[i].position);
                    Scene.ajouterScenable(Autres[i]);
                }
            }
        }
    }
}
