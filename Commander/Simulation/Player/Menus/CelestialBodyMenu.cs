namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyMenu : ContextualMenu
    {
        public Dictionary<TurretType, bool> AvailableTurrets;

        private List<KeyValuePair<string, PanelWidget>> HBMessageOneTurret;
        private List<KeyValuePair<string, PanelWidget>> HBMessageManyTurrets;
        private KeyValuePair<string, PanelWidget> TurretDescription;

        private List<TurretType> AvailableTurretsList;

        private SimPlayer Owner;


        public CelestialBodyMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 5)
        {
            Owner = owner;

            HBMessageConstructor messageConstructor = new HBMessageConstructor();

            TurretDescription = messageConstructor.CreateLabel(0, "");

            HBMessageOneTurret = new List<KeyValuePair<string, PanelWidget>>();
            HBMessageOneTurret.AddRange(Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.BuyTurret));
            HBMessageOneTurret.Add(messageConstructor.CreateSeparator(0));
            HBMessageOneTurret.Add(TurretDescription);

            HBMessageManyTurrets = new List<KeyValuePair<string, PanelWidget>>();
            HBMessageManyTurrets.AddRange(Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.ToggleChoices));
            HBMessageManyTurrets.Add(messageConstructor.CreateSeparator(0));
            HBMessageManyTurrets.AddRange(Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.BuyTurret));
            HBMessageManyTurrets.Add(messageConstructor.CreateSeparator(1));
            HBMessageManyTurrets.Add(TurretDescription);
        }


        public override void Initialize()
        {
            AvailableTurretsList = new List<TurretType>();

            foreach (var t in Simulator.Data.Level.AvailableTurrets.Values)
            {
                Image image = t.BaseImage.Clone();
                image.SizeX = 3;
                image.Origin = Vector2.Zero;

                AddChoice(new LogoTextContextualMenuChoice("Buy",
                    new Text(t.BuyPrice + "$", @"Pixelite") { SizeX = 2 },
                    image) { LogoOffet = new Vector3(0, -2, 0) });

                AvailableTurretsList.Add(t.Type);
            }
        }


        public override void NextChoice()
        {
            base.NextChoice();

            Owner.ActualSelection.TurretToBuy = Selection;
        }


        public override void PreviousChoice()
        {
            base.PreviousChoice();

            Owner.ActualSelection.TurretToBuy = Selection;
        }


        public TurretType Selection
        {
            get
            {
                return AvailableTurretsList.Count == 0 ? TurretType.None : AvailableTurretsList[SelectedIndex];
            }
        }


        public override List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            ((Label) TurretDescription.Value).Value = Simulator.TurretsFactory.All[Selection].Description;

            return AvailableTurretsList.Count > 1 ? HBMessageManyTurrets : HBMessageOneTurret;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    (Simulator.GameMode || Simulator.EditorPlaytestingMode) &&
                    Owner.ActualSelection.CelestialBody != null &&
                    AvailableTurretsList.Count != 0 &&
                    Owner.ActualSelection.Turret == null &&
                    Owner.ActualSelection.TurretToPlace == null;
            }
            set { base.Visible = value; }
        }


        public override void UpdateSelection()
        {
            Owner.ActualSelection.TurretToBuy = Visible ? Selection : TurretType.None;
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            for (int i = 0; i < AvailableTurretsList.Count; i++)
            {
                var turret = (LogoTextContextualMenuChoice) Choices[i];

                bool canBuy = AvailableTurrets[AvailableTurretsList[i]];
                bool selected = i == SelectedIndex;

                turret.SetColor(Owner.InnerPlayer.GetCMColor(canBuy, selected));
            }

            base.Draw();
        }
    }
}
