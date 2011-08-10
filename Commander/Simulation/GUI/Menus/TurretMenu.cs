﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
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


        public TurretMenu(Simulator simulator, double visualPriority, Color color, InputType inputType)
        {
            Simulator = simulator;

            float textSize = 2f;

            Choices = new List<ContextualMenuChoice>()
            {
                new UpgradeTurretContextualMenuChoice("upgrade",
                    new Text("Pixelite") { SizeX = textSize },
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("upgrade") { SizeX = 3, Origin = Vector2.Zero }),
                new LogoTextContextualMenuChoice("sell",
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("sell") { SizeX = 3, Origin = Vector2.Zero }) { LogoOffet = new Vector3(3, 3, 0), DistanceBetweenNameAndLogo = new Vector2(60, 0) }
            };

            Menu = new ContextualMenu(simulator, visualPriority, color, Choices, 15);


            InitializeHelpBarMessages(inputType);
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


        public Vector3 Position
        {
            set { Menu.Position = value; }
        }


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage(TurretChoice choice)
        {
            return choice == TurretChoice.Sell ? SellTurretHBMessage : UpgradeTurretHBMessage;
        }


        public void Update()
        {
            //if (Turret != null)
            //    Menu.Position = Turret.Position;
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
                upgrade.SetPrice("-" + Turret.UpdatePrice + "M$");
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


        private void InitializeHelpBarMessages(InputType inputType)
        {
            UpgradeTurretHBMessage = new List<KeyValuePair<string, PanelWidget>>();
            SellTurretHBMessage = new List<KeyValuePair<string, PanelWidget>>();

            UpgradeTurretHBMessage.AddRange(Simulator.HelpBar.GetPredefinedMessage(HelpBarMessage.ToggleChoices, inputType));
            UpgradeTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));

            SellTurretHBMessage.AddRange(Simulator.HelpBar.GetPredefinedMessage(HelpBarMessage.ToggleChoices, inputType));
            SellTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));

            if (inputType == InputType.Gamepad)
            {
                UpgradeTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Upgrade", "Pixelite") { SizeX = 2f })));
                SellTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Sell", "Pixelite") { SizeX = 2f })));
            }

            else
            {
                UpgradeTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Upgrade", "Pixelite") { SizeX = 2f })));
                SellTurretHBMessage.Add(new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Sell", "Pixelite") { SizeX = 2f })));
            }
        }
    }
}
