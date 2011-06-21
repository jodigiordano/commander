namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    abstract class AbstractMenu
    {
        public Vector3 Position;
        public Bubble Bubble;

        private Vector2 Size;
        protected Simulator Simulation;


        public AbstractMenu(Simulator simulator, double visualPriority, Color color)
        {
            this.Simulation = simulator;

            this.Size = Vector2.Zero;

            Bubble = new Bubble(Simulation, new PhysicalRectangle(), visualPriority + 0.05f)
            {
                Color = color
            };
        }


        public virtual void Draw()
        {
            this.Position = BasePosition;
            this.Size = MenuSize;

            bool tropADroite = Position.X + this.Size.X + 50 > 640 - Preferences.Xbox360DeadZoneV2.X;
            bool tropBas = Position.Y + this.Size.Y > 370 - Preferences.Xbox360DeadZoneV2.Y;

            if (tropADroite && tropBas)
            {
                this.Position += new Vector3(-this.Size.X - 50, -this.Size.Y - 10, 0);
                Bubble.BlaPosition = 2;
            }

            else if (tropADroite)
            {
                this.Position += new Vector3(-this.Size.X - 50, 0, 0);
                Bubble.BlaPosition = 1;
            }

            else if (tropBas)
            {
                this.Position += new Vector3(0, -this.Size.Y - 50, 0);
                Bubble.BlaPosition = 3;
            }

            else
            {
                this.Position += new Vector3(50, -10, 0);
                Bubble.BlaPosition = 0;
            }

            Bubble.Dimension = new PhysicalRectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        protected abstract Vector2 MenuSize { get; }
        protected abstract Vector3 BasePosition { get; }
    }
}
