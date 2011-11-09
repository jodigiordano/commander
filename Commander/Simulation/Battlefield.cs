namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class Battlefield
    {
        public PhysicalRectangle Inner;
        public PhysicalRectangle Outer;

        private Simulator Simulator;


        public Battlefield(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize(Level level)
        {
            if (Simulator.EditorEditingMode)
                Inner = new PhysicalRectangle(-5000, -5000, 10000, 10000);
            else
                Inner = level.Descriptor.GetBoundaries(new Vector3(6 * (int) Size.Big));

            Outer = new PhysicalRectangle(Inner.X - 200, Inner.Y - 200, Inner.Width + 400, Inner.Height + 400);
        }


        public void Clamp(ref Vector3 position, float radius)
        {
            position.X = MathHelper.Clamp(position.X, Inner.Left + radius, Inner.Right - radius);
            position.Y = MathHelper.Clamp(position.Y, Inner.Top + radius, Inner.Bottom - radius);
        }


        public Vector3 Clamp(Vector3 position, float radius)
        {
            return new Vector3(
                MathHelper.Clamp(position.X, Inner.Left + radius, Inner.Right - radius),
                MathHelper.Clamp(position.Y, Inner.Top + radius, Inner.Bottom - radius),
                0);
        }
    }
}
