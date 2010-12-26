﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;

    class Monde : DrawableGameComponent
    {
        public List<TrouRose> TrousRoses;
        public bool Debloque;
        public int Numero;
        public DescripteurScenario ScenarioSelectionne { get; private set; }

        protected Main Main;
        protected Scene Scene;
        public Simulation Simulation;
        protected IVisible Titre;
        protected IVisible Infos;
        protected DescripteurScenario Descripteur;
        protected Dictionary<String, DescripteurScenario> Scenarios;

        private Dictionary<String, List<Lune>> ScenarioLunes;
        private CorpsCeleste dernierCorpsCelesteSelectionne;
        private TheResistance ResistancePartieEnCours;
        private CorpsCeleste corpsCelestePartieEnPause;


        public String CorpsCelestePartieEnPause
        {
            set
            {
                corpsCelestePartieEnPause = null;

                if (value == null)
                    return;

                for (int i = 0; i < Simulation.ControleurSystemePlanetaire.CorpsCelestes.Count; i++)
                    if (Simulation.ControleurSystemePlanetaire.CorpsCelestes[i].Nom.Equals(value))
                    {
                        corpsCelestePartieEnPause = Simulation.ControleurSystemePlanetaire.CorpsCelestes[i];
                        return;
                    }
            }
        }


        public Monde(Main main, Scene scene, int numero, DescripteurScenario descripteur, Dictionary<String, DescripteurScenario> scenarios, bool initParticules)
            : base(main)
        {
            Main = main;
            Scene = scene;

            Numero = numero;

            Descripteur = descripteur;

            ScenarioSelectionne = new DescripteurScenario();

            Simulation = new Simulation(main, scene, Descripteur);
            Simulation.InitParticules = initParticules;
            Simulation.Players = Main.Players;
            Simulation.DemoModeSelectedScenario = ScenarioSelectionne;
            Simulation.Initialize();
            Simulation.ModeDemo = true;

            Scenarios = scenarios;

            Titre = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
            Titre.Taille = 4;
            Titre.Origine = Titre.Centre;
            Titre.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            Titre.Couleur.A = 200;

            Infos = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
            Infos.Taille = 3;
            Infos.Origine = Infos.Centre;
            Infos.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            Infos.Couleur.A = 200;

            TrousRoses = new List<TrouRose>();
            ScenarioLunes = new Dictionary<string, List<Lune>>();

            foreach (var corpsCeleste in Simulation.ControleurSystemePlanetaire.CorpsCelestes)
            {
                if (corpsCeleste is TrouRose)
                    TrousRoses.Add((TrouRose)corpsCeleste);

                ScenarioLunes.Add(corpsCeleste.Nom, new List<Lune>());
            }

            Debloque = true;

            ResistancePartieEnCours = new TheResistance(Simulation, null, new List<Ennemi>());
            
            initLunes();
        }

        public String CorpsSelectionne
        {
            get { return (Simulation.CorpsCelesteSelectionne != null) ? Simulation.CorpsCelesteSelectionne.Nom : ""; }
        }


        public void afficherMessageBloque(String message)
        {
            dernierCorpsCelesteSelectionne = Simulation.CorpsCelesteSelectionne;
            Simulation.ControleurMessages.afficherMessage(dernierCorpsCelesteSelectionne, message, 5000, -1);
        }


        public void arreterMessageBloque()
        {
            Simulation.ControleurMessages.arreterMessage(dernierCorpsCelesteSelectionne);
        }


        public void afficherMessagePause(String message)
        {
            Simulation.ControleurMessages.afficherMessage(corpsCelestePartieEnPause, message, 1000000000, -1);
        }


        public void arreterMessagePause()
        {
            Simulation.ControleurMessages.arreterMessage(corpsCelestePartieEnPause);
        }        


        public override void Update(GameTime gameTime)
        {
            ScenarioSelectionne = (CorpsSelectionne != "") ? Scenarios[CorpsSelectionne] : null;
            Simulation.DemoModeSelectedScenario.Numero = (ScenarioSelectionne != null) ? ScenarioSelectionne.Numero : -1;

            Simulation.Update(gameTime);

            foreach (var listeLunes in ScenarioLunes.Values)
                foreach (var lune in listeLunes)
                    lune.Update(gameTime);

            if (corpsCelestePartieEnPause != null)
            {
                ResistancePartieEnCours.CorpsCelesteDepart = corpsCelestePartieEnPause;
                ResistancePartieEnCours.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Simulation.Draw(gameTime);

            foreach (var listeLunes in ScenarioLunes.Values)
                foreach (var lune in listeLunes)
                    lune.Draw(gameTime);

            if (corpsCelestePartieEnPause != null)
            {
                ResistancePartieEnCours.CorpsCelesteDepart = corpsCelestePartieEnPause;
                ResistancePartieEnCours.Draw(gameTime);
            }

            if (Simulation.CorpsCelesteSelectionne == null)
                return;

            Titre.Texte = Scenarios[Simulation.CorpsCelesteSelectionne.Nom].Mission;
            Titre.Position = new Vector3(Simulation.CorpsCelesteSelectionne.Position.X, Simulation.CorpsCelesteSelectionne.Position.Y - Simulation.CorpsCelesteSelectionne.Cercle.Rayon - 32, 0);
            Titre.Origine = Titre.Centre;
            Infos.Texte = Scenarios[Simulation.CorpsCelesteSelectionne.Nom].Difficulte;
            Infos.Position = new Vector3(Simulation.CorpsCelesteSelectionne.Position.X, Simulation.CorpsCelesteSelectionne.Position.Y + Simulation.CorpsCelesteSelectionne.Cercle.Rayon + 16, 0);
            Infos.Origine = Infos.Centre;
            Scene.ajouterScenable(Titre);
            Scene.ajouterScenable(Infos);
        }

        public void initLunes()
        {
            foreach (var corpsCeleste in Simulation.ControleurSystemePlanetaire.CorpsCelestes)
            {
                ScenarioLunes[corpsCeleste.Nom].Clear();

                if (!Scenarios.ContainsKey(corpsCeleste.Nom))
                    continue;

                DescripteurScenario desc = Scenarios[corpsCeleste.Nom];

                int level = 0;
                bool negatif = Main.SaveGame.Progress.TryGetValue(desc.Numero, out level) && level < 0;
                int nbLunes = Math.Min(Math.Abs(level), 10);

                for (int i = 0; i < nbLunes; i++)
                {
                    Lune lune;

                    if ((Main.Random.Next(0, 2) == 0))
                        lune = new LuneMatrice(Simulation, corpsCeleste);
                    else
                        lune = new LuneTrajet(Simulation, corpsCeleste);

                    lune.Representation.Texture = EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>((negatif) ? "luneLoose" : "luneWin");
                    lune.Representation.Couleur.A = 255;
                    lune.Representation.Taille = 4;
                    lune.Representation.Origine = lune.Representation.Centre;

                    ScenarioLunes[corpsCeleste.Nom].Add(lune);
                }
            }
        }
    }
}
