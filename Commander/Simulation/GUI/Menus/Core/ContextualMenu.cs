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
        public bool Visible;

        protected Simulator Simulator;
        protected double VisualPriority;

        private Text Title;
        private Image TitleSeparator;

        private Image WidgetSelection;

        public Bubble Bubble { get; private set; }
        private Vector3 Size;

        private List<ContextualMenuChoice> Choices;

        private int DistanceBetweenTwoChoices;

        private bool ChoiceDataChanged;
        private bool ChoiceAvailabilityChanged;

        private Vector3 ActualPosition;
        private Vector3 Border;


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

            Choices = choices;

            foreach (var c in choices)
            {
                c.Scene = Simulator.Scene;
                c.VisualPriority = visualPriority;
                c.DataChanged += new NoneHandler(DoChoiceDataChanged);
                c.AvailabilityChanged += new NoneHandler(DoChoiceAvailabilityChanged);

            }

            Border = new Vector3(3, 3, 0);

            ComputeSize();

            ChoiceDataChanged = false;
            ChoiceAvailabilityChanged = false;
        }


        public void AddChoice(ContextualMenuChoice choice)
        {
            //Text textChoice = new Text(choice, "Pixelite") { SizeX = 2, VisualPriority = VisualPriority };
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


        private void DrawChoices()
        {
            int slotCounter = 0;

            float distanceY = Choices[0].Size.Y + DistanceBetweenTwoChoices; //(Title != null) ? (Size.Y - Title.TextSize.Y - 10) / Choices.Count : Size.Y / Choices.Count;
            float startingAt = (Title != null) ? Title.TextSize.Y + 10 : 0;

            foreach (var choice in Choices)
            {
                var position = ActualPosition;
                Vector3.Add(ref position, ref Border, out position);
                position.Y += startingAt + slotCounter * distanceY;

                choice.Position = position;

                choice.Draw();

                if (slotCounter == SelectedIndex)
                {
                    position.X -= Border.X;

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
            bool tropADroite = Position.X + Size.X + 30 > 640 - Preferences.Xbox360DeadZoneV2.X;
            bool tropBas = Position.Y + Size.Y + 20 > 370 - Preferences.Xbox360DeadZoneV2.Y;


            if (tropADroite && tropBas)
            {
                ActualPosition.X -= Size.X + 30;
                ActualPosition.Y -= Size.Y - 20;
                Bubble.BlaPosition = 2;
            }

            else if (tropADroite)
            {
                ActualPosition.X -= Size.X + 30;
                Bubble.BlaPosition = 1;
            }

            else if (tropBas)
            {
                ActualPosition.Y -= Size.Y + 20;
                Bubble.BlaPosition = 3;
            }

            else
            {
                ActualPosition.X += 30;
                ActualPosition.Y -= 20;
                Bubble.BlaPosition = 0;
            }

            Bubble.Dimension = new PhysicalRectangle((int) ActualPosition.X, (int) ActualPosition.Y, (int) Size.X, (int) Size.Y);

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

            width += Border.X * 2;

            Size = new Vector3(
                width,
                (Choices.Count * height) + ((Choices.Count - 1) * DistanceBetweenTwoChoices) + (Title != null ? height + 10 : 0) + (Border.Y * 4),
                0);

            WidgetSelection.Size = new Vector2(width, height + Border.Y * 2);
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
