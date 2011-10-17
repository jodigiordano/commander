namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;


    class ChoicesHorizontalSlider : PanelWidget
    {       
        private Image DecrementRep;
        private Image IncrementRep;
        private Image DecrementRepCannot;
        private Image IncrementRepCannot;
        private Text ValueText;
        private Text Label;

        private int ChoiceIndex;
        private List<string> Choices;
        private Text LongestChoice;

        private Circle DecrementCircle;
        private Circle IncrementCircle;

        private Particle Selection;
        private Vector3 position;

        private int SpaceForLabel;
        private int SpaceForValue;


        public ChoicesHorizontalSlider(string label, List<string> choices, int spaceForLabel, int spaceForValue, int startingIndex)
        {
            Choices = choices;
            ChoiceIndex = startingIndex;

            DecrementRep = new Image("WidgetNext") { Origin = Vector2.Zero, Effect = SpriteEffects.FlipHorizontally, SizeX = 4 };
            IncrementRep = new Image("WidgetNext") { Origin = Vector2.Zero, SizeX = 4 };
            DecrementRepCannot = new Image("WidgetNextCannot") { Origin = Vector2.Zero, Effect = SpriteEffects.FlipHorizontally, SizeX = 4 };
            IncrementRepCannot = new Image("WidgetNextCannot") { Origin = Vector2.Zero, SizeX = 4 };

            ValueText = new Text(Choices[ChoiceIndex], @"Pixelite") { SizeX = 2 };
            Label = new Text(label, @"Pixelite") { SizeX = 2 };

            LongestChoice = new Text(@"Pixelite") { SizeX = 2 };

            foreach (var choice in Choices)
            {
                if (choice.Length > LongestChoice.Data.Length)
                    LongestChoice.Data = choice;
            }

            DecrementCircle = new Circle(Vector3.Zero, DecrementRep.AbsoluteSize.X / 2);
            IncrementCircle = new Circle(Vector3.Zero, IncrementRep.AbsoluteSize.X / 2);

            SpaceForLabel = spaceForLabel;
            SpaceForValue = spaceForValue;
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"selectionCorpsCeleste");

            ((CircleEmitter) Selection.Model[0]).Radius = DecrementCircle.Radius + 5;
        }


        public override double VisualPriority
        {
            get
            {
                return Label.VisualPriority;
            }

            set
            {
                Label.VisualPriority = value;
                DecrementRep.VisualPriority = value;
                IncrementRep.VisualPriority = value;
                DecrementRepCannot.VisualPriority = value;
                IncrementRepCannot.VisualPriority = value;
                ValueText.VisualPriority = value;
                Selection.VisualPriority = value + 0.0000001;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;

                Label.Position = value;
                DecrementRep.Position = Label.Position + new Vector3(SpaceForLabel, 0, 0);
                IncrementRep.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize.X + SpaceForValue, 0, 0);
                ValueText.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize.X + SpaceForValue / 2, 0, 0);

                // Sync circles
                DecrementCircle.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize / 2f, 0);
                IncrementCircle.Position = IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize / 2f, 0);

                // Center text
                Label.Position += new Vector3(0, (DecrementRep.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);

                ValueText.Position += new Vector3(0, (DecrementRep.AbsoluteSize.Y - ValueText.AbsoluteSize.Y), 0);
                ValueText.CenterIt();

                // Sync cannots
                DecrementRepCannot.Position = DecrementRep.Position;
                IncrementRepCannot.Position = IncrementRep.Position;
            }
        }


        public override byte Alpha
        {
            get { return DecrementRep.Alpha; }
            set
            {
                DecrementRep.Alpha =
                IncrementRep.Alpha =
                DecrementRepCannot.Alpha =
                IncrementRepCannot.Alpha =
                ValueText.Alpha =
                Label.Alpha =
                value;
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize, 0) - Label.Position;
            }


            set
            {

            }
        }


        public string Value
        {
            get { return ValueText.Data; }
            set
            {
                for (int i = 0; i < Choices.Count; i++)
                    if (Choices[i] == value)
                    {
                        ChoiceIndex = i;
                        ValueText.Data = Choices[ChoiceIndex];

                        return;
                    }
            }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, DecrementCircle) && ChoiceIndex > 0)
            {
                ValueText.Data = Choices[--ChoiceIndex];
                return true;
            }

            if (Physics.CircleCicleCollision(circle, IncrementCircle) && ChoiceIndex < Choices.Count - 1)
            {
                ValueText.Data = Choices[++ChoiceIndex];
                return true;
            }

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, DecrementCircle))
            {
                Selection.Trigger(ref DecrementCircle.Position);

                Sticky = true;
                return true;
            }


            if (Physics.CircleCicleCollision(circle, IncrementCircle))
            {
                Selection.Trigger(ref IncrementCircle.Position);

                Sticky = true;
                return true;
            }

            Sticky = false;
            return false;
        }


        public override void Draw()
        {
            ValueText.Data = Value.ToString();

            Scene.Add(Label);
            Scene.Add(ChoiceIndex > 0 ? DecrementRep : DecrementRepCannot);
            Scene.Add(ChoiceIndex < Choices.Count - 1 ? IncrementRep : IncrementRepCannot);
            Scene.Add(ValueText);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Label.Alpha = (byte) from;
            DecrementRep.Alpha = (byte) from;
            IncrementRep.Alpha = (byte) from;
            DecrementRepCannot.Alpha = (byte) from;
            IncrementRepCannot.Alpha = (byte) from;
            ValueText.Alpha = (byte) from;

            Scene.VisualEffects.Add(Label, effect);
            Scene.VisualEffects.Add(DecrementRep, effect);
            Scene.VisualEffects.Add(IncrementRep, effect);
            Scene.VisualEffects.Add(DecrementRepCannot, effect);
            Scene.VisualEffects.Add(IncrementRepCannot, effect);
            Scene.VisualEffects.Add(ValueText, effect);
        }
    }
}
