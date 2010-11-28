namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using ProjectMercury.Emitters;

    public enum PowerUp
    {
        Aucune = -1,
        DoItYourself = 0,
        CollectTheRent = 1,
        FinalSolution = 2,
        TheResistance = 3
    }


    class MenuCorpsCeleste : DrawableGameComponent
    {
        // Données externes
        public CorpsCeleste CorpsCeleste;
        public Vector3 Position;
        public Dictionary<PowerUp, bool> Options;
        public PowerUp OptionSelectionee;

        private Scene Scene;
        private Bulle Bulle;

        private IVisible WidgetSelection;
        private IVisible LogoDoItYourSelf;
        private IVisible PrixDoItYourself;
        private IVisible LogoCollecteur;
        private IVisible PrixCollecteur;
        private IVisible LogoDestructionCorpsCeleste;
        private IVisible PrixDestructionCorpsCeleste;
        private IVisible LogoTheResistance;
        private IVisible PrixTheResistance;
        private float PrioriteAffichage;


        public MenuCorpsCeleste(
            Simulation simulation,
            Bulle bulle,
            CorpsCeleste corpsCeleste,
            Vector3 positionInitiale,
            Dictionary<PowerUp, bool> options,
            float prioriteAffichage)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;
            Bulle = bulle;
            CorpsCeleste = corpsCeleste;
            Position = positionInitiale;
            Options = options;
            PrioriteAffichage = prioriteAffichage;

            LogoDoItYourSelf = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Vaisseau"), Vector3.Zero, Scene);
            LogoDoItYourSelf.Taille = 4;
            LogoDoItYourSelf.PrioriteAffichage = this.PrioriteAffichage;

            PrixDoItYourself = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixDoItYourself.Taille = 2;
            PrixDoItYourself.PrioriteAffichage = this.PrioriteAffichage;

            LogoCollecteur = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Collecteur"), Vector3.Zero, Scene);
            LogoCollecteur.Taille = 4;
            LogoCollecteur.PrioriteAffichage = this.PrioriteAffichage;

            PrixCollecteur = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixCollecteur.Taille = 2;
            PrixCollecteur.PrioriteAffichage = this.PrioriteAffichage;

            LogoDestructionCorpsCeleste = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Destruction"), Vector3.Zero, Scene);
            LogoDestructionCorpsCeleste.Taille = 4;
            LogoDestructionCorpsCeleste.PrioriteAffichage = this.PrioriteAffichage;

            PrixDestructionCorpsCeleste = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixDestructionCorpsCeleste.Taille = 2;
            PrixDestructionCorpsCeleste.PrioriteAffichage = this.PrioriteAffichage;

            LogoTheResistance = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("TheResistance"), Vector3.Zero, Scene);
            LogoTheResistance.Taille = 4;
            LogoTheResistance.PrioriteAffichage = this.PrioriteAffichage;

            PrixTheResistance = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixTheResistance.Taille = 2;
            PrixTheResistance.PrioriteAffichage = this.PrioriteAffichage;


            WidgetSelection = new IVisible
            (
                Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"),
                Position,
                simulation.Scene
            );
            WidgetSelection.Couleur = new Color(Color.Green, 230);
            WidgetSelection.PrioriteAffichage = this.PrioriteAffichage + 0.01f;
        }


        public override void Draw(GameTime gameTime)
        { 
            WidgetSelection.TailleVecteur = new Vector2(190, 40);

            Vector3 prochainePositionRelative = new Vector3(0, 0, 0);
            Vector3 margin = new Vector3(10, 10, 0);

            // Afficher DoItYourself

            if (CorpsCeleste.PeutAvoirDoItYourself)
            {
                LogoDoItYourSelf.Position = this.Position + prochainePositionRelative + margin;
                PrixDoItYourself.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixDoItYourself.Texte = CorpsCeleste.PrixDoItYourself + "M$";

                if (OptionSelectionee == 0)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Scene.ajouterScenable(WidgetSelection);
                }

                PrixDoItYourself.Couleur = (Options[0]) ? Color.White : Color.Red;

                Scene.ajouterScenable(LogoDoItYourSelf);
                Scene.ajouterScenable(PrixDoItYourself);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher CollectTheRent

            if (CorpsCeleste.PeutAvoirCollecteur)
            {
                LogoCollecteur.Position = this.Position + prochainePositionRelative + margin;
                PrixCollecteur.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixCollecteur.Texte = CorpsCeleste.PrixCollecteur + "M$";

                if (OptionSelectionee == PowerUp.CollectTheRent)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Scene.ajouterScenable(WidgetSelection);
                }

                PrixCollecteur.Couleur = (Options[PowerUp.CollectTheRent]) ? Color.White : Color.Red;

                Scene.ajouterScenable(LogoCollecteur);
                Scene.ajouterScenable(PrixCollecteur);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher FinalSolution

            if (CorpsCeleste.PeutDetruire)
            {
                LogoDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + margin;
                PrixDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixDestructionCorpsCeleste.Texte = CorpsCeleste.PrixDestruction + "M$";

                if (OptionSelectionee == PowerUp.FinalSolution)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Scene.ajouterScenable(WidgetSelection);
                }

                PrixDestructionCorpsCeleste.Couleur = (Options[PowerUp.FinalSolution]) ? Color.White : Color.Red;

                Scene.ajouterScenable(LogoDestructionCorpsCeleste);
                Scene.ajouterScenable(PrixDestructionCorpsCeleste);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher TheResistance

            if (CorpsCeleste.PeutAvoirTheResistance)
            {
                LogoTheResistance.Position = this.Position + prochainePositionRelative + margin;
                PrixTheResistance.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixTheResistance.Texte = CorpsCeleste.PrixTheResistance + "M$";

                if (OptionSelectionee == PowerUp.TheResistance)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Scene.ajouterScenable(WidgetSelection);
                }

                PrixTheResistance.Couleur = (Options[PowerUp.TheResistance]) ? Color.White : Color.Red;

                Scene.ajouterScenable(LogoTheResistance);
                Scene.ajouterScenable(PrixTheResistance);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }

            if (CorpsCeleste.PeutAvoirDoItYourself || CorpsCeleste.PeutDetruire || CorpsCeleste.PeutAvoirCollecteur)
                Bulle.Draw(null);
        }
    }
}
