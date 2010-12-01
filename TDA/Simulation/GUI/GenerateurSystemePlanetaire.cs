namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Visuel;
    using RainingSundays.Core.Physique;
    using RainingSundays.Core.Utilities;

    class GenerateurSystemePlanetaire : DrawableGameComponent
    {
        public bool Visible;

        private Simulation Simulation;
        private IVisible Filtre;
        private IVisible PlanetarySystem;
        private IVisible NumberOfPlanets;
        private HorizontalSlider NumberOfPlanetsSlider;
        private IVisible Generate;
        private PushButton GeneratePushButton;
        private Vector3 Position;

        private GenerateurScenario Generateur;
        private DescripteurScenario DescripteurScenario;


        public GenerateurSystemePlanetaire(Simulation simulation, Curseur curseur, Vector3 position)
            : base(simulation.Main)
        {
            Simulation = simulation;
            Position = position;

            Filtre = new IVisible(RainingSundays.Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Position, Simulation.Scene);
            Filtre.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.01f;
            Filtre.TailleVecteur = new Vector2(540, 340);
            Filtre.Couleur = new Color(255, 0, 220, 128);
            Filtre.Origine = Filtre.Centre;

            PlanetarySystem = new IVisible("Planetary System", RainingSundays.Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(0, 150, 0), Simulation.Scene);
            PlanetarySystem.Taille = 4;
            PlanetarySystem.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.005f;
            PlanetarySystem.Origine = PlanetarySystem.Centre;

            NumberOfPlanets = new IVisible("Number of planets", RainingSundays.Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 120, 0), Simulation.Scene);
            NumberOfPlanets.Taille = 2;
            NumberOfPlanets.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.005f;

            NumberOfPlanetsSlider = new HorizontalSlider(Simulation, curseur, Position - new Vector3(-150, 110, 0), 1, 15, 3, Preferences.PrioriteGUIPanneauGeneral + 0.005f);

            Generate = new IVisible("Generate", RainingSundays.Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(200, -120, 0), Simulation.Scene);
            Generate.Taille = 2;
            Generate.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.005f;

            GeneratePushButton = new PushButton(Simulation, curseur, Position - new Vector3(250, -120, 0), Preferences.PrioriteGUIPanneauGeneral + 0.005f);

            Generateur = new GenerateurScenario();
            Generateur.NbCorpsCelestes = NumberOfPlanetsSlider.Valeur;
            Generateur.NbEmplacements = 10;
            //DescripteurScenario = Generateur.generer();
        }


        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            NumberOfPlanetsSlider.Update(gameTime);
            GeneratePushButton.Update(gameTime);

            if (GeneratePushButton.Pressed)
            {
                Generateur.NbCorpsCelestes = NumberOfPlanetsSlider.Valeur;

                DescripteurScenario = Generateur.generer();
                Simulation.DescriptionScenario = DescripteurScenario;
                Simulation.Initialize();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            Simulation.Scene.ajouterScenable(Filtre);
            Simulation.Scene.ajouterScenable(PlanetarySystem);
            Simulation.Scene.ajouterScenable(NumberOfPlanets);
            Simulation.Scene.ajouterScenable(Generate);

            NumberOfPlanetsSlider.Draw(gameTime);
            GeneratePushButton.Draw(gameTime);
        }
    }
}
