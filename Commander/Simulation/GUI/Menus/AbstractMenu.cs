namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    abstract class AbstractMenu
    {
        public Vector3 ActualPosition;
        public Bubble Bubble;

        private Vector3 Size;
        protected Simulator Simulation;


        public AbstractMenu(Simulator simulator, double visualPriority, Color color)
        {
            Simulation = simulator;

            Size = Vector3.Zero;

            Bubble = new Bubble(Simulation, new PhysicalRectangle(), visualPriority + 0.00005)
            {
                Color = color
            };
        }


        private static Vector3 ScreenBottomRight = new Vector3(640 - Preferences.Xbox360DeadZone.X, 360 - Preferences.Xbox360DeadZone.Y, 0);


        public virtual void Draw()
        {
            ActualPosition = BasePosition;
            Size = MenuSize;

            bool tropADroite = ActualPosition.X + Size.X > 640 - Preferences.Xbox360DeadZoneV2.X;
            bool tropBas = ActualPosition.Y + Size.Y > 370 - Preferences.Xbox360DeadZoneV2.Y;


            if (tropADroite && tropBas)
            {
                ActualPosition.X -= Size.X + 30;
                ActualPosition.Y -= Size.Y - 20;
                Bubble.BlaPosition = 2;
            }

            else if (tropADroite)
            {
                ActualPosition.X -= Size.X + 30;
                Bubble.BlaPosition = 1;
            }

            else if (tropBas)
            {
                ActualPosition.Y -= Size.Y + 20;
                Bubble.BlaPosition = 3;
            }

            else
            {
                ActualPosition.X += 30;
                ActualPosition.Y -= 20;
                Bubble.BlaPosition = 0;
            }

            Bubble.Dimension = new PhysicalRectangle((int)ActualPosition.X, (int)ActualPosition.Y, (int)Size.X, (int)Size.Y);
        }


        protected abstract Vector3 MenuSize { get; }
        protected abstract Vector3 BasePosition { get; }
    }
}
