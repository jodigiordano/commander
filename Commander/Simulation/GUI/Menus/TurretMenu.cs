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
        
        private Simulator Simulator;

        private List<KeyValuePair<string, PanelWidget>> UpgradeTurretHBMessage;
        private List<KeyValuePair<string, PanelWidget>> SellTurretHBMessage;


        public TurretMenu(Simulator simulator, double visualPriority, Color color)
        {
            Simulator = simulator;

            float textSize = 2f;

            Choices = new List<ContextualMenuChoice>()
            {
                new UpgradeTurretContextualMenuChoice("upgrade",
                    new Text("Pixelite") { SizeX = textSize },
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("upgrade") { SizeX = 0.75f, Origin = Vector2.Zero }),
                new LogoTextContextualMenuChoice("sell",
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("sell") { SizeX = 0.75f, Origin = Vector2.Zero }) { LogoOffet = new Vector3(3, 3, 0), DistanceBetweenNameAndLogo = new Vector2(60, 0) }
            };

            Menu = new ContextualMenu(simulator, visualPriority, color, Choices, 15);

            UpgradeTurretHBMessage = new List<KeyValuePair<string, PanelWidget>>();
            UpgradeTurretHBMessage.AddRange(Simulator.HelpBar.PredefinedMessages[HelpBarMessage.ToggleChoices]);
            UpgradeTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));
            UpgradeTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Upgrade", "Pixelite") { SizeX = 2f })));

            SellTurretHBMessage = new List<KeyValuePair<string, PanelWidget>>();
            SellTurretHBMessage.AddRange(Simulator.HelpBar.PredefinedMessages[HelpBarMessage.ToggleChoices]);
            SellTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));
            SellTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.AlternateSelect]), new Text("Sell", "Pixelite") { SizeX = 2f })));
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


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage(TurretChoice choice)
        {
            return choice == TurretChoice.Sell ? SellTurretHBMessage : UpgradeTurretHBMessage;
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


            var sell = (LogoTextContextualMenuChoice) Choices[1];
            sell.Active = Turret.CanSell;

            if (sell.Active)
            {
                sell.SetText("+" + Turret.SellPrice + "M$");
                sell.SetColor((AvailableTurretOptions[0]) ? Color.White : Color.Red);
            }


            var upgrade = (UpgradeTurretContextualMenuChoice) Choices[0];
            upgrade.Active = Turret.CanUpdate;

            if (upgrade.Active)
            {
                upgrade.SetPrice(" " + Turret.UpdatePrice + "M$");
                upgrade.SetLevel((Turret.Level + 1).ToString());
                upgrade.SetColor((AvailableTurretOptions[TurretChoice.Update]) ? Color.White : Color.Red);
            }

            else if (Turret.Level == Simulator.TweakingController.TurretsFactory.TurretsLevels[Turret.Type].Count - 1)
            {
                upgrade.SetPrice("MAX");
                upgrade.SetLevel((Turret.Level).ToString());
                upgrade.SetColor(Color.Red);
            }

            Menu.SelectedIndex = SelectedOption == TurretChoice.Sell ? 1 : 0;
            Menu.Draw();
        }
    }
}
