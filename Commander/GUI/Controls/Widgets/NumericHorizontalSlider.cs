namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NumericHorizontalSlider : PanelWidget
    {
        public int Value;
        public int Min;
        public int Max;
        public int Increment;
        
        private Image DecrementRep;
        private Image IncrementRep;
        private Text ValueText;
        private Text Label;

        private Circle DecrementCircle;
        private Circle IncrementCircle;

        private int MinimalSpaceForValue;


        public NumericHorizontalSlider(string label, int min, int max, int startingValue, int increment, int minimalSpaceForValue)
        {
            Value = startingValue;
            Min = min;
            Max = max;
            Increment = increment;

            DecrementRep = new Image("Gauche") { Origin = Vector2.Zero };
            IncrementRep = new Image("Droite") { Origin = Vector2.Zero };

            ValueText = new Text(Value.ToString(), "Pixelite") { SizeX = 2 };
            Label = new Text(label, "Pixelite") { SizeX = 2 };

            DecrementCircle = new Circle(Vector3.Zero, 20);
            IncrementCircle = new Circle(Vector3.Zero, 20);

            MinimalSpaceForValue = minimalSpaceForValue;
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
                IncrementRep.Position = ValueText.Position + new Vector3(Math.Max(MinimalSpaceForValue, ValueText.TextSize.X) + 20, 0, 0);

                DecrementCircle.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize / 2f, 0);
                IncrementCircle.Position = IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize / 2f, 0);
            }
        }


        public override byte Alpha
        {
            get { return DecrementRep.Alpha; }
            set { DecrementRep.Alpha = IncrementRep.Alpha = ValueText.Alpha = Label.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get
            {
                return IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize, 0) - Label.Position;
            }

            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, DecrementCircle) && Value > Min)
            {
                Value = Math.Max(Min, Value - Increment);
                return true;
            }

            if (Physics.CircleCicleCollision(circle, IncrementCircle) && Value < Max)
            {
                Value = Math.Min(Max, Value + Increment);
                return true;
            }

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            return Physics.CircleCicleCollision(circle, DecrementCircle) || Physics.CircleCicleCollision(circle, IncrementCircle);
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


        public void SetLabel(string text)
        {
            Label.Data = text;
        }
    }
}
