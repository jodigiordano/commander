namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TurretMenu : AbstractMenu
    {
        public Turret Turret;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;
        public TurretAction SelectedOption;

        public bool Visible;

        private Image WidgetSelection;
        private Image LogoPrixVente;
        private Text PrixVente;
        private Image LogoPrixMiseAJour;
        private Text PrixMiseAJour;
        private Text NiveauTourelle;

        private double VisualPriority;

        private int MenuWidth;
        private int DistanceBetweenTwoChoices;
        private float TextSize;


        public TurretMenu(Simulator simulator, double visualPriority, Color color)
            : base(simulator, visualPriority, color)
        {
            MenuWidth = 140;
            DistanceBetweenTwoChoices = 30;
            TextSize = 2f;

            VisualPriority = visualPriority;

            LogoPrixVente = new Image("sell");
            LogoPrixVente.VisualPriority = this.VisualPriority;
            LogoPrixVente.SizeX = 0.75f;
            LogoPrixVente.Origin = Vector2.Zero;

            PrixVente = new Text("Pixelite");
            PrixVente.SizeX = TextSize;
            PrixVente.VisualPriority = this.VisualPriority;

            LogoPrixMiseAJour = new Image("upgrade");
            LogoPrixMiseAJour.VisualPriority = this.VisualPriority;
            LogoPrixMiseAJour.SizeX = 0.75f;
            LogoPrixMiseAJour.Origin = Vector2.Zero;

            PrixMiseAJour = new Text("Pixelite");
            PrixMiseAJour.SizeX = TextSize;
            PrixMiseAJour.VisualPriority = this.VisualPriority;

            NiveauTourelle = new Text("Pixelite");
            NiveauTourelle.SizeX = TextSize;
            NiveauTourelle.VisualPriority = this.VisualPriority - 0.0001f;

            WidgetSelection = new Image("PixelBlanc", ActualPosition)
            {
                Color = color,
                Alpha = 230,
                VisualPriority = VisualPriority + 0.01f,
                Size = new Vector2(MenuWidth, DistanceBetweenTwoChoices),
                Origin = Vector2.Zero
            };

            Visible = false;
        }


        protected override Vector3 MenuSize
        {
            get
            {
                if (Turret == null)
                    return Vector3.Zero;

                int nb = AvailableTurretOptions.Count;

                if (!Turret.CanSell)
                    nb--;

                if (!Turret.CanUpdate)
                    nb--;

                return new Vector3(MenuWidth, (nb == 0) ? 0 : nb * DistanceBetweenTwoChoices, 0);
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
            if (!Visible)
                return;

            base.Draw();

            // Afficher le prix de vente
            if (Turret.CanSell)
            {
                LogoPrixVente.Position = this.ActualPosition + (Turret.CanUpdate ? new Vector3(7, DistanceBetweenTwoChoices + 2, 0) : new Vector3(7, 2, 0));
                PrixVente.Position = this.ActualPosition + (Turret.CanUpdate ? new Vector3(60, DistanceBetweenTwoChoices + 2, 0) : new Vector3(60, 2, 0));
                PrixVente.Data = Turret.SellPrice + "M$";

                if (SelectedOption == 0)
                {
                    WidgetSelection.Position = this.ActualPosition + (Turret.CanUpdate ? new Vector3(0, DistanceBetweenTwoChoices, 0) : new Vector3(0, 0, 0));
                    Simulation.Scene.Add(WidgetSelection);
                }

                PrixVente.Color = (AvailableTurretOptions[0]) ? Color.White : Color.Red;

                Simulation.Scene.Add(LogoPrixVente);
                Simulation.Scene.Add(PrixVente);
            }

            // Afficher le prix de mise a jour
            if (Turret.CanUpdate)
            {
                LogoPrixMiseAJour.Position = this.ActualPosition + new Vector3(5, 2, 0);
                NiveauTourelle.Position = this.ActualPosition + new Vector3(20, 2, 0);
                NiveauTourelle.Data = (Turret.Level + 1).ToString();
                PrixMiseAJour.Position = this.ActualPosition + new Vector3(60, 2, 0);
                PrixMiseAJour.Data = Turret.UpdatePrice + "M$";

                if (SelectedOption == TurretAction.Update)
                {
                    WidgetSelection.Position = this.ActualPosition;
                    Simulation.Scene.Add(WidgetSelection);
                }

                PrixMiseAJour.Color = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;
                NiveauTourelle.Color = (AvailableTurretOptions[TurretAction.Update]) ? Color.White : Color.Red;

                Simulation.Scene.Add(LogoPrixMiseAJour);
                Simulation.Scene.Add(NiveauTourelle);
                Simulation.Scene.Add(PrixMiseAJour);
            }

            if (Turret.CanSell || Turret.CanUpdate)
                Bubble.Draw();
        }
    }
}
