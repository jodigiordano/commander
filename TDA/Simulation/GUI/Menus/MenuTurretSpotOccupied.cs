//namespace EphemereGames.Commander
//{
//    using System.Collections.Generic;
//    using EphemereGames.Core.Visuel;
//    using Microsoft.Xna.Framework;
//    using Microsoft.Xna.Framework.Graphics;

//    class MenuTurretSpotOccupied : MenuAbstract
//    {
//        public Emplacement TurretSpot;
//        public Dictionary<TurretAction, bool> AvailableTurretOptions;
//        public TurretAction SelectedOption;

//        private IVisible WidgetSelection;
//        private IVisible LogoPrixVente;
//        private IVisible PrixVente;
//        private IVisible LogoPrixMiseAJour;
//        private IVisible PrixMiseAJour;
//        private IVisible NiveauTourelle;

//        private float PrioriteAffichage;


//        public MenuTurretSpotOccupied(Simulation simulation, float prioriteAffichage)
//            : base(simulation)
//        {
//            PrioriteAffichage = prioriteAffichage;

//            LogoPrixVente = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("sell"), Vector3.Zero);
//            LogoPrixVente.VisualPriority = this.PrioriteAffichage;

//            PrixVente = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixVente.Taille = 2;
//            PrixVente.VisualPriority = this.PrioriteAffichage;

//            LogoPrixMiseAJour = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("upgrade"), Vector3.Zero);
//            LogoPrixMiseAJour.VisualPriority = this.PrioriteAffichage;

//            PrixMiseAJour = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            PrixMiseAJour.Taille = 2;
//            PrixMiseAJour.VisualPriority = this.PrioriteAffichage;

//            NiveauTourelle = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
//            NiveauTourelle.Taille = 2;
//            NiveauTourelle.VisualPriority = this.PrioriteAffichage;

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
//                if (TurretSpot == null || !TurretSpot.EstOccupe)
//                    return Vector2.Zero;

//                int nb = AvailableTurretOptions.Count;

//                if (!TurretSpot.Tourelle.CanSell)
//                    nb--;

//                if (!TurretSpot.Tourelle.CanUpdate)
//                    nb--;

//                return new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
//            }
//        }


//        protected override Vector3 BasePosition
//        {
//            get
//            {
//                return (TurretSpot == null) ? Vector3.Zero : TurretSpot.Position;
//            }
//        }


//        public override void Draw()
//        {
//            if (TurretSpot == null || !TurretSpot.EstOccupe)
//                return;

//            base.Draw();

//            WidgetSelection.TailleVecteur = new Vector2(190, 40);

//            // Afficher le prix de vente
//            if (TurretSpot.Tourelle.CanSell)
//            {
//                LogoPrixVente.Position = this.Position + (TurretSpot.Tourelle.CanUpdate ? new Vector3(10, 50, 0) : new Vector3(10, 10, 0));
//                PrixVente.Position = this.Position + (TurretSpot.Tourelle.CanUpdate ? new Vector3(95, 50, 0) : new Vector3(95, 10, 0));
//                PrixVente.Texte = TurretSpot.Tourelle.SellPrice + "M$";

//                if (SelectedOption == 0)
//                {
//                    WidgetSelection.Position = this.Position + (TurretSpot.Tourelle.CanUpdate ? new Vector3(0, 44, 0) : new Vector3(0, 4, 0));
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixVente.Couleur = (AvailableTurretOptions[0]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoPrixVente);
//                Simulation.Scene.ajouterScenable(PrixVente);
//            }

//            // Afficher le prix de mise a jour
//            if (TurretSpot.Tourelle.CanUpdate)
//            {
//                LogoPrixMiseAJour.Position = this.Position + new Vector3(10, 10, 0);
//                NiveauTourelle.Position = this.Position + new Vector3(40, 10, 0);
//                NiveauTourelle.Texte = (TurretSpot.Tourelle.Level + 1).ToString();
//                PrixMiseAJour.Position = this.Position + new Vector3(95, 10, 0);
//                PrixMiseAJour.Texte = TurretSpot.Tourelle.UpdatePrice + "M$";

//                if (SelectedOption == TurretAction.Update)
//                {
//                    WidgetSelection.Position = this.Position + new Vector3(0, 4, 0);
//                    Simulation.Scene.ajouterScenable(WidgetSelection);
//                }

//                PrixMiseAJour.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;
//                NiveauTourelle.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;

//                Simulation.Scene.ajouterScenable(LogoPrixMiseAJour);
//                Simulation.Scene.ajouterScenable(NiveauTourelle);
//                Simulation.Scene.ajouterScenable(PrixMiseAJour);
//            }

//            if (TurretSpot.Tourelle.CanSell || TurretSpot.Tourelle.CanUpdate)
//                Bulle.Draw(null);
//        }
//    }
//}
