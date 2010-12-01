namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using ProjectMercury.Emitters;

    class MenuEmplacement : DrawableGameComponent
    {
        public Emplacement Emplacement;
        public Vector3 Position;

        public Dictionary<Tourelle, bool> TourellesAchat;
        public Dictionary<int, bool> OptionsTourellesAchetees;
        public Tourelle TourellePourAchatSelectionnee;
        public int OptionPourTourelleAcheteeSelectionne;

        private Scene Scene;
        private Bulle Bulle;

        private IVisible WidgetSelection;
        private IVisible LogoPrixVente;
        private IVisible PrixVente;
        private IVisible LogoPrixMiseAJour;
        private IVisible PrixMiseAJour;
        private IVisible NiveauTourelle;
        private Dictionary<TypeTourelle, IVisible> LogosTourellesAchat;
        private Dictionary<TypeTourelle, IVisible> PrixTourellesAchat;

        private float PrioriteAffichage;


        public MenuEmplacement(
            Simulation simulation,
            Bulle bulle,
            Emplacement emplacement,
            Vector3 positionInitiale,
            Dictionary<Tourelle, bool> tourellesAchat,
            Dictionary<int, bool> optionsTourellesAchetees,
            float prioriteAffichage)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;
            Bulle = bulle;
            Emplacement = emplacement;
            //EtatChange = !emplacement.EstOccupe;
            Position = positionInitiale;
            TourellesAchat = tourellesAchat;
            OptionsTourellesAchetees = optionsTourellesAchetees;
            TourellePourAchatSelectionnee = null;
            PrioriteAffichage = prioriteAffichage;

            //verifierEtat();

            LogoPrixVente = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("sell"), Vector3.Zero, Scene);
            LogoPrixVente.PrioriteAffichage = this.PrioriteAffichage;

            PrixVente = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixVente.Taille = 2;
            PrixVente.PrioriteAffichage = this.PrioriteAffichage;

            LogoPrixMiseAJour = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("upgrade"), Vector3.Zero, Scene);
            LogoPrixMiseAJour.PrioriteAffichage = this.PrioriteAffichage;

            PrixMiseAJour = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            PrixMiseAJour.Taille = 2;
            PrixMiseAJour.PrioriteAffichage = this.PrioriteAffichage;

            NiveauTourelle = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
            NiveauTourelle.Taille = 2;
            NiveauTourelle.PrioriteAffichage = this.PrioriteAffichage;

            LogosTourellesAchat = new Dictionary<TypeTourelle, IVisible>();
            PrixTourellesAchat = new Dictionary<TypeTourelle, IVisible>();

            List<Tourelle> tourellesDisponibles = FactoryTourelles.GetTourellesDisponibles();

            for (int i = 0; i < tourellesDisponibles.Count; i++)
            {
                Tourelle t = tourellesDisponibles[i];

                IVisible iv = (IVisible)t.representationBase.Clone();
                iv.Origine = Vector2.Zero;
                iv.PrioriteAffichage = this.PrioriteAffichage;

                LogosTourellesAchat.Add(t.Type, iv);

                IVisible prix = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Scene);
                prix.Taille = 2;
                prix.PrioriteAffichage = this.PrioriteAffichage;

                PrixTourellesAchat.Add(t.Type, prix);
            }

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
            if (Emplacement.EstOccupe)
            {
                WidgetSelection.TailleVecteur = new Vector2(190, 40);

                // Afficher le prix de vente
                if (Emplacement.Tourelle.PeutVendre)
                {
                    LogoPrixVente.Position = this.Position + (Emplacement.Tourelle.PeutMettreAJour ? new Vector3(10, 50, 0) : new Vector3(10, 10, 0));
                    PrixVente.Position = this.Position + (Emplacement.Tourelle.PeutMettreAJour ? new Vector3(95, 50, 0) : new Vector3(95, 10, 0));
                    PrixVente.Texte = Emplacement.Tourelle.PrixVente + "M$";

                    if (OptionPourTourelleAcheteeSelectionne == 0)
                    {
                        WidgetSelection.Position = this.Position + (Emplacement.Tourelle.PeutMettreAJour ? new Vector3(0, 44, 0) : new Vector3(0, 4, 0));
                        Scene.ajouterScenable(WidgetSelection);
                    }

                    PrixVente.Couleur = (OptionsTourellesAchetees[0]) ? Color.White : Color.Red;

                    Scene.ajouterScenable(LogoPrixVente);
                    Scene.ajouterScenable(PrixVente);
                }

                // Afficher le prix de mise a jour
                if (Emplacement.Tourelle.PeutMettreAJour)
                {
                    LogoPrixMiseAJour.Position = this.Position + new Vector3(10, 10, 0);
                    NiveauTourelle.Position = this.Position + new Vector3(40, 10, 0);
                    NiveauTourelle.Texte = (Emplacement.Tourelle.Niveau + 1).ToString();
                    PrixMiseAJour.Position = this.Position + new Vector3(95, 10, 0);
                    PrixMiseAJour.Texte = Emplacement.Tourelle.PrixMiseAJour + "M$";

                    if (OptionPourTourelleAcheteeSelectionne == 1)
                    {
                        WidgetSelection.Position = this.Position + new Vector3(0, 4, 0);
                        Scene.ajouterScenable(WidgetSelection);
                    }

                    PrixMiseAJour.Couleur = (OptionsTourellesAchetees[1]) ? Color.White : Color.Red;
                    NiveauTourelle.Couleur = (OptionsTourellesAchetees[1]) ? Color.White : Color.Red;

                    Scene.ajouterScenable(LogoPrixMiseAJour);
                    Scene.ajouterScenable(NiveauTourelle);
                    Scene.ajouterScenable(PrixMiseAJour);
                }

                if (Emplacement.Tourelle.PeutVendre || Emplacement.Tourelle.PeutMettreAJour)
                    Bulle.Draw(null);
            }

            else
            {
                WidgetSelection.TailleVecteur = new Vector2(190, 40);

                int compteurEmplacement = 0;

                foreach (var tourelle in TourellesAchat)
                {
                    LogosTourellesAchat[tourelle.Key.Type].Position = this.Position + new Vector3(0, 0 + compteurEmplacement * 40, 0);
                    PrixTourellesAchat[tourelle.Key.Type].Position = this.Position + new Vector3(70, 10 + compteurEmplacement * 40, 0);
                    PrixTourellesAchat[tourelle.Key.Type].Texte = tourelle.Key.PrixAchat + "M$";


                    Scene.ajouterScenable(LogosTourellesAchat[tourelle.Key.Type]);
                    Scene.ajouterScenable(PrixTourellesAchat[tourelle.Key.Type]);

                    if (TourellePourAchatSelectionnee != null && TourellePourAchatSelectionnee == tourelle.Key)
                    {
                        WidgetSelection.Position = this.Position + new Vector3(0, 4 + compteurEmplacement * 40, 0);
                        Scene.ajouterScenable(WidgetSelection);
                    }

                    PrixTourellesAchat[tourelle.Key.Type].Couleur = (tourelle.Value) ? Color.White : Color.Red;

                    compteurEmplacement++;
                }

                if (TourellesAchat.Count > 0)
                    Bulle.Draw(null);
            }
        }
    }
}
