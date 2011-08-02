namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class CheckBox : PanelWidget
    {
        public bool Checked;

        protected Image Box;
        private Image CheckedRep;
        private Circle BoxCircle;

        private Particle Selection;


        public CheckBox()
        {
            Box = new Image("WidgetPush") { SizeX = 4, Origin = Vector2.Zero };
            CheckedRep = new Image("WidgetChecked") { SizeX = 4, Origin = Vector2.Zero };

            BoxCircle = new Circle(Vector3.Zero, Box.AbsoluteSize.X / 2);
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"selectionCorpsCeleste");

            ((CircleEmitter) Selection.ParticleEffect[0]).Radius = BoxCircle.Radius + 5;
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
                CheckedRep.VisualPriority = value - 0.0000001;
                Selection.VisualPriority = value + 0.0000001;
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
                CheckedRep.Position = value;

                BoxCircle.Position = Box.Position + new Vector3(Box.AbsoluteSize / 2f, 0);
            }
        }


        public override byte Alpha
        {
            get { return Box.Alpha; }
            set { Box.Alpha = CheckedRep.Alpha = value; }
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
