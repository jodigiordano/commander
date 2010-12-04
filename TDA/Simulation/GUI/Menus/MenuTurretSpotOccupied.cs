namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class MenuTurretSpotOccupied : MenuAbstract
    {
        public Emplacement TurretSpot;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;
        public TurretAction SelectedOption;

        private IVisible WidgetSelection;
        private IVisible LogoPrixVente;
        private IVisible PrixVente;
        private IVisible LogoPrixMiseAJour;
        private IVisible PrixMiseAJour;
        private IVisible NiveauTourelle;

        private float PrioriteAffichage;


        public MenuTurretSpotOccupied(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            PrioriteAffichage = prioriteAffichage;

            LogoPrixVente = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("sell"), Vector3.Zero, Simulation.Scene);
            LogoPrixVente.PrioriteAffichage = this.PrioriteAffichage;

            PrixVente = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            PrixVente.Taille = 2;
            PrixVente.PrioriteAffichage = this.PrioriteAffichage;

            LogoPrixMiseAJour = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("upgrade"), Vector3.Zero, Simulation.Scene);
            LogoPrixMiseAJour.PrioriteAffichage = this.PrioriteAffichage;

            PrixMiseAJour = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            PrixMiseAJour.Taille = 2;
            PrixMiseAJour.PrioriteAffichage = this.PrioriteAffichage;

            NiveauTourelle = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            NiveauTourelle.Taille = 2;
            NiveauTourelle.PrioriteAffichage = this.PrioriteAffichage;

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
                if (TurretSpot == null || !TurretSpot.EstOccupe)
                    return Vector2.Zero;

                int nb = AvailableTurretOptions.Count;

                if (!TurretSpot.Tourelle.PeutVendre)
                    nb--;

                if (!TurretSpot.Tourelle.PeutMettreAJour)
                    nb--;

                return new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (TurretSpot == null) ? Vector3.Zero : TurretSpot.Position;
            }
        }


        public override void Draw()
        {
            if (TurretSpot == null || !TurretSpot.EstOccupe)
                return;

            base.Draw();

            WidgetSelection.TailleVecteur = new Vector2(190, 40);

            // Afficher le prix de vente
            if (TurretSpot.Tourelle.PeutVendre)
            {
                LogoPrixVente.Position = this.Position + (TurretSpot.Tourelle.PeutMettreAJour ? new Vector3(10, 50, 0) : new Vector3(10, 10, 0));
                PrixVente.Position = this.Position + (TurretSpot.Tourelle.PeutMettreAJour ? new Vector3(95, 50, 0) : new Vector3(95, 10, 0));
                PrixVente.Texte = TurretSpot.Tourelle.PrixVente + "M$";

                if (SelectedOption == 0)
                {
                    WidgetSelection.Position = this.Position + (TurretSpot.Tourelle.PeutMettreAJour ? new Vector3(0, 44, 0) : new Vector3(0, 4, 0));
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixVente.Couleur = (AvailableTurretOptions[0]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoPrixVente);
                Simulation.Scene.ajouterScenable(PrixVente);
            }

            // Afficher le prix de mise a jour
            if (TurretSpot.Tourelle.PeutMettreAJour)
            {
                LogoPrixMiseAJour.Position = this.Position + new Vector3(10, 10, 0);
                NiveauTourelle.Position = this.Position + new Vector3(40, 10, 0);
                NiveauTourelle.Texte = (TurretSpot.Tourelle.Niveau + 1).ToString();
                PrixMiseAJour.Position = this.Position + new Vector3(95, 10, 0);
                PrixMiseAJour.Texte = TurretSpot.Tourelle.PrixMiseAJour + "M$";

                if (SelectedOption == TurretAction.Update)
                {
                    WidgetSelection.Position = this.Position + new Vector3(0, 4, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixMiseAJour.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;
                NiveauTourelle.Couleur = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;

                Simulation.Scene.ajouterScenable(LogoPrixMiseAJour);
                Simulation.Scene.ajouterScenable(NiveauTourelle);
                Simulation.Scene.ajouterScenable(PrixMiseAJour);
            }

            if (TurretSpot.Tourelle.PeutVendre || TurretSpot.Tourelle.PeutMettreAJour)
                Bulle.Draw(null);
        }
    }
}
