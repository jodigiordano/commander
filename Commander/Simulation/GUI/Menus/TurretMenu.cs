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

        public ContextualMenu Menu;
        
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


        public void Update()
        {
            if (Turret != null)
                Menu.Position = Turret.Position;
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

            Menu.SelectedIndex = (int) SelectedOption;
            Menu.Draw();
        }
    }
}
