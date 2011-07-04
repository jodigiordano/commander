namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewCheckBox : PanelWidget
    {
        public bool Checked;

        protected Image Box;
        private Text CheckedRep;
        private Circle BoxCircle;


        public NewCheckBox()
        {
            Box = new Image("checkbox") { SizeX = 4, Origin = Vector2.Zero };
            CheckedRep = new Text("X", "Pixelite") { SizeX = 3 }.CenterIt();

            BoxCircle = new Circle(Vector3.Zero, 20);
        }


        public override double VisualPriority
        {
            get
            {
                return Box.VisualPriority;
            }

            set
            {
                Box.VisualPriority = value;
                CheckedRep.VisualPriority = value;
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
                Box.Position = value;

                BoxCircle.Position = Box.Position + new Vector3(Box.AbsoluteSize / 2f, 0);

                CheckedRep.Position = BoxCircle.Position;
            }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3(Box.AbsoluteSize, 0); }
            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, BoxCircle))
            {
                Checked = !Checked;
                return true;
            }

            return false;
        }


        public override void Draw()
        {
            Scene.Add(Box);

            if (Checked)
                Scene.Add(CheckedRep);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Box.Alpha = (byte) from;
            CheckedRep.Alpha = (byte) from;

            Scene.VisualEffects.Add(Box, effect);
            Scene.VisualEffects.Add(CheckedRep, effect);
        }
    }
}
