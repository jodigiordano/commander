namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class MenuTurretSpotEmpty : MenuAbstract
    {
        public Emplacement TurretSpot;
        public Dictionary<Tourelle, bool> AvailableTurretsToBuy;
        public Tourelle TurretToBuy;

        private IVisible WidgetSelection;
        private Dictionary<TypeTourelle, IVisible> LogosTourellesAchat;
        private Dictionary<TypeTourelle, IVisible> PrixTourellesAchat;
        private float PrioriteAffichage;


        public MenuTurretSpotEmpty(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            AvailableTurretsToBuy = new Dictionary<Tourelle, bool>();
            PrioriteAffichage = prioriteAffichage;

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

                IVisible prix = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
                prix.Taille = 2;
                prix.PrioriteAffichage = this.PrioriteAffichage;

                PrixTourellesAchat.Add(t.Type, prix);
            }

            WidgetSelection = new IVisible
            (
                Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"),
                Position
            );
            WidgetSelection.Couleur = new Color(Color.Green, 230);
            WidgetSelection.PrioriteAffichage = this.PrioriteAffichage + 0.01f;
        }


        protected override Vector2 MenuSize
        {
            get
            {
                return (AvailableTurretsToBuy.Count == 0) ? Vector2.Zero : new Vector2(190, 10 + AvailableTurretsToBuy.Count * 40);
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
            if (TurretSpot == null || TurretSpot.EstOccupe || AvailableTurretsToBuy.Count == 0)
                return;

            base.Draw();

            WidgetSelection.TailleVecteur = new Vector2(190, 40);

            int compteurEmplacement = 0;

            foreach (var tourelle in AvailableTurretsToBuy)
            {
                LogosTourellesAchat[tourelle.Key.Type].Position = this.Position + new Vector3(0, 0 + compteurEmplacement * 40, 0);
                PrixTourellesAchat[tourelle.Key.Type].Position = this.Position + new Vector3(70, 10 + compteurEmplacement * 40, 0);
                PrixTourellesAchat[tourelle.Key.Type].Texte = tourelle.Key.PrixAchat + "M$";


                Simulation.Scene.ajouterScenable(LogosTourellesAchat[tourelle.Key.Type]);
                Simulation.Scene.ajouterScenable(PrixTourellesAchat[tourelle.Key.Type]);

                if (TurretToBuy != null && TurretToBuy == tourelle.Key)
                {
                    WidgetSelection.Position = this.Position + new Vector3(0, 4 + compteurEmplacement * 40, 0);
                    Simulation.Scene.ajouterScenable(WidgetSelection);
                }

                PrixTourellesAchat[tourelle.Key.Type].Couleur = (tourelle.Value) ? Color.White : Color.Red;

                compteurEmplacement++;
            }

            if (AvailableTurretsToBuy.Count > 0)
                Bulle.Draw(null);
        }
    }
}
