namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public enum MineralType
    {
        Cash10,
        Cash25,
        Cash150,
        Life1
    };


    class MineralsFactory
    {
        private Simulator Simulation;
        private Dictionary<MineralType, MineralDefinition> Minerals;
        private Pool<Mineral> Pool;


        public MineralsFactory(Simulator simulation)
        {
            Simulation = simulation;

            Pool = new Pool<Mineral>();
            Minerals = new Dictionary<MineralType, MineralDefinition>(MineralTypeComparer.Default);

            MineralDefinition m;

            m = new MineralDefinition()
            {
                Type = MineralType.Cash10,
                Texture = "cash10",
                TimeAlive = 12000, 
                Value = 10,
                Radius = 12, 
                ParticulesRepresentation = "mineral1",
                Origin = new Vector2(7, 7)
            };
            Minerals.Add(MineralType.Cash10, m);


            m = new MineralDefinition()
            {
                Type = MineralType.Cash25,
                Texture = "cash25",
                TimeAlive = 8000,
                Value = 25,
                Radius = 14,
                ParticulesRepresentation = "mineral2",
                Origin = new Vector2(0, 10)
            };
            Minerals.Add(MineralType.Cash25, m);


            m = new MineralDefinition()
            {
                Type = MineralType.Cash150,
                Texture = "cash150",
                TimeAlive = 4000,
                Value = 150,
                Radius = 14,
                ParticulesRepresentation = "mineral3",
                Origin = new Vector2(0, 12)
            };
            Minerals.Add(MineralType.Cash150, m);


            m = new MineralDefinition()
            {
                Type = MineralType.Life1,
                Texture = "1up",
                TimeAlive = 6000,
                Value = 1,
                Radius = 14,
                ParticulesRepresentation = "mineralPointsVie",
                Origin = new Vector2(15, -5)
            };
            Minerals.Add(MineralType.Life1, m);
        }


        public Mineral Get(MineralType type, double visualPriority)
        {
            Mineral m = Pool.Get();

            m.Definition = Minerals[type];
            m.VisualPriority = visualPriority;
            m.Scene = Simulation.Scene;

            m.Initialize();

            return m;
        }


        public void Return(Mineral m)
        {
            Pool.Return(m);
        }


        public int GetValue(MineralType type)
        {
            return Minerals[type].Value;
        }
    }
}
