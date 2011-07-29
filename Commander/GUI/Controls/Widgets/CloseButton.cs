namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class CloseButton : PanelWidget
    {
        private Image Button;
        private Circle ButtonCircle;
        private Particle Selection;


        public CloseButton(Scene scene, Vector3 position, double visualPriority)
        {
            Button = new Image("WidgetClose")
            {
                SizeX = 4
            };

            Selection = scene.Particles.Get(@"selectionCorpsCeleste");

            VisualPriority = visualPriority;
            Position = position;

            ButtonCircle = new Circle(position, Button.AbsoluteSize.X / 2);

            ((CircleEmitter) Selection.ParticleEffect[0]).Radius = ButtonCircle.Radius + 5;
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
                Selection.VisualPriority = value + 0.0000002;
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
            }
        }


        protected override bool Click(Circle circle)
        {
            return Physics.CircleCicleCollision(circle, ButtonCircle);
        }


        protected override bool Hover(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, ButtonCircle))
            {
                Selection.Trigger(ref Button.position);
                return true;
            }

            return false;
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Alpha = (byte) from;

            Scene.VisualEffects.Add(this, effect);
        }


        public override void Draw()
        {
            Scene.Add(Button);
        }
    }
}
