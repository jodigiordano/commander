namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HumanBattleship : IObjetPhysique, IPhysicalObject
    {
        public Vector3 Position                 { get; set; }
        public float Speed                      { get; set; }
        public Vector3 Direction                { get; set; }
        public float Rotation                   { get; set; }
        public Shape Shape                      { get; set; }
        public Circle Circle                    { get; set; }
        public PhysicalRectangle Rectangle      { get; set; }
        public Line Line                       { get; set; }

        public RailGunTurret RailGun;
        public SniperTurret Sniper;

        private Simulator Simulator;
        private double VisualPriority;
        private Image Image;


        public HumanBattleship(Simulator simulator, double visualPriority)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;

            Image = new Image("HumanBattleship")
            {
                VisualPriority = visualPriority,
                SizeX = 4
            };
        }


        public void Initialize()
        {
            Image.Position = Position;
        }


        public void Draw()
        {
            Image.Position = this.Position;

            Simulator.Scene.Add(Image);
        }
    }
}
