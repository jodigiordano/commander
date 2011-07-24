namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ContextualMenu
    {
        public Vector3 Position;
        public int SelectedIndex;
        public virtual bool Visible { get; set; }
        public int Layout;

        protected Simulator Simulator;
        protected double VisualPriority;

        private Text Title;
        private Image TitleSeparator;

        private Image WidgetSelection;

        public Bubble Bubble { get; private set; }
        private Vector3 Size;

        public List<ContextualMenuChoice> Choices;

        private int DistanceBetweenTwoChoices;

        private bool ChoiceDataChanged;
        private bool ChoiceAvailabilityChanged;

        private Vector3 ActualPosition;
        private Vector3 Margin;

        private List<PhysicalRectangle> Layouts;
        private List<PhysicalRectangle> PossibleLayouts;


        public ContextualMenu(Simulator simulator, double visualPriority, Color color, List<ContextualMenuChoice> choices, int distanceBetweenTwoChoices)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            DistanceBetweenTwoChoices = distanceBetweenTwoChoices;

            SelectedIndex = 0;
            Visible = true;

            Size = Vector3.Zero;

            Bubble = new Bubble(Simulator, new PhysicalRectangle(), visualPriority + 0.00005)
            {
                Color = color
            };

            TitleSeparator = new Image("PixelBlanc")
            {
                Color = color,
                VisualPriority = visualPriority + 0.00001,
                Origin = Vector2.Zero
            };

            WidgetSelection = new Image("PixelBlanc")
            {
                Color = color,
                Alpha = 230,
                VisualPriority = visualPriority + 0.00001,
                Origin = Vector2.Zero
            };

            Margin = new Vector3(3, 3, 0);

            Choices = new List<ContextualMenuChoice>();

            foreach (var c in choices)
                AddChoice(c);

            ChoiceDataChanged = false;
            ChoiceAvailabilityChanged = false;

            Layout = 0;
            Layouts = new List<PhysicalRectangle>() { new PhysicalRectangle(), new PhysicalRectangle(), new PhysicalRectangle(), new PhysicalRectangle() };
            PossibleLayouts = new List<PhysicalRectangle>();
        }


        public void AddChoice(ContextualMenuChoice choice)
        {
            choice.Scene = Simulator.Scene;
            choice.VisualPriority = VisualPriority;
            choice.DataChanged += new NoneHandler(DoChoiceDataChanged);
            choice.AvailabilityChanged += new NoneHandler(DoChoiceAvailabilityChanged);
            
            Choices.Add(choice);

            if (Choices.Count == 1)
                SelectedIndex = 0;

            ComputeSize();
        }


        public int ChoicesCount
        {
            get { return Choices.Count; }
        }


        public void RemoveChoice(ContextualMenuChoice choice)
        {
            Choices.Remove(choice);

            if (Choices.Count == 0)
                SelectedIndex = -1;

            ComputeSize();
        }


        public void Clear()
        {
            Choices.Clear();

            SelectedIndex = -1;

            ComputeSize();
        }


        public ContextualMenuChoice GetChoice(int index)
        {
            return Choices[index];
        }


        public ContextualMenuChoice GetCurrentChoice()
        {
            if (SelectedIndex == -1)
                return null;

            return Choices[SelectedIndex];
        }


        public void SetTitle(string title)
        {
            Title = new Text(title, "Pixelite") { SizeX = 2, VisualPriority = VisualPriority };

            ComputeSize();
        }


        public virtual Color Color
        {
            get
            {
                return TitleSeparator.Color;
            }

            set
            {
                WidgetSelection.Color = value;
                Bubble.Color = value;
                TitleSeparator.Color = value;
            }
        }


        public virtual void Draw()
        {
            if (!Visible)
                return;

            if (ChoiceDataChanged || ChoiceAvailabilityChanged)
                ComputeSize();

            ActualPosition = Position;

            DrawBubble();
            DrawTitle();
            DrawChoices();

            ChoiceDataChanged = false;
            ChoiceAvailabilityChanged = false;
        }


        public virtual void Fade(int from, int to, double length, Core.NoneHandler callback)
        {
            Bubble.Fade(from, to, length, callback);

            var effect = VisualEffects.Fade(from, to, 0, length);

            foreach (var c in Choices)
                c.Fade(effect);

            if (Title != null)
            {
                Simulator.Scene.VisualEffects.Add(Title, effect);
                Simulator.Scene.VisualEffects.Add(TitleSeparator, effect);
            }

            effect = VisualEffects.Fade(Math.Min(from, 230), Math.Min(to, 230), 0, length);

            Simulator.Scene.VisualEffects.Add(WidgetSelection, effect);
        }


        public List<PhysicalRectangle> GetPossibleLayouts()
        {
            PossibleLayouts.Clear();

            Layouts[0].X = (int) Position.X;
            Layouts[0].Y = (int) Position.Y - 30;
            Layouts[0].Width = (int) Size.X + 40;
            Layouts[0].Height = (int) Size.Y + 20;

            PossibleLayouts.Add(Simulator.InnerTerrain.Includes(Layouts[0]) ? Layouts[0] : null);


            Layouts[1].X = (int) Position.X - 10;
            Layouts[1].Y = (int) (Position.Y - Size.Y - 30);
            Layouts[1].Width = (int) Size.X + 20;
            Layouts[1].Height = (int) Size.Y + 40;

            PossibleLayouts.Add(Simulator.InnerTerrain.Includes(Layouts[1]) ? Layouts[1] : null);


            Layouts[2].X = (int) (Position.X - Size.X - 40);
            Layouts[2].Y = (int) Position.Y - 10;
            Layouts[2].Width = (int) Size.X + 40;
            Layouts[2].Height = (int) Size.Y + 20;

            PossibleLayouts.Add(Simulator.InnerTerrain.Includes(Layouts[2]) ? Layouts[2] : null);


            Layouts[3].X = (int) (Position.X - Size.X - 40);
            Layouts[3].Y = (int) (Position.Y - Size.Y - 30);
            Layouts[3].Width = (int) Size.X + 40;
            Layouts[3].Height = (int) Size.Y + 30;

            PossibleLayouts.Add(Layouts[3]);

            return PossibleLayouts;
        }


        private void DrawChoices()
        {
            if (Choices.Count == 0)
                return;

            int slotCounter = 0;

            float distanceY = Choices[0].Size.Y + DistanceBetweenTwoChoices;
            float startingAt = (Title != null) ? Title.TextSize.Y + 10 : 0;

            foreach (var choice in Choices)
            {
                var position = ActualPosition;
                Vector3.Add(ref position, ref Margin, out position);
                position.Y += startingAt + slotCounter * distanceY;

                choice.Position = position;

                choice.Draw();

                if (slotCounter == SelectedIndex)
                {
                    position.X -= Margin.X;

                    WidgetSelection.Position = position;
                    Simulator.Scene.Add(WidgetSelection);
                }

                slotCounter++;
            }
        }


        private void DrawTitle()
        {
            if (Title != null)
            {
                Title.Position = ActualPosition;
                TitleSeparator.Position = ActualPosition + new Vector3(0, Title.TextSize.Y + 3, 0);

                Simulator.Scene.Add(Title);
                Simulator.Scene.Add(TitleSeparator);
            }
        }


        private void DrawBubble()
        {
            switch (Layout)
            {
                case 0:
                    Bubble.BlaPosition = 0;
                    Bubble.Dimension.X = (int) Position.X + 30;
                    Bubble.Dimension.Y = (int) Position.Y - 20;
                    Bubble.Dimension.Width = (int) Size.X;
                    Bubble.Dimension.Height = (int) Size.Y;
                    break;

                case 1:
                    Bubble.BlaPosition = 3;
                    Bubble.Dimension.X = (int) Position.X;
                    Bubble.Dimension.Y = (int) (Position.Y - Size.Y - 20);
                    Bubble.Dimension.Width = (int) Size.X;
                    Bubble.Dimension.Height = (int) Size.Y;
                    break;

                case 2:
                    Bubble.BlaPosition = 1;
                    Bubble.Dimension.X = (int) (Position.X - Size.X - 30);
                    Bubble.Dimension.Y = (int) Position.Y;
                    Bubble.Dimension.Width = (int) Size.X;
                    Bubble.Dimension.Height = (int) Size.Y;
                    break;

                case 3:
                    Bubble.BlaPosition = 2;
                    Bubble.Dimension.X = (int) (Position.X - Size.X - 30);
                    Bubble.Dimension.Y = (int) (Position.Y - Size.Y - 20);
                    Bubble.Dimension.Width = (int) Size.X;
                    Bubble.Dimension.Height = (int) Size.Y;
                    break;
            }

            ActualPosition.X = Bubble.Dimension.X;
            ActualPosition.Y = Bubble.Dimension.Y;

            //if (Simulator.DebugMode)
            //{
            //    Simulator.Scene.Add(new VisualRectangle(Layouts[Layout].RectanglePrimitif, Color.Red));
            //}

            Bubble.Draw();
        }


        private void ComputeSize()
        {
            if (Choices.Count == 0)
            {
                Size = Vector3.Zero;
                WidgetSelection.Size = Vector2.Zero;
                return;
            }

            float width = 0;
            float height = Choices[0].Size.Y;

            foreach (var c in Choices)
                if (c.Size.X > width)
                    width = c.Size.X;

            if (Title != null && Title.TextSize.X > width)
                width = Title.TextSize.X;

            width += Margin.X * 2;

            Size = new Vector3(
                width,
                (Choices.Count * height) + ((Choices.Count - 1) * DistanceBetweenTwoChoices) + (Title != null ? height + 10 : 0) + (Margin.Y * 4),
                0);

            WidgetSelection.Size = new Vector2(width, height + Margin.Y * 2);
            TitleSeparator.Size = new Vector2(width, 5);
        }


        private void DoChoiceDataChanged()
        {
            ChoiceDataChanged = true;
        }


        private void DoChoiceAvailabilityChanged()
        {
            ChoiceAvailabilityChanged = true;
        }
    }
}
