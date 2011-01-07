namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class MenuTurret : MenuAbstract
    {
        public Turret Turret;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;
        public TurretAction SelectedOption;

        private IVisible WidgetSelection;
        private IVisible LogoPrixVente;
        private IVisible PrixVente;
        private IVisible LogoPrixMiseAJour;
        private IVisible PrixMiseAJour;
        private IVisible NiveauTourelle;

        private float PrioriteAffichage;

        private int MenuWidth;
        private int DistanceBetweenTwoChoices;
        private float TextSize;


        public MenuTurret(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            MenuWidth = 140;
            DistanceBetweenTwoChoices = 30;
            TextSize = 2f;

            PrioriteAffichage = prioriteAffichage;

            LogoPrixVente = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("sell"), Vector3.Zero);
            LogoPrixVente.VisualPriority = this.PrioriteAffichage;
            LogoPrixVente.Taille = 0.75f;

            PrixVente = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
            PrixVente.Taille = TextSize;
            PrixVente.VisualPriority = this.PrioriteAffichage;

            LogoPrixMiseAJour = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("upgrade"), Vector3.Zero);
            LogoPrixMiseAJour.VisualPriority = this.PrioriteAffichage;
            LogoPrixMiseAJour.Taille = 0.75f;

            PrixMiseAJour = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
            PrixMiseAJour.Taille = TextSize;
            PrixMiseAJour.VisualPriority = this.PrioriteAffichage;

            NiveauTourelle = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
            NiveauTourelle.Taille = TextSize;
            NiveauTourelle.VisualPriority = this.PrioriteAffichage - 0.0001f;

            WidgetSelection = new IVisible
            (
                EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"),
                Position
            );
            WidgetSelection.Couleur = Color.Green;
            WidgetSelection.Couleur.A = 230;
            WidgetSelection.VisualPriority = this.PrioriteAffichage + 0.01f;
            WidgetSelection.TailleVecteur = new Vector2(MenuWidth, DistanceBetweenTwoChoices);
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (Turret == null)
                    return Vector2.Zero;

                int nb = AvailableTurretOptions.Count;

                if (!Turret.CanSell)
                    nb--;

                if (!Turret.CanUpdate)
                    nb--;

                return new Vector2(MenuWidth, (nb == 0) ? 0 : nb * DistanceBetweenTwoChoices);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (Turret == null) ? Vector3.Zero : Turret.Position;
            }
        }


        public override void Draw()
        {
            if (Turret == null)
                return;

            base.Draw();

            // Afficher le prix de vente
            if (Turret.CanSell)
            {
                LogoPrixVente.Position = this.Position + (Turret.CanUpdate ? new Vector3(7, DistanceBetweenTwoChoices + 2, 0) : new Vector3(7, 2, 0));
                PrixVente.Position = this.Position + (Turret.CanUpdate ? new Vector3(60, DistanceBetweenTwoChoices + 2, 0) : new Vector3(60, 2, 0));
                PrixVente.Texte = Turret.SellPrice + "M$";

                if (SelectedOption == 0)
                {
                    WidgetSelection.Position = this.Position + (Turret.CanUpdate ? new Vector3(0, DistanceBetweenTwoChoices, 0) : new Vector3(0, 0, 0));
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixVente.Couleur = (AvailableTurretOptions[0]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoPrixVente);
                Simulation.Scene.ajouterScenable(PrixVente);
            }

            // Afficher le prix de mise a jour
            if (Turret.CanUpdate)
            {
                LogoPrixMiseAJour.Position = this.Position + new Vector3(5, 2, 0);
                NiveauTourelle.Position = this.Position + new Vector3(20, 2, 0);
                NiveauTourelle.Texte = (Turret.Level + 1).ToString();
                PrixMiseAJour.Position = this.Position + new Vector3(60, 2, 0);
                PrixMiseAJour.Texte = Turret.UpdatePrice + "M$";

                if (SelectedOption == TurretAction.Update)
                {
                    WidgetSelection.Position = this.Position;
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixMiseAJour.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;
                NiveauTourelle.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoPrixMiseAJour);
                Simulation.Scene.ajouterScenable(NiveauTourelle);
                Simulation.Scene.ajouterScenable(PrixMiseAJour);
            }

            if (Turret.CanSell || Turret.CanUpdate)
                Bulle.Draw(null);
        }
    }
}
