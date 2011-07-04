namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class TurretCheckBox : NewCheckBox
    {
        public Turret Turret;


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


        public override Vector3 Position
        {
            set
            {
                Turret.Position = value + new Vector3(Turret.BaseImage.AbsoluteSize, 0);

                base.Position = Turret.Position + new Vector3(Turret.BaseImage.AbsoluteSize.X + 30, 0, 0);
            }
        }


        public override Vector3 Dimension
        {
            get { return Box.Position + base.Dimension - Turret.Position; }
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
