namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;


    class NumericHorizontalSlider : PanelWidget
    {
        public int Value;
        public int Min;
        public int Max;
        public int Increment;
        
        private Image DecrementRep;
        private Image IncrementRep;
        private Image DecrementRepCannot;
        private Image IncrementRepCannot;
        private Text ValueText;
        private Text Label;

        private Circle DecrementCircle;
        private Circle IncrementCircle;

        public int SpaceForValue;
        public int SpaceForLabel;
        private Particle Selection;

        private Vector3 position;


        public NumericHorizontalSlider(string label, int min, int max, int startingValue, int increment, int spaceForValue, int spaceForLabel)
        {
            Value = startingValue;
            Min = min;
            Max = max;
            Increment = increment;

            DecrementRep = new Image("WidgetNext") { Origin = Vector2.Zero, Effect = SpriteEffects.FlipHorizontally, SizeX = 4 };
            IncrementRep = new Image("WidgetNext") { Origin = Vector2.Zero, SizeX = 4 };

            DecrementRepCannot = new Image("WidgetNextCannot") { Origin = Vector2.Zero, Effect = SpriteEffects.FlipHorizontally, SizeX = 4 };
            IncrementRepCannot = new Image("WidgetNextCannot") { Origin = Vector2.Zero, SizeX = 4 };

            ValueText = new Text(Value.ToString(), @"Pixelite") { SizeX = 2 }.CenterIt();
            Label = new Text(label, @"Pixelite") { SizeX = 2 };

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
                ValueText.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize.X, 0, 0);
                IncrementRep.Position = ValueText.Position + new Vector3(SpaceForValue, 0, 0);

                // Sync circles
                DecrementCircle.Position = DecrementRep.Position + new Vector3(DecrementRep.AbsoluteSize / 2f, 0);
                IncrementCircle.Position = IncrementRep.Position + new Vector3(IncrementRep.AbsoluteSize / 2f, 0);

                // Center text
                Label.Position += new Vector3(0, (DecrementRep.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);
                ValueText.Position += new Vector3(SpaceForValue / 2, DecrementRep.AbsoluteSize.Y / 2, 0);
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
                Label.Alpha = value;
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return new Vector3(SpaceForLabel + IncrementRep.AbsoluteSize.X + DecrementRep.AbsoluteSize.X + SpaceForValue, IncrementRep.AbsoluteSize.Y, 0);
            }

            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, DecrementCircle) && Value > Min)
            {
                Value = Math.Max(Min, Value - Increment);
                Position = Position;
                return true;
            }

            if (Physics.CircleCicleCollision(circle, IncrementCircle) && Value < Max)
            {
                Value = Math.Min(Max, Value + Increment);
                Position = Position;
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
            Scene.Add(Value > Min ? DecrementRep : DecrementRepCannot);
            Scene.Add(Value < Max ? IncrementRep : IncrementRepCannot);
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


        public void SetLabel(string text)
        {
            Label.Data = text;

            Position = position;
        }
    }
}
