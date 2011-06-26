namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TurretMenu
    {
        public Turret Turret;
        public Dictionary<TurretChoice, bool> AvailableTurretOptions;
        public TurretChoice SelectedOption;

        private ContextualMenu Menu;
        private List<ContextualMenuChoice> Choices;


        public TurretMenu(Simulator simulator, double visualPriority, Color color)
        {
            float textSize = 2f;

            Choices = new List<ContextualMenuChoice>()
            {
                new LogoTextContextualMenuChoice(
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("sell") { SizeX = 0.75f, Origin = Vector2.Zero }) { LogoOffet = new Vector3(3, 3, 0), DistanceBetweenNameAndLogo = new Vector2(60, 0) },
                new UpgradeTurretContextualMenuChoice(
                    new Text("Pixelite") { SizeX = textSize },
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("upgrade") { SizeX = 0.75f, Origin = Vector2.Zero })
            };

            Menu = new ContextualMenu(simulator, visualPriority, color, Choices, 15);
        }


        public bool Visible
        {
            get { return Menu.Visible; }
            set { Menu.Visible = value; }
        }


        public Bubble Bubble
        {
            get { return Menu.Bubble; }
        }


        public void Draw()
        {
            if (!Visible)
                return;

            if (Turret == null || (!Turret.CanSell && Turret.CanUpdate))
                return;


            var sell = (LogoTextContextualMenuChoice) Choices[0];
            sell.Active = Turret.CanSell;

            if (sell.Active)
            {
                sell.SetText(Turret.SellPrice + "M$");
                sell.SetColor((AvailableTurretOptions[0]) ? Color.White : Color.Red);
            }


            var upgrade = (UpgradeTurretContextualMenuChoice) Choices[1];
            upgrade.Active = Turret.CanUpdate;

            if (upgrade.Active)
            {
                upgrade.SetPrice(Turret.UpdatePrice + "M$");
                upgrade.SetLevel((Turret.Level + 1).ToString());
                upgrade.SetColor((AvailableTurretOptions[TurretChoice.Update]) ? Color.White : Color.Red);
            }

            // Afficher le prix de vente
            //if (Turret.CanSell)
            //{
                //LogoPrixVente.Position = this.ActualPosition + (Turret.CanUpdate ? new Vector3(7, DistanceBetweenTwoChoices + 2, 0) : new Vector3(7, 2, 0));
                //PrixVente.Position = this.ActualPosition + (Turret.CanUpdate ? new Vector3(60, DistanceBetweenTwoChoices + 2, 0) : new Vector3(60, 2, 0));
                //PrixVente.Data = Turret.SellPrice + "M$";

                //Simulator.Scene.Add(LogoPrixVente);
                //Simulator.Scene.Add(PrixVente);
            //}

            // Afficher le prix de mise a jour
            //if (Turret.CanUpdate)
            //{
                //LogoPrixMiseAJour.Position = this.ActualPosition + new Vector3(5, 2, 0);
                //NiveauTourelle.Position = this.ActualPosition + new Vector3(20, 2, 0);
                //NiveauTourelle.Data = (Turret.Level + 1).ToString();
                //PrixMiseAJour.Position = this.ActualPosition + new Vector3(60, 2, 0);
                //PrixMiseAJour.Data = Turret.UpdatePrice + "M$";

                //if (SelectedOption == TurretChoice.Update)
                //{
                //    WidgetSelection.Position = this.ActualPosition;
                //    Simulator.Scene.Add(WidgetSelection);
                //}

                //PrixMiseAJour.Color = (AvailableTurretOptions[TurretChoice.Update]) ? Color.White : Color.Red;
                //NiveauTourelle.Color = (AvailableTurretOptions[TurretChoice.Update]) ? Color.White : Color.Red;

                //Simulator.Scene.Add(LogoPrixMiseAJour);
                //Simulator.Scene.Add(NiveauTourelle);
                //Simulator.Scene.Add(PrixMiseAJour);
            //}

            Menu.Position = Turret.Position;
            Menu.SelectedIndex = (int) SelectedOption;
            Menu.Draw();
        }
    }
}
