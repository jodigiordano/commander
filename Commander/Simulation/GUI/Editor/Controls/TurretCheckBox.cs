namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class TurretCheckBox : CheckBox
    {
        public Turret Turret;

        private Vector3 position;


        public TurretCheckBox(Turret turret)
        {
            Turret = turret;
            Turret.Wander = true;
            Turret.Disabled = false;
            Turret.ShowForm = false;
        }


        public override double VisualPriority
        {
            set
            {
                Turret.VisualPriority = value;

                base.VisualPriority = value;
            }
        }


        public override byte Alpha
        {
            get { return base.Alpha; }
            set
            {
                Turret.Alpha = value;

                base.Alpha = value;
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

                Turret.Position = value + new Vector3(Turret.BaseImage.AbsoluteSize, 0);

                base.Position = Turret.Position + new Vector3(Turret.BaseImage.AbsoluteSize.X + 15, 0, 0);

                Turret.Position += new Vector3(0, Box.AbsoluteSize.Y / 2, 0);
            }
        }


        public override Vector3 Dimension
        {
            get { return Box.Position + base.Dimension - position; }
        }


        public override void Draw()
        {
            Turret.Update();
            Turret.Draw();

            base.Draw();
        }


        public override void Fade(int from, int to, double length)
        {
            Turret.Fade(from, to, length);

            base.Fade(from, to, length);
        }
    }
}
