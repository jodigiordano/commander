//namespace EphemereGames.Commander
//{
//    using System.Collections.Generic;
//    using EphemereGames.Core.Visuel;
//    using Microsoft.Xna.Framework;
//    using Microsoft.Xna.Framework.Graphics;


//    class MenuPowerUps : MenuAbstract
//    {
//        public CorpsCeleste CelestialBody;
//        public Dictionary<PowerUp, bool> Options;
//        public PowerUp SelectedOption;

//        private IVisible WidgetSelection;
//        private IVisible LogoDoItYourSelf;
//        private IVisible PrixDoItYourself;
//        private IVisible LogoCollecteur;
//        private IVisible PrixCollecteur;
//        private IVisible LogoDestructionCorpsCeleste;
//        private IVisible PrixDestructionCorpsCeleste;
//        private IVisible LogoTheResistance;
//        private IVisible PrixTheResistance;
//        private float PrioriteAffichage;


//        public MenuPowerUps(Simulation simulation, float prioriteAffichage)
//            : base(simulation)
//        {
//            PrioriteAffichage = prioriteAffichage;

//            LogoDoItYourSelf = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Vaisseau"), Vector3.Zero);
//            LogoDoItYourSelf.Taille = 4;
//            LogoDoItYourSelf.VisualPriority = this.PrioriteAffichage;

//            PrixDoItYourself = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixDoItYourself.Taille = 2;
//            PrixDoItYourself.VisualPriority = this.PrioriteAffichage;

//            LogoCollecteur = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Collecteur"), Vector3.Zero);
//            LogoCollecteur.Taille = 4;
//            LogoCollecteur.VisualPriority = this.PrioriteAffichage;

//            PrixCollecteur = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixCollecteur.Taille = 2;
//            PrixCollecteur.VisualPriority = this.PrioriteAffichage;

//            LogoDestructionCorpsCeleste = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Destruction"), Vector3.Zero);
//            LogoDestructionCorpsCeleste.Taille = 4;
//            LogoDestructionCorpsCeleste.VisualPriority = this.PrioriteAffichage;

//            PrixDestructionCorpsCeleste = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixDestructionCorpsCeleste.Taille = 2;
//            PrixDestructionCorpsCeleste.VisualPriority = this.PrioriteAffichage;

//            LogoTheResistance = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("TheResistance"), Vector3.Zero);
//            LogoTheResistance.Taille = 4;
//            LogoTheResistance.VisualPriority = this.PrioriteAffichage;

//            PrixTheResistance = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixTheResistance.Taille = 2;
//            PrixTheResistance.VisualPriority = this.PrioriteAffichage;


//            WidgetSelection = new IVisible
//            (
//                EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"),
//                Position
//            );
//            WidgetSelection.Couleur = Color.Green;
//            WidgetSelection.Couleur.A = 230;
//            WidgetSelection.VisualPriority = this.PrioriteAffichage + 0.01f;
//        }


//        protected override Vector2 MenuSize
//        {
//            get
//            {
//                if (CelestialBody == null)
//                    return Vector2.Zero;

//                int nb = Options.Count;

//                if (!CelestialBody.PeutAvoirDoItYourself)
//                    nb--;

//                if (!CelestialBody.PeutDetruire)
//                    nb--;

//                if (!CelestialBody.PeutAvoirCollecteur)
//                    nb--;

//                if (!CelestialBody.PeutAvoirTheResistance)
//                    nb--;

//                return new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
//            }
//        }


//        protected override Vector3 BasePosition
//        {
//            get
//            {
//                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position;
//            }
//        }


//        public override void Draw()
//        {
//            if (CelestialBody == null || SelectedOption == PowerUp.Aucune)
//                return;

//            base.Draw();

//            WidgetSelection.TailleVecteur = new Vector2(190, 40);

//            Vector3 prochainePositionRelative = new Vector3(0, 0, 0);
//            Vector3 margin = new Vector3(10, 10, 0);

//            // Afficher DoItYourself

//            if (CelestialBody.PeutAvoirDoItYourself)
//            {
//                LogoDoItYourSelf.Position = this.Position + prochainePositionRelative + margin;
//                PrixDoItYourself.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
//                PrixDoItYourself.Texte = CelestialBody.PrixDoItYourself + "M$";

//                if (SelectedOption == 0)
//                {
//                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixDoItYourself.Couleur = (Options[0]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoDoItYourSelf);
//                Simulation.Scene.ajouterScenable(PrixDoItYourself);

//                prochainePositionRelative += new Vector3(0, 40, 0);
//            }


//            // Afficher CollectTheRent

//            if (CelestialBody.PeutAvoirCollecteur)
//            {
//                LogoCollecteur.Position = this.Position + prochainePositionRelative + margin;
//                PrixCollecteur.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
//                PrixCollecteur.Texte = CelestialBody.PrixCollecteur + "M$";

//                if (SelectedOption == PowerUp.CollectTheRent)
//                {
//                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixCollecteur.Couleur = (Options[PowerUp.CollectTheRent]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoCollecteur);
//                Simulation.Scene.ajouterScenable(PrixCollecteur);

//                prochainePositionRelative += new Vector3(0, 40, 0);
//            }


//            // Afficher FinalSolution

//            if (CelestialBody.PeutDetruire)
//            {
//                LogoDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + margin;
//                PrixDestructionCorpsCeleste.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
//                PrixDestructionCorpsCeleste.Texte = CelestialBody.PrixDestruction + "M$";

//                if (SelectedOption == PowerUp.FinalSolution)
//                {
//                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixDestructionCorpsCeleste.Couleur = (Options[PowerUp.FinalSolution]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoDestructionCorpsCeleste);
//                Simulation.Scene.ajouterScenable(PrixDestructionCorpsCeleste);

//                prochainePositionRelative += new Vector3(0, 40, 0);
//            }


//            // Afficher TheResistance

//            if (CelestialBody.PeutAvoirTheResistance)
//            {
//                LogoTheResistance.Position = this.Position + prochainePositionRelative + margin;
//                PrixTheResistance.Position = this.Position + prochainePositionRelative + new Vector3(85, 0, 0) + margin;
//                PrixTheResistance.Texte = CelestialBody.PrixTheResistance + "M$";

//                if (SelectedOption == PowerUp.TheResistance)
//                {
//                    WidgetSelection.Position = this.Position + prochainePositionRelative + new Vector3(0, 4, 0);
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixTheResistance.Couleur = (Options[PowerUp.TheResistance]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoTheResistance);
//                Simulation.Scene.ajouterScenable(PrixTheResistance);

//                prochainePositionRelative += new Vector3(0, 40, 0);
//            }

//            if (CelestialBody.PeutAvoirDoItYourself || CelestialBody.PeutDetruire || CelestialBody.PeutAvoirCollecteur)
//                Bulle.Draw(null);
//        }
//    }
//}
