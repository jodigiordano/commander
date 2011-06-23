namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextMenu : AbstractMenu
    {
        public Vector3 Position;
        public int SelectedIndex;
        public bool Visible;

        private Image WidgetSelection;
        private List<Text> Choices;

        private Vector3 menuSize;
        private int DistanceBetweenTwoChoices;


        public TextMenu(Simulator simulator, List<string> choices, double visualPriority, Color color)
            : base(simulator, visualPriority, color)
        {
            DistanceBetweenTwoChoices = 15;

            WidgetSelection = new Image("PixelBlanc")
            {
                Color = color,
                Alpha = 230,
                VisualPriority = visualPriority + 0.00001,
                Origin = Vector2.Zero
            };

            SelectedIndex = 0;
            Visible = true;

            Choices = new List<Text>();

            foreach (var c in choices)
                Choices.Add(new Text(c, "Pixelite") { SizeX = 2, VisualPriority = visualPriority });

            ComputeSize();
        }


        public void AddChoice(string choice)
        {
            Text textChoice = new Text(choice, "Pixelite") { SizeX = 2 };
            Choices.Add(textChoice);

            if (Choices.Count == 1)
                SelectedIndex = 0;

            ComputeSize();
        }


        public int ChoicesCount
        {
            get { return Choices.Count;
        }


        public void RemoveChoice(string choice)
        {
            for (int i = Choices.Count - 1; i > -1; i--)
            {
                if (Choices[i].Data == choice)
                {
                    Choices.RemoveAt(i);
                    break;
                }
            }

            if (Choices.Count == 0)
                SelectedIndex = -1;

            ComputeSize();
        }


        protected override Vector3 MenuSize
        {
            get { return menuSize; }
        }


        protected override Vector3 BasePosition
        {
            get { return Position; }
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();
            Bubble.Draw();

            int slotCounter = 0;

            float distanceY = MenuSize.Y / Choices.Count;

            foreach (var choice in Choices)
            {
                choice.Position = ActualPosition + new Vector3(0, slotCounter * distanceY, 0);

                Simulation.Scene.Add(choice);

                if (slotCounter == SelectedIndex)
                {
                    WidgetSelection.Position = choice.Position;
                    Simulation.Scene.Add(choice);
                }

                slotCounter++;
            }
        }


        private void ComputeSize()
        {
            if (Choices.Count == 0)
            {
                menuSize = Vector3.Zero;
                WidgetSelection.Size = Vector2.Zero;
                return;
            }

            float width = 0;
            float height = Choices[0].TextSize.Y;

            foreach (var c in Choices)
                if (c.TextSize.X > width)
                    width = c.TextSize.X;

            menuSize = new Vector3(width, Choices.Count * height + (Choices.Count - 1) * DistanceBetweenTwoChoices, 0);

            WidgetSelection.Size = new Vector2(width, height);
        }
    }
}
