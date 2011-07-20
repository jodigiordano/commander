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

        private List<KeyValuePair<string, PanelWidget>> HBMessage;
        private Label TurretDescription;


        public CelestialBodyMenu(Simulator simulator, double visualPriority, Color color)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            Color = color;

            HBMessage = new List<KeyValuePair<string, PanelWidget>>();
            HBMessage.AddRange(Simulator.HelpBar.PredefinedMessages[HelpBarMessage.ToggleChoices]);
            HBMessage.Add(new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()));
            HBMessage.AddRange(Simulator.HelpBar.PredefinedMessages[HelpBarMessage.Select]);
            HBMessage.Add(new KeyValuePair<string, PanelWidget>("separator2", new VerticalSeparatorWidget()));

            TurretDescription = new Label(new Text("Pixelite") { SizeX = 2f });
            HBMessage.Add(new KeyValuePair<string, PanelWidget>("turretDescription", TurretDescription));
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

                Choices.Add(new LogoTextContextualMenuChoice(
                    new Text(t.BuyPrice + "M$", "Pixelite") { SizeX = 2 },
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


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage(TurretType turretToBuy)
        {
            TurretDescription.SetData(Simulator.TurretsFactory.All[turretToBuy].Description);

            return HBMessage;
        }


        public void Update()
        {
            if (CelestialBody != null)
                Menu.Position = CelestialBody.Position;
        }


        public void Draw()
        {
            if (!Visible)
                return;

            if (CelestialBody == null)
                return;

            int slotCounter = 0;

            foreach (var kvp in AvailableTurrets)
            {
                var turret = (LogoTextContextualMenuChoice) Choices[slotCounter];

                turret.SetColor((kvp.Value) ? Color.White : Color.Red);

                if (TurretToBuy == Simulator.TurretsFactory.Availables[kvp.Key].Type)
                    Menu.SelectedIndex = slotCounter;

                slotCounter++;
            }

            Menu.Draw();
        }
    }
}
