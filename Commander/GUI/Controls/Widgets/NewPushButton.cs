namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewPushButton : PanelWidget
    {
        protected Image Box;
        private Circle BoxCircle;


        public NewPushButton()
        {
            Box = new Image("checkbox") { SizeX = 4, Origin = Vector2.Zero };

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
                return true;

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            return Physics.CircleCicleCollision(circle, BoxCircle);
        }


        public override void Draw()
        {
            Scene.Add(Box);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Box.Alpha = (byte) from;

            Scene.VisualEffects.Add(Box, effect);
        }
    }
}
