namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class PointsDeVieJoueur : DrawableGameComponent
    {
        private Simulation Simulation;
        private CorpsCeleste CorpsCeleste;
        private List<Lune> Lunes;
        //private IVisible RepresentationDessous;
        //private IVisible RepresentationPoints;
        //private IVisible RepresentationDessus;
        //private Vector3 PositionOffset;

        //public float PointsDeVieOffset;


        public PointsDeVieJoueur(Simulation simulation, CorpsCeleste corpsCeleste, Vector3 positionOffset, Color couleur)
            : base(simulation.Main)
        {
            Simulation = simulation;
            CorpsCeleste = corpsCeleste;

            CorpsCeleste.Lunes.Clear();

            Lunes = new List<Lune>();

            for (int i = 0; i < corpsCeleste.PointsVie; i++)
                Lunes.Add(creerLune());

            //PositionOffset = positionOffset;

            //RepresentationDessus = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PointsVieJoueur1"), corpsCeleste.Position, Scene);
            //RepresentationDessus.PrioriteAffichage = Preferences.PrioriteGUIPointsVieJoueur + 0.01f;
            //RepresentationDessus.Origine = RepresentationDessus.Centre;

            //RepresentationPoints = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), couleur, corpsCeleste.Position, Scene);
            //RepresentationPoints.Taille = 4;
            //RepresentationPoints.PrioriteAffichage = Preferences.PrioriteGUIPointsVieJoueur + 0.02f;
            //RepresentationPoints.Origine = RepresentationPoints.Centre;


            //RepresentationDessous = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PointsVieJoueur2"), corpsCeleste.Position, Scene);
            //RepresentationDessous.PrioriteAffichage = Preferences.PrioriteGUIPointsVieJoueur + 0.03f;
            //RepresentationDessous.Origine = RepresentationDessous.Centre;
        }

        public override void Update(GameTime gameTime)
        {
            int difference = (int) (Lunes.Count - CorpsCeleste.PointsVie);

            if (difference < 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                    Lunes.Add(creerLune());
            }

            else if (difference > 0)
            {
                for (int i = 0; i < difference && Lunes.Count > 0; i++)
                    Lunes.RemoveAt(Lunes.Count - 1);
            }

            foreach (var lune in Lunes)
                lune.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (CorpsCeleste.EstVivant)
            {
                foreach (var lune in Lunes)
                    lune.Draw(gameTime);

                //RepresentationPoints.Position = CorpsCeleste.Position + this.PositionOffset;
                //RepresentationDessus.Position = RepresentationDessous.Position = RepresentationPoints.Position;

                //RepresentationPoints.Texte = (CorpsCeleste.PointsVie + this.PointsDeVieOffset).ToString();
                //RepresentationPoints.Origine = RepresentationPoints.Centre;

                //Scene.ajouterScenable(RepresentationPoints);
                //Scene.ajouterScenable(RepresentationDessous);
                //Scene.ajouterScenable(RepresentationDessus);
            }
        }

        private Lune creerLune()
        {
            Lune lune;

            if (Main.Random.Next(0, 2) == 0)
                lune = new LuneMatrice(Simulation, CorpsCeleste);
            else
                lune = new LuneTrajet(Simulation, CorpsCeleste);

            lune.Representation.Texture = Core.Persistance.Facade.recuperer<Texture2D>("luneVies");
            lune.Representation.Couleur.A = 255;
            lune.Representation.Taille = 3;
            lune.Representation.Origine = lune.Representation.Centre;

            return lune;
        }
    }
}
