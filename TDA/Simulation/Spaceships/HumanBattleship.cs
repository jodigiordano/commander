﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Physique;


    class HumanBattleship : IObjetPhysique, IPhysicalObject
    {
        public Vector3 Position                 { get; set; }
        public float Speed                      { get; set; }
        public Vector3 Direction                { get; set; }
        public float Rotation                   { get; set; }
        public Shape Shape                      { get; set; }
        public Cercle Circle                    { get; set; }
        public RectanglePhysique Rectangle      { get; set; }
        public Ligne Line                       { get; set; }

        public RailGunTurret RailGun;
        public SniperTurret Sniper;

        private Simulation Simulation;
        private double VisualPriority;
        private Image Image;


        public HumanBattleship(Simulation simulation, Vector3 position, double visualPriority)
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
