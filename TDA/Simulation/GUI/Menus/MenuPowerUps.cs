namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuPowerUps : MenuAbstract
    {
        public CorpsCeleste CelestialBody;
        public Dictionary<PowerUp, bool> Options;
        public PowerUp SelectedOption;

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


        public MenuPowerUps(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            PrioriteAffichage = prioriteAffichage;

            LogoDoItYourSelf = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Vaisseau"), Vector3.Zero, Simulation.Scene);
            LogoDoItYourSelf.Taille = 4;
            LogoDoItYourSelf.PrioriteAffichage = this.PrioriteAffichage;

            PrixDoItYourself = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            PrixDoItYourself.Taille = 2;
            PrixDoItYourself.PrioriteAffichage = this.PrioriteAffichage;

            LogoCollecteur = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Collecteur"), Vector3.Zero, Simulation.Scene);
            LogoCollecteur.Taille = 4;
            LogoCollecteur.PrioriteAffichage = this.PrioriteAffichage;

            PrixCollecteur = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            PrixCollecteur.Taille = 2;
            PrixCollecteur.PrioriteAffichage = this.PrioriteAffichage;

            LogoDestructionCorpsCeleste = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Destruction"), Vector3.Zero, Simulation.Scene);
            LogoDestructionCorpsCeleste.Taille = 4;
            LogoDestructionCorpsCeleste.PrioriteAffichage = this.PrioriteAffichage;

            PrixDestructionCorpsCeleste = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            PrixDestructionCorpsCeleste.Taille = 2;
            PrixDestructionCorpsCeleste.PrioriteAffichage = this.PrioriteAffichage;

            LogoTheResistance = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("TheResistance"), Vector3.Zero, Simulation.Scene);
            LogoTheResistance.Taille = 4;
            LogoTheResistance.PrioriteAffichage = this.PrioriteAffichage;

            PrixTheResistance = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
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


        protected override Vector2 MenuSize
        {
            get
            {
                if (CelestialBody == null)
                    return Vector2.Zero;

                int nb = Options.Count;

                if (!CelestialBody.PeutAvoirDoItYourself)
                    nb--;

                if (!CelestialBody.PeutDetruire)
                    nb--;

                if (!CelestialBody.PeutAvoirCollecteur)
                    nb--;

                if (!CelestialBody.PeutAvoirTheResistance)
                    nb--;

                return new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position;
            }
        }


        public override void Draw()
        {
            if (CelestialBody == null || SelectedOption == PowerUp.Aucune)
                return;

            base.Draw();

            WidgetSelection.TailleVecteur = new Vector2(190, 40);

            Vector3 prochainePositionRelative = new Vector3(0, 0, 0);
            Vector3 margin = new Vector3(10, 10, 0);

            // Afficher DoItYourself

            if (CelestialBody.PeutAvoirDoItYourself)
            {
                LogoDoItYourSelf.Position = this.Position + prochainePositionRelative + margin;
                PrixDoItYourself.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixDoItYourself.Texte = CelestialBody.PrixDoItYourself + "M$";

                if (SelectedOption == 0)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixDoItYourself.Couleur = (Options[0]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoDoItYourSelf);
                Simulation.Scene.ajouterScenable(PrixDoItYourself);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher CollectTheRent

            if (CelestialBody.PeutAvoirCollecteur)
            {
                LogoCollecteur.Position = this.Position + prochainePositionRelative + margin;
                PrixCollecteur.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixCollecteur.Texte = CelestialBody.PrixCollecteur + "M$";

                if (SelectedOption == PowerUp.CollectTheRent)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixCollecteur.Couleur = (Options[PowerUp.CollectTheRent]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoCollecteur);
                Simulation.Scene.ajouterScenable(PrixCollecteur);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher FinalSolution

            if (CelestialBody.PeutDetruire)
            {
                LogoDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + margin;
                PrixDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixDestructionCorpsCeleste.Texte = CelestialBody.PrixDestruction + "M$";

                if (SelectedOption == PowerUp.FinalSolution)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixDestructionCorpsCeleste.Couleur = (Options[PowerUp.FinalSolution]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoDestructionCorpsCeleste);
                Simulation.Scene.ajouterScenable(PrixDestructionCorpsCeleste);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }


            // Afficher TheResistance

            if (CelestialBody.PeutAvoirTheResistance)
            {
                LogoTheResistance.Position = this.Position + prochainePositionRelative + margin;
                PrixTheResistance.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
                PrixTheResistance.Texte = CelestialBody.PrixTheResistance + "M$";

                if (SelectedOption == PowerUp.TheResistance)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixTheResistance.Couleur = (Options[PowerUp.TheResistance]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoTheResistance);
                Simulation.Scene.ajouterScenable(PrixTheResistance);

                prochainePositionRelative += new Vector3(0, 40, 0);
            }

            if (CelestialBody.PeutAvoirDoItYourself || CelestialBody.PeutDetruire || CelestialBody.PeutAvoirCollecteur)
                Bulle.Draw(null);
        }
    }
}
