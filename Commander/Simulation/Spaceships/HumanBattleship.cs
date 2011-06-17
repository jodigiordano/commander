﻿namespace EphemereGames.Commander.Simulation
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
        public RectanglePhysique Rectangle      { get; set; }
        public Line Line                       { get; set; }

        public RailGunTurret RailGun;
        public SniperTurret Sniper;

        private Simulator Simulation;
        private double VisualPriority;
        private Image Image;


        public HumanBattleship(Simulator simulation, Vector3 position, double visualPriority)
        {
            Simulation = simulation;
            VisualPriority = visualPriority;
            Position = position;

            Image = new Image("HumanBattleship")
            {
                VisualPriority = visualPriority,
                SizeX = 4,
                Position = position
            };
        }


        public void Draw()
        {
            Image.Position = this.Position;

            Simulation.Scene.Add(Image);
        }
    }
}
