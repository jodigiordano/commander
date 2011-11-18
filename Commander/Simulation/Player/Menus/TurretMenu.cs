namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TurretMenu : ContextualMenu
    {
        private List<KeyValuePair<string, PanelWidget>> UpgradeTurretHBMessage;
        private List<KeyValuePair<string, PanelWidget>> SellTurretHBMessage;

        private SimPlayer Owner;
        private Turret SelectedTurret;


        public TurretMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 15)
        {
            Owner = owner;
            float textSize = 2f;

            AddChoice(new UpgradeTurretContextualMenuChoice("upgrade",
                    new Text("Pixelite") { SizeX = textSize },
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("upgrade") { SizeX = 3, Origin = Vector2.Zero }));

            AddChoice(new LogoTextContextualMenuChoice("sell",
                    new Text("Pixelite") { SizeX = textSize },
                    new Image("sell") { SizeX = 3, Origin = Vector2.Zero }) { LogoOffet = new Vector3(3, 3, 0), DistanceBetweenNameAndLogo = new Vector2(60, 0) });
            
            InitializeHelpBarMessages(owner.InnerPlayer);
        }


        public TurretChoice Selection
        {
            get { return (TurretChoice) SelectedIndex; }
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.GameMode &&
                    Owner.ActualSelection.Turret != null &&
                    !Owner.ActualSelection.Turret.Disabled &&
                    (Owner.ActualSelection.Turret.CanSell || Owner.ActualSelection.Turret.CanUpdate) &&
                    Owner.ActualSelection.TurretToPlace == null;
            }

            set { base.Visible = value; }
        }


        public override void OnOpen()
        {
            if (SelectedTurret != Owner.ActualSelection.Turret)
                SelectedIndex = 0;

            Owner.ActualSelection.TurretChoice = Selection;
        }


        public override void OnClose()
        {
            Owner.ActualSelection.TurretChoice = TurretChoice.None;
        }


        public override void NextChoice()
        {
            base.NextChoice();

            Owner.ActualSelection.TurretChoice = Selection;
        }


        public override void PreviousChoice()
        {
            base.PreviousChoice();

            Owner.ActualSelection.TurretChoice = Selection;
        }


        public override List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            return Selection == TurretChoice.Sell ? SellTurretHBMessage : UpgradeTurretHBMessage;
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            var sell = (LogoTextContextualMenuChoice) Choices[1];
            sell.Active = Owner.ActualSelection.Turret.CanSell;

            if (sell.Active)
            {
                sell.SetText("+" + Owner.ActualSelection.Turret.SellPrice + "$");

                bool canSell = Owner.AvailableTurretOptions[TurretChoice.Sell];

                sell.SetColor(Owner.InnerPlayer.GetCMColor(canSell, SelectedIndex == 1));
            }


            var upgrade = (UpgradeTurretContextualMenuChoice) Choices[0];
            upgrade.Active = Owner.ActualSelection.Turret.CanUpdate;

            bool max = Owner.ActualSelection.Turret.Level == Simulator.TweakingController.TurretsFactory.TurretsLevels[Owner.ActualSelection.Turret.Type].Count - 1;

            if (upgrade.Active)
            {
                upgrade.SetPrice("-" + Owner.ActualSelection.Turret.UpgradePrice + "$");
                upgrade.SetLevel((Owner.ActualSelection.Turret.Level + 1).ToString());
            }

            else if (max)
            {
                upgrade.SetPrice("MAX");
                upgrade.SetLevel((Owner.ActualSelection.Turret.Level).ToString());
            }

            if (upgrade.Active || max)
            {
                bool canUpgrade = Owner.AvailableTurretOptions[TurretChoice.Update];

                var color = Owner.InnerPlayer.GetCMColor(canUpgrade, SelectedIndex == 0);

                upgrade.SetPriceColor(color);
                upgrade.SetLevelColor(color);
            }

            base.Draw();
        }


        private void InitializeHelpBarMessages(Commander.Player p)
        {
            HBMessageConstructor messageConstructor = new HBMessageConstructor();

            UpgradeTurretHBMessage = messageConstructor.CreateToggleSelectMessage(p.InputType, p.InputConfiguration, "Toggle choices", "Upgrade");
            SellTurretHBMessage = messageConstructor.CreateToggleSelectMessage(p.InputType, p.InputConfiguration, "Toggle choices", "Sell");
        }
    }
}
