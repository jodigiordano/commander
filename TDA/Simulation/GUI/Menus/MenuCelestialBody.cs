namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuCelestialBody : MenuAbstract
    {
        public CorpsCeleste CelestialBody;
        public Dictionary<PowerUp, bool> Options;
        public PowerUp SelectedOption;

        public Dictionary<Turret, bool> AvailableTurretsToBuy;
        public Turret TurretToBuy;

        private Dictionary<TurretType, IVisible> LogosTourellesAchat;
        private Dictionary<TurretType, IVisible> PrixTourellesAchat;

        private Image WidgetSelection;
        private Image LogoDoItYourSelf;
        private Text PrixDoItYourself;
        private Image LogoCollecteur;
        private Text PrixCollecteur;
        private Image LogoDestructionCorpsCeleste;
        private Text PrixDestructionCorpsCeleste;
        private Image LogoTheResistance;
        private Text PrixTheResistance;
        private float PrioriteAffichage;

        private int MenuWidth;
        private int DistanceBetweenTwoChoices;
        private Vector3 PositionChoice;
        private Vector3 PositionChoicePrice;
        private float TextSize;
        private float ImageSize;


        public MenuCelestialBody(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            MenuWidth = 140;
            DistanceBetweenTwoChoices = 30;
            PositionChoice = new Vector3(20, 15, 0);
            PositionChoicePrice = new Vector3(40, 2, 0);
            TextSize = 2f;
            ImageSize = 3;

            PrioriteAffichage = prioriteAffichage;

            LogoDoItYourSelf = new Image("Vaisseau", Vector3.Zero);
            LogoDoItYourSelf.SizeX = ImageSize;
            LogoDoItYourSelf.VisualPriority = this.PrioriteAffichage;

            PrixDoItYourself = new Text("", "Pixelite", Color.White, Vector3.Zero);
            PrixDoItYourself.SizeX = TextSize;
            PrixDoItYourself.VisualPriority = this.PrioriteAffichage;

            LogoCollecteur = new Image("Collecteur", Vector3.Zero);
            LogoCollecteur.SizeX = ImageSize;
            LogoCollecteur.VisualPriority = this.PrioriteAffichage;

            PrixCollecteur = new Text("", "Pixelite", Color.White, Vector3.Zero);
            PrixCollecteur.SizeX = TextSize;
            PrixCollecteur.VisualPriority = this.PrioriteAffichage;

            LogoDestructionCorpsCeleste = new Image("Destruction", Vector3.Zero);
            LogoDestructionCorpsCeleste.SizeX = ImageSize;
            LogoDestructionCorpsCeleste.VisualPriority = this.PrioriteAffichage;

            PrixDestructionCorpsCeleste = new Text("", "Pixelite", Color.White, Vector3.Zero);
            PrixDestructionCorpsCeleste.SizeX = TextSize;
            PrixDestructionCorpsCeleste.VisualPriority = this.PrioriteAffichage;

            LogoTheResistance = new Image("TheResistance", Vector3.Zero);
            LogoTheResistance.SizeX = ImageSize;
            LogoTheResistance.VisualPriority = this.PrioriteAffichage;

            PrixTheResistance = new Text("", "Pixelite", Color.White, Vector3.Zero);
            PrixTheResistance.SizeX = TextSize;
            PrixTheResistance.VisualPriority = this.PrioriteAffichage;


            WidgetSelection = new Image
            (
                "PixelBlanc",
                Position
            );
            WidgetSelection.Origin = Vector2.Zero;
            WidgetSelection.Size = new Vector2(MenuWidth, DistanceBetweenTwoChoices);
            WidgetSelection.Color = Color.Green;
            WidgetSelection.Color.A = 230;
            WidgetSelection.VisualPriority = this.PrioriteAffichage + 0.01f;



            AvailableTurretsToBuy = new Dictionary<Turret, bool>();

            LogosTourellesAchat = new Dictionary<TurretType, IVisible>();
            PrixTourellesAchat = new Dictionary<TurretType, IVisible>();

            List<Turret> tourellesDisponibles = simulation.TurretFactory.AvailableTurrets;

            for (int i = 0; i < tourellesDisponibles.Count; i++)
            {
                Turret t = tourellesDisponibles[i];

                IVisible iv = (IVisible) t.BaseImage.Clone();
                iv.VisualPriority = this.PrioriteAffichage;
                iv.Taille = ImageSize;
                LogosTourellesAchat.Add(t.Type, iv);

                IVisible prix = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
                prix.Taille = TextSize;
                prix.VisualPriority = this.PrioriteAffichage;

                PrixTourellesAchat.Add(t.Type, prix);
            }
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (CelestialBody == null)
                    return Vector2.Zero;

                int AvailablePowerUps = Options.Count;

                if (!CelestialBody.PeutAvoirDoItYourself)
                    AvailablePowerUps--;

                if (!CelestialBody.PeutDetruire)
                    AvailablePowerUps--;

                if (!CelestialBody.PeutAvoirCollecteur)
                    AvailablePowerUps--;

                if (!CelestialBody.PeutAvoirTheResistance)
                    AvailablePowerUps--;

                return new Vector2(MenuWidth, AvailablePowerUps * DistanceBetweenTwoChoices + AvailableTurretsToBuy.Count * DistanceBetweenTwoChoices);
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
            if (CelestialBody == null ||
                (CelestialBody != null &&
                AvailableTurretsToBuy.Count == 0 &&
                !Options[PowerUp.CollectTheRent] &&
                !Options[PowerUp.DoItYourself] &&
                !Options[PowerUp.FinalSolution] &&
                !Options[PowerUp.TheResistance]))
                return;

            base.Draw();

            DrawTurretsToBuy();
            DrawPowerUps();

            Bulle.Draw(null);
        }


        private void DrawTurretsToBuy()
        {
            int compteurEmplacement = 0;

            foreach (var tourelle in AvailableTurretsToBuy)
            {
                LogosTourellesAchat[tourelle.Key.Type].Position = this.Position + PositionChoice + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices, 0);
                PrixTourellesAchat[tourelle.Key.Type].Position = this.Position + PositionChoicePrice + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices, 0);
                PrixTourellesAchat[tourelle.Key.Type].Texte = tourelle.Key.BuyPrice + "M$";


                Simulation.Scene.ajouterScenable(LogosTourellesAchat[tourelle.Key.Type]);
                Simulation.Scene.ajouterScenable(PrixTourellesAchat[tourelle.Key.Type]);

                if (TurretToBuy != null && TurretToBuy == tourelle.Key)
                {
                    WidgetSelection.Position = this.Position + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixTourellesAchat[tourelle.Key.Type].Couleur = (tourelle.Value) ? Color.White : Color.Red;

                compteurEmplacement++;
            }
        }


        private void DrawPowerUps()
        {
            Vector3 prochainePositionRelative = new Vector3(0, AvailableTurretsToBuy.Count * DistanceBetweenTwoChoices, 0);

            // Afficher DoItYourself

            if (CelestialBody.PeutAvoirDoItYourself)
            {
                LogoDoItYourSelf.Position = this.Position + prochainePositionRelative + PositionChoice;
                PrixDoItYourself.Position = this.Position + prochainePositionRelative + PositionChoicePrice;
                PrixDoItYourself.Data = CelestialBody.PrixDoItYourself + "M$";

                if (SelectedOption == PowerUp.DoItYourself)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative;
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixDoItYourself.Color = (Options[0]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoDoItYourSelf);
                Simulation.Scene.ajouterScenable(PrixDoItYourself);

                prochainePositionRelative += new Vector3(0, DistanceBetweenTwoChoices, 0);
            }


            // Afficher CollectTheRent

            if (CelestialBody.PeutAvoirCollecteur)
            {
                LogoCollecteur.Position = this.Position + prochainePositionRelative + PositionChoice;
                PrixCollecteur.Position = this.Position + prochainePositionRelative + PositionChoicePrice;
                PrixCollecteur.Data = CelestialBody.PrixCollecteur + "M$";

                if (SelectedOption == PowerUp.CollectTheRent)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative;
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixCollecteur.Color = (Options[PowerUp.CollectTheRent]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoCollecteur);
                Simulation.Scene.ajouterScenable(PrixCollecteur);

                prochainePositionRelative += new Vector3(0, DistanceBetweenTwoChoices, 0);
            }


            // Afficher FinalSolution

            if (CelestialBody.PeutDetruire)
            {
                LogoDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + PositionChoice;
                PrixDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + PositionChoicePrice;
                PrixDestructionCorpsCeleste.Data = CelestialBody.PrixDestruction + "M$";

                if (SelectedOption == PowerUp.FinalSolution)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative;
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixDestructionCorpsCeleste.Color = (Options[PowerUp.FinalSolution]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoDestructionCorpsCeleste);
                Simulation.Scene.ajouterScenable(PrixDestructionCorpsCeleste);

                prochainePositionRelative += new Vector3(0, DistanceBetweenTwoChoices, 0);
            }


            // Afficher TheResistance

            if (CelestialBody.PeutAvoirTheResistance)
            {
                LogoTheResistance.Position = this.Position + prochainePositionRelative + PositionChoice;
                PrixTheResistance.Position = this.Position + prochainePositionRelative + PositionChoicePrice;
                PrixTheResistance.Data = CelestialBody.PrixTheResistance + "M$";

                if (SelectedOption == PowerUp.TheResistance)
                {
                    WidgetSelection.Position = this.Position + prochainePositionRelative;
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixTheResistance.Color = (Options[PowerUp.TheResistance]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoTheResistance);
                Simulation.Scene.ajouterScenable(PrixTheResistance);

                prochainePositionRelative += new Vector3(0, DistanceBetweenTwoChoices, 0);
            }
        }
    }
}
