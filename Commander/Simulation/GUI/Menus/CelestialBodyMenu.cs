namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyMenu
    {
        public CelestialBody CelestialBody;

        public Dictionary<TurretType, bool> AvailableTurrets;
        public TurretType TurretToBuy;

        public ContextualMenu Menu;

        private List<ContextualMenuChoice> Choices;

        private Simulator Simulator;
        private double VisualPriority;
        private Color Color;

        private List<KeyValuePair<string, PanelWidget>> HBMessageOneTurret;
        private List<KeyValuePair<string, PanelWidget>> HBMessageManyTurrets;
        private Label TurretDescription;

        private bool AlternateSelectedCannotDo;
        private bool AlternateSelectedText;


        public CelestialBodyMenu(Simulator simulator, double visualPriority, Color color, Commander.Player p)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            Color = color;

            AlternateSelectedCannotDo = color == Colors.Spaceship.Pink;
            AlternateSelectedText = color == Colors.Spaceship.Yellow;

            TurretDescription = new Label(new Text(@"Pixelite") { SizeX = 2f });
            
            HBMessageOneTurret = new List<KeyValuePair<string, PanelWidget>>();
            HBMessageOneTurret.AddRange(Simulator.HelpBar.GetPredefinedMessage(p, HelpBarMessage.BuyTurret));
            HBMessageOneTurret.Add(new KeyValuePair<string, PanelWidget>("separator2", new VerticalSeparatorWidget()));
            HBMessageOneTurret.Add(new KeyValuePair<string, PanelWidget>("turretDescription", TurretDescription));

            HBMessageManyTurrets = new List<KeyValuePair<string, PanelWidget>>();
            HBMessageManyTurrets.AddRange(Simulator.HelpBar.GetPredefinedMessage(p, HelpBarMessage.ToggleChoices));
            HBMessageManyTurrets.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));
            HBMessageManyTurrets.AddRange(Simulator.HelpBar.GetPredefinedMessage(p, HelpBarMessage.BuyTurret));
            HBMessageManyTurrets.Add(new KeyValuePair<string, PanelWidget>("separator2", new VerticalSeparatorWidget()));
            HBMessageManyTurrets.Add(new KeyValuePair<string, PanelWidget>("turretDescription", TurretDescription));
        }


        public void Initialize()
        {
            Choices = new List<ContextualMenuChoice>();

            Dictionary<TurretType, Turret> availableTurrets = Simulator.TurretsFactory.Availables;

            foreach (var t in availableTurrets.Values)
            {
                Image image = t.BaseImage.Clone();
                image.SizeX = 3;
                image.Origin = Vector2.Zero;

                Choices.Add(new LogoTextContextualMenuChoice("Buy",
                    new Text(t.BuyPrice + "$", @"Pixelite") { SizeX = 2 },
                    image) { LogoOffet = new Vector3(0, -2, 0) });
            }

            Menu = new ContextualMenu(Simulator, VisualPriority, Color, Choices, 5);
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


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage(TurretType turretToBuy)
        {
            TurretDescription.SetData(Simulator.TurretsFactory.All[turretToBuy].Description);

            return AvailableTurrets.Count > 1 ? HBMessageManyTurrets : HBMessageOneTurret;
        }


        public void Update()
        {
            //if (CelestialBody != null)
            //    Menu.Position = CelestialBody.Position;
        }


        public void Draw()
        {
            if (!Visible)
                return;

            if (CelestialBody == null)
                return;

            if (AvailableTurrets.Count == 0)
                return;

            int slotCounter = 0;

            foreach (var kvp in AvailableTurrets)
            {
                var turret = (LogoTextContextualMenuChoice) Choices[slotCounter];

                bool canBuy = kvp.Value;
                bool selected = TurretToBuy == Simulator.TurretsFactory.Availables[kvp.Key].Type;

                if (!canBuy && AlternateSelectedCannotDo && selected)
                    turret.SetColor(Colors.Spaceship.CannotDo);
                else if (!canBuy)
                    turret.SetColor(Color.Red);
                else if (AlternateSelectedText && selected)
                    turret.SetColor(Colors.Spaceship.Selected);
                else
                    turret.SetColor(Color.White);

                if (selected)
                    Menu.SelectedIndex = slotCounter;

                slotCounter++;
            }

            if (TurretToBuy == TurretType.None)
                Menu.SelectedIndex = -1;

            Menu.Draw();
        }
    }
}
