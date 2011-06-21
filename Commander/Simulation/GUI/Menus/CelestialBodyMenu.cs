namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyMenu : AbstractMenu
    {
        public CelestialBody CelestialBody;

        public Dictionary<TurretType, bool> AvailableTurrets;
        public TurretType TurretToBuy;

        public bool Visible;

        private Dictionary<TurretType, Image> LogosTourellesAchat;
        private Dictionary<TurretType, Text> PrixTourellesAchat;

        private Image WidgetSelection;
        private double VisualPriority;

        private int MenuWidth;
        private int DistanceBetweenTwoChoices;
        private Vector3 PositionChoice;
        private Vector3 PositionChoicePrice;
        private float TextSize;
        private float ImageSize;


        public CelestialBodyMenu(Simulator simulator, double visualPriority, Color color)
            : base(simulator, visualPriority, color)
        {
            MenuWidth = 140;
            DistanceBetweenTwoChoices = 35;
            PositionChoice = new Vector3(20, 15, 0);
            PositionChoicePrice = new Vector3(40, 2, 0);
            TextSize = 2;
            ImageSize = 3;

            VisualPriority = visualPriority;

            WidgetSelection = new Image("PixelBlanc", Position)
            {
                Origin = Vector2.Zero,
                Size = new Vector2(MenuWidth, DistanceBetweenTwoChoices),
                Color = Color.Green,
                Alpha = 230,
                VisualPriority = this.VisualPriority + 0.01f
            };

            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);

            Visible = false;
        }


        public void Initialize()
        {
            LogosTourellesAchat = new Dictionary<TurretType, Image>(TurretTypeComparer.Default);
            PrixTourellesAchat = new Dictionary<TurretType, Text>(TurretTypeComparer.Default);

            Dictionary<TurretType, Turret> tourellesDisponibles = Simulation.TurretsFactory.Availables;

            foreach (var t in tourellesDisponibles.Values)
            {
                Image image = t.BaseImage.Clone();
                image.VisualPriority = this.VisualPriority;
                image.SizeX = ImageSize;
                LogosTourellesAchat.Add(t.Type, image);

                Text prix = new Text("Pixelite");
                prix.SizeX = TextSize;
                prix.VisualPriority = this.VisualPriority;

                PrixTourellesAchat.Add(t.Type, prix);
            }
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (!Visible)
                    return Vector2.Zero;

                return new Vector2(MenuWidth, AvailableTurrets.Count * DistanceBetweenTwoChoices);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position;
            }
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();

            DrawTurretsToBuy();

            Bubble.Draw();
        }


        private void DrawTurretsToBuy()
        {
            int compteurEmplacement = 0;

            foreach (var kvp in AvailableTurrets)
            {
                Turret t = Simulation.TurretsFactory.Availables[kvp.Key];

                LogosTourellesAchat[t.Type].Position = this.Position + PositionChoice + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices + 2, 0);
                PrixTourellesAchat[t.Type].Position = this.Position + PositionChoicePrice + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices + 2, 0);
                PrixTourellesAchat[t.Type].Data = t.BuyPrice + "M$";


                Simulation.Scene.Add(LogosTourellesAchat[t.Type]);
                Simulation.Scene.Add(PrixTourellesAchat[t.Type]);

                if (TurretToBuy != null && TurretToBuy == t.Type)
                {
                    WidgetSelection.Position = this.Position + new Vector3(0, compteurEmplacement * DistanceBetweenTwoChoices, 0);
                    Simulation.Scene.Add(WidgetSelection);
                }

                PrixTourellesAchat[t.Type].Color = (kvp.Value) ? Color.White : Color.Red;

                compteurEmplacement++;
            }
        }
    }
}
