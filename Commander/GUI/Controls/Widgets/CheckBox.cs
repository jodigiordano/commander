namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class CheckBox : PanelWidget
    {
        public bool Value;

        protected Image Box;
        private Image CheckedRep;
        private Circle BoxCircle;

        private Particle Selection;
        private Text Label;

        public int MinSpaceLabelX;


        public CheckBox() : this("") {}


        public CheckBox(string label)
        {
            Label = new Text(label, @"Pixelite") { SizeX = 2 };

            Box = new Image("WidgetPush") { SizeX = 4, Origin = Vector2.Zero };
            CheckedRep = new Image("WidgetChecked") { SizeX = 4, Origin = Vector2.Zero };

            BoxCircle = new Circle(Vector3.Zero, Box.AbsoluteSize.X / 2);

            MinSpaceLabelX = 50;
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"selectionCorpsCeleste");

            ((CircleEmitter) Selection.Model[0]).Radius = BoxCircle.Radius + 5;
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
                Box.VisualPriority = value;
                CheckedRep.VisualPriority = value - 0.0000001;
                Selection.VisualPriority = value - 0.0000002;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Box.Position;
            }

            set
            {
                Label.Position = value;

                Box.Position = value;
                
                if (Label.AbsoluteSize.X != 0)
                    Box.Position += new Vector3(MathHelper.Max(MinSpaceLabelX, Label.AbsoluteSize.X), 0, 0);

                CheckedRep.Position = Box.Position;

                BoxCircle.Position = Box.Position + new Vector3(Box.AbsoluteSize / 2f, 0);

                // Center text
                Label.Position += new Vector3(0, (Box.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);
            }
        }


        public override byte Alpha
        {
            get { return Box.Alpha; }
            set { Box.Alpha = CheckedRep.Alpha = Label.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3((Label.AbsoluteSize.X != 0 ? MathHelper.Max(MinSpaceLabelX, Label.AbsoluteSize.X) : 0) + Box.AbsoluteSize.X, Box.AbsoluteSize.Y, 0); }
            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, BoxCircle))
            {
                Value = !Value;
                return true;
            }

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, BoxCircle))
            {
                Selection.Trigger(ref BoxCircle.Position);

                Sticky = true;
                return true;
            }


            Sticky = false;
            return false;
        }


        public override void Draw()
        {
            Scene.Add(Label);
            Scene.Add(Box);

            if (Value)
                Scene.Add(CheckedRep);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Label.Alpha = (byte) from;
            Box.Alpha = (byte) from;
            CheckedRep.Alpha = (byte) from;

            Scene.VisualEffects.Add(Label, effect);
            Scene.VisualEffects.Add(Box, effect);
            Scene.VisualEffects.Add(CheckedRep, effect);
        }


        public void SetLabel(string text)
        {
            Label.Data = text;
        }
    }
}
