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
        public virtual bool Visible { get; set; }
        public int Layout;
        public Bubble Bubble { get; private set; }

        protected int SelectedIndex;

        protected Simulator Simulator;
        protected double VisualPriority;

        private Text Title;
        private Image TitleSeparator;

        private Image WidgetSelection;

        private Vector3 Size;

        protected List<ContextualMenuChoice> Choices;

        private int DistanceBetweenTwoChoices;

        private bool ChoiceDataChanged;
        private bool ChoiceAvailabilityChanged;

        private Vector3 ActualPosition;
        private Vector3 Margin;

        private List<ContextualMenuLayout> Layouts;
        private List<ContextualMenuLayout> PossibleLayouts;


        public ContextualMenu(Simulator simulator, double visualPriority, Color color, int distanceBetweenTwoChoices)
            : this(simulator, visualPriority, color, new List<ContextualMenuChoice>(), distanceBetweenTwoChoices)
        {

        }


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
            Layouts = new List<ContextualMenuLayout>()
            {
                new ContextualMenuLayout(0, 0, new Vector2(30, -20)),
                new ContextualMenuLayout(1, 3, new Vector2(0, -20)) { SubstractSizeY = true },
                new ContextualMenuLayout(2, 1, new Vector2(-30, 0)) { SubstractSizeX = true },
                new ContextualMenuLayout(3, 2, new Vector2(-30, -20)) { SubstractSizeX = true, SubstractSizeY = true }
            };

            PossibleLayouts = new List<ContextualMenuLayout>();
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


        public virtual List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            return new List<KeyValuePair<string, PanelWidget>>();
        }


        public ContextualMenuChoice GetChoiceByName(string name)
        {
            foreach (var c in Choices)
                if (c.Name == name)
                    return c;

            return null;
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
            Title = new Text(title, @"Pixelite") { SizeX = 2, VisualPriority = VisualPriority };

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


        public virtual void NextChoice()
        {
            if (Choices.Count == 0)
                return;

            SelectedIndex = (SelectedIndex + 1) % Choices.Count;
        }


        public virtual void PreviousChoice()
        {
            if (Choices.Count == 0)
                return;

            SelectedIndex = SelectedIndex - 1;

            if (SelectedIndex == -1)
                SelectedIndex = Choices.Count - 1;
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


        public virtual void Fade(int from, int to, double length, Core.IntegerHandler callback)
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


        public List<ContextualMenuLayout> GetPossibleLayouts()
        {
            PossibleLayouts.Clear();

            AddPossibleLayout1(0);
            AddPossibleLayout2(1);
            AddPossibleLayout3(2);
            AddPossibleLayout4(3);

            //Put the last used layout as the first choice
            ContextualMenuLayout toMove = PossibleLayouts[0];
            PossibleLayouts[0] = PossibleLayouts[Layout];
            PossibleLayouts[Layout] = toMove;

            return PossibleLayouts;
        }


        public virtual void Initialize()
        {

        }


        public virtual void UpdateSelection()
        {
        
        }


        public virtual void Update()
        {

        }


        private void AddPossibleLayout4(int index)
        {
            Layouts[index].Rectangle.X = (int) (Position.X - Size.X - 40);
            Layouts[index].Rectangle.Y = (int) (Position.Y - Size.Y - 30);
            Layouts[index].Rectangle.Width = (int) Size.X + 40;
            Layouts[index].Rectangle.Height = (int) Size.Y + 30;

            PossibleLayouts.Add(Layouts[index]);
        }


        private void AddPossibleLayout3(int index)
        {
            Layouts[index].Rectangle.X = (int) (Position.X - Size.X - 40);
            Layouts[index].Rectangle.Y = (int) Position.Y - 10;
            Layouts[index].Rectangle.Width = (int) Size.X + 40;
            Layouts[index].Rectangle.Height = (int) Size.Y + 20;

            PossibleLayouts.Add(Simulator.Data.Battlefield.Inner.Includes(Layouts[index].Rectangle) ? Layouts[index] : null);
        }


        private void AddPossibleLayout2(int index)
        {
            Layouts[index].Rectangle.X = (int) Position.X - 10;
            Layouts[index].Rectangle.Y = (int) (Position.Y - Size.Y - 30);
            Layouts[index].Rectangle.Width = (int) Size.X + 20;
            Layouts[index].Rectangle.Height = (int) Size.Y + 40;

            PossibleLayouts.Add(Simulator.Data.Battlefield.Inner.Includes(Layouts[index].Rectangle) ? Layouts[index] : null);
        }


        private void AddPossibleLayout1(int index)
        {
            Layouts[index].Rectangle.X = (int) Position.X;
            Layouts[index].Rectangle.Y = (int) Position.Y - 30;
            Layouts[index].Rectangle.Width = (int) Size.X + 40;
            Layouts[index].Rectangle.Height = (int) Size.Y + 20;

            PossibleLayouts.Add(Simulator.Data.Battlefield.Inner.Includes(Layouts[index].Rectangle) ? Layouts[index] : null);
        }


        private void DrawChoices()
        {
            if (Choices.Count == 0)
                return;

            int slotCounter = 0;

            float distanceY = Choices[0].Size.Y + DistanceBetweenTwoChoices;
            float startingAt = (Title != null) ? Title.AbsoluteSize.Y + 10 : 0;

            foreach (var choice in Choices)
            {
                var position = ActualPosition;
                Vector3.Add(ref position, ref Margin, out position);
                position.Y += startingAt + slotCounter * distanceY;

                choice.Position = position; // +new Vector3(0, (distanceY - Margin.Y * 2 - choice.Size.Y) / 2, 0);

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
                TitleSeparator.Position = ActualPosition + new Vector3(0, Title.AbsoluteSize.Y + 3, 0);

                Simulator.Scene.Add(Title);
                Simulator.Scene.Add(TitleSeparator);
            }
        }


        private void DrawBubble()
        {
            var layout = Layouts[Layout];

            Bubble.BlaPosition = layout.BubbleTailId;
            Bubble.Dimension.X = (int) (Position.X + layout.BubblePositionCorrection.X);
            Bubble.Dimension.Y = (int) (Position.Y + layout.BubblePositionCorrection.Y);
            Bubble.Dimension.Width = (int) Size.X;
            Bubble.Dimension.Height = (int) Size.Y;

            if (layout.SubstractSizeX)
                Bubble.Dimension.X -= (int) Size.X;

            if (layout.SubstractSizeY)
                Bubble.Dimension.Y -= (int) Size.Y;

            ActualPosition.X = Bubble.Dimension.X;
            ActualPosition.Y = Bubble.Dimension.Y;

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

            if (Title != null && Title.AbsoluteSize.X > width)
                width = Title.AbsoluteSize.X;

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
