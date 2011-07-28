namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CloseButton : PanelWidget
    {
        private Image Button;
        private Text ButtonX;
        private Circle ButtonCircle;


        public CloseButton(Vector3 position, double visualPriority)
        {
            Button = new Image("checkbox")
            {
                SizeX = 3
            };

            ButtonX = new Text("X", "Pixelite")
            {
                SizeX = 2
            }.CenterIt();

            VisualPriority = visualPriority;
            Position = position;

            ButtonCircle = new Circle(position, 8);
        }


        public override double VisualPriority
        {
            get
            {
                return Button.VisualPriority;
            }
            set
            {
                Button.VisualPriority = value + 0.0000001;
                ButtonX.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Button.Position;
            }
            set
            {
                Button.Position = value;
                ButtonX.Position = value;
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return new Vector3(Button.AbsoluteSize, 0);
            }
            set
            {

            }
        }


        public override byte Alpha
        {
            get
            {
                return Button.Alpha;
            }
            set
            {
                Button.Alpha = value;
                ButtonX.Alpha = value;
            }
        }


        protected override bool Click(Circle circle)
        {
            return Physics.CircleCicleCollision(circle, ButtonCircle);
        }


        protected override bool Hover(Circle circle)
        {
            return Physics.CircleCicleCollision(circle, ButtonCircle);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Button.Alpha = (byte) from;
            ButtonX.Alpha = (byte) from;

            Scene.VisualEffects.Add(Button, effect);
            Scene.VisualEffects.Add(ButtonX, effect);
        }


        public override void Draw()
        {
            Scene.Add(Button);
            Scene.Add(ButtonX);
        }
    }
}
