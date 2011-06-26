namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class BoostedTurrets
    {
        public List<KeyValuePair<Turret, Turret>> Output;

        public GridWorld TurretsGrid;
        public List<Turret> Turrets;

        private IntegerHandler Handler;
        private Turret CurrentTurret;
        private PhysicalRectangle CurrentTurretRectangle;
        private Circle CurrentTurretRange;
        
        
        public BoostedTurrets()
        {
            Output = new List<KeyValuePair<Turret, Turret>>();
            Handler = new IntegerHandler(CheckTurretIsBoosted);
            CurrentTurret = null;
            CurrentTurretRectangle = new PhysicalRectangle();
            CurrentTurretRange = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Turrets.Count; i++)
            {
                CurrentTurret = Turrets[i];

                if (CurrentTurret.Type != TurretType.Booster)
                    continue;

                SyncRectangleAndCircle();

                TurretsGrid.GetItems(CurrentTurretRectangle, Handler);
            }
        }


        private bool CheckTurretIsBoosted(int index)
        {
            Turret boostedTurret = Turrets[index];

            if (boostedTurret.Type == TurretType.Booster)
                return true;

            if (Physics.CircleCicleCollision(boostedTurret.Circle, CurrentTurretRange))
                Output.Add(new KeyValuePair<Turret, Turret>(CurrentTurret, boostedTurret));

            return true;
        }


        private void SyncRectangleAndCircle()
        {
            float range = CurrentTurret.Range;
            CurrentTurretRectangle.X = (int) (CurrentTurret.Position.X - range);
            CurrentTurretRectangle.Y = (int) (CurrentTurret.Position.Y - range);
            CurrentTurretRectangle.Width = (int) (range * 2);
            CurrentTurretRectangle.Height = (int) (range * 2);

            CurrentTurretRange.Position = CurrentTurret.Position;
            CurrentTurretRange.Radius = range;
        }
    }
}
