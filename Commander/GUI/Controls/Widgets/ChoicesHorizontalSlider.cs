namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ChoicesHorizontalSlider : PanelWidget
    {
        public string Value { get { return ValueText.Data; } }
        
        private Image DecrementRep;
        private Image IncrementRep;
        private Text ValueText;
        private Text Label;

        private int ChoiceIndex;
        private List<string> Choices;
        private Text LongestChoice;

        private Circle DecrementCircle;
        private Circle IncrementCircle;


        public ChoicesHorizontalSlider(string label, List<string> choices, int startingIndex)
        {
            Choices = choices;
            ChoiceIndex = startingIndex;

            DecrementRep = new Image("Gauche") { Origin = Vector2.Zero };
            IncrementRep = new Image("Droite") { Origin = Vector2.Zero };

            ValueText = new Text(Choices[ChoiceIndex], "Pixelite") { SizeX = 2 };
            Label = new Text(label, "Pixelite") { SizeX = 2 };

            LongestChoice = new Text("Pixelite") { SizeX = 2 };

            foreach (var choice in Choices)
            {
                if (choice.Length > LongestChoice.Data.Length)
                    LongestChoice.Data = choice;
            }

            DecrementCircle = new Circle(Vector3.Zero, 20);
            IncrementCircle = new Circle(Vector3.Zero, 20);
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
                ValueText.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Label.Position;
            }

            set
            {
                Label.Position = value;
                DecrementRep.Position = Label.Position + new Vector3(Label.TextSize.X + 50, 0, 0);
                ValueText.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize.X + 20, 0, 0);
                IncrementRep.Position = ValueText.Position + new Vector3(LongestChoice.TextSize.X + 20, 0, 0);

                DecrementCircle.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize / 2f, 0);
                IncrementCircle.Position = IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize / 2f, 0);
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


        public override void Draw()
        {
            ValueText.Data = Value.ToString();

            Scene.Add(Label);
            Scene.Add(DecrementRep);
            Scene.Add(IncrementRep);
            Scene.Add(ValueText);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Label.Alpha = (byte) from;
            DecrementRep.Alpha = (byte) from;
            IncrementRep.Alpha = (byte) from;
            ValueText.Alpha = (byte) from;

            Scene.VisualEffects.Add(Label, effect);
            Scene.VisualEffects.Add(DecrementRep, effect);
            Scene.VisualEffects.Add(IncrementRep, effect);
            Scene.VisualEffects.Add(ValueText, effect);
        }
    }
}
