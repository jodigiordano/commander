namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TurretMenu : AbstractMenu
    {
        public Turret Turret;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;
        public TurretAction SelectedOption;

        private Image WidgetSelection;
        private Image LogoPrixVente;
        private Text PrixVente;
        private Image LogoPrixMiseAJour;
        private Text PrixMiseAJour;
        private Text NiveauTourelle;

        private float PrioriteAffichage;

        private int MenuWidth;
        private int DistanceBetweenTwoChoices;
        private float TextSize;


        public TurretMenu(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            MenuWidth = 140;
            DistanceBetweenTwoChoices = 30;
            TextSize = 2f;

            PrioriteAffichage = prioriteAffichage;

            LogoPrixVente = new Image("sell");
            LogoPrixVente.VisualPriority = this.PrioriteAffichage;
            LogoPrixVente.SizeX = 0.75f;
            LogoPrixVente.Origin = Vector2.Zero;

            PrixVente = new Text("Pixelite");
            PrixVente.SizeX = TextSize;
            PrixVente.VisualPriority = this.PrioriteAffichage;

            LogoPrixMiseAJour = new Image("upgrade");
            LogoPrixMiseAJour.VisualPriority = this.PrioriteAffichage;
            LogoPrixMiseAJour.SizeX = 0.75f;
            LogoPrixMiseAJour.Origin = Vector2.Zero;

            PrixMiseAJour = new Text("Pixelite");
            PrixMiseAJour.SizeX = TextSize;
            PrixMiseAJour.VisualPriority = this.PrioriteAffichage;

            NiveauTourelle = new Text("Pixelite");
            NiveauTourelle.SizeX = TextSize;
            NiveauTourelle.VisualPriority = this.PrioriteAffichage - 0.0001f;

            WidgetSelection = new Image
            (
                "PixelBlanc",
                Position
            );
            WidgetSelection.Color = Color.Green;
            WidgetSelection.Color.A = 230;
            WidgetSelection.VisualPriority = this.PrioriteAffichage + 0.01f;
            WidgetSelection.Size = new Vector2(MenuWidth, DistanceBetweenTwoChoices);
            WidgetSelection.Origin = Vector2.Zero;
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
            if (Turret == null || Turret.Disabled)
                return;

            base.Draw();

            // Afficher le prix de vente
            if (Turret.CanSell)
            {
                LogoPrixVente.Position = this.Position + (Turret.CanUpdate ? new Vector3(7, DistanceBetweenTwoChoices + 2, 0) : new Vector3(7, 2, 0));
                PrixVente.Position = this.Position + (Turret.CanUpdate ? new Vector3(60, DistanceBetweenTwoChoices + 2, 0) : new Vector3(60, 2, 0));
                PrixVente.Data = Turret.SellPrice + "M$";

                if (SelectedOption == 0)
                {
                    WidgetSelection.Position = this.Position + (Turret.CanUpdate ? new Vector3(0, DistanceBetweenTwoChoices, 0) : new Vector3(0, 0, 0));
                    Simulation.Scene.Add(WidgetSelection);
                }

                PrixVente.Color = (AvailableTurretOptions[0]) ? Color.White : Color.Red;

                Simulation.Scene.Add(LogoPrixVente);
                Simulation.Scene.Add(PrixVente);
            }

            // Afficher le prix de mise a jour
            if (Turret.CanUpdate)
            {
                LogoPrixMiseAJour.Position = this.Position + new Vector3(5, 2, 0);
                NiveauTourelle.Position = this.Position + new Vector3(20, 2, 0);
                NiveauTourelle.Data = (Turret.Level + 1).ToString();
                PrixMiseAJour.Position = this.Position + new Vector3(60, 2, 0);
                PrixMiseAJour.Data = Turret.UpdatePrice + "M$";

                if (SelectedOption == TurretAction.Update)
                {
                    WidgetSelection.Position = this.Position;
                    Simulation.Scene.Add(WidgetSelection);
                }

                PrixMiseAJour.Color = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;
                NiveauTourelle.Color = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;

                Simulation.Scene.Add(LogoPrixMiseAJour);
                Simulation.Scene.Add(NiveauTourelle);
                Simulation.Scene.Add(PrixMiseAJour);
            }

            if (Turret.CanSell || Turret.CanUpdate)
                Bulle.Draw();
        }
    }
}
