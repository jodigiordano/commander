namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class CelestialBodyExplosion
    {
        public Dictionary<int, Enemy> Output;
        
        public CelestialBody CurrentCelestialBody;
        public GridWorld EnemiesGrid;
        public List<Enemy> Enemies;

        private GridWorld.IntegerHandler Handler;
        private RectanglePhysique CelestialBodyRectangle;
        private Circle CelestiablBodyRange;


        public CelestialBodyExplosion()
        {
            Handler = new GridWorld.IntegerHandler(CheckEnemiesHit);
            Output = new Dictionary<int, Enemy>();
            CelestialBodyRectangle = new RectanglePhysique();
            CelestiablBodyRange = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            SyncRectangleAndCircle();

            EnemiesGrid.GetItems(CelestialBodyRectangle, Handler);
        }


        private bool CheckEnemiesHit(int index)
        {
            Enemy e = Enemies[index];
            int hash = e.GetHashCode();

            if (Output.ContainsKey(hash))
                return true;

            if (Physics.collisionCercleCercle(CelestiablBodyRange, e.Circle))
                Output.Add(hash, e);

            return true;
        }


        private void SyncRectangleAndCircle()
        {
            float range = CurrentCelestialBody.ZoneImpactDestruction;
            CelestialBodyRectangle.X = (int) (CurrentCelestialBody.Position.X - range);
            CelestialBodyRectangle.Y = (int) (CurrentCelestialBody.Position.Y - range);
            CelestialBodyRectangle.Width = (int) (range * 2);
            CelestialBodyRectangle.Height = (int) (range * 2);

            CelestiablBodyRange.Position = CurrentCelestialBody.Position;
            CelestiablBodyRange.Radius = range;
        }
    }
}
