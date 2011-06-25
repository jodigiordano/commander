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

        private ContextualMenu Menu;
        private List<ContextualMenuChoice> Choices;

        private Simulator Simulator;
        private double VisualPriority;
        private Color Color;


        public CelestialBodyMenu(Simulator simulator, double visualPriority, Color color)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            Color = color;
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


            Menu.Position = CelestialBody.Position;
            Menu.Draw();
        }
    }
}
