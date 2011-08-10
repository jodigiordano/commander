namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class SelectedTurretToBuyAllController : SelectedTurretToBuyController
    {
        private LinkedListNode<TurretType> LastSelectedTurret;


        public SelectedTurretToBuyAllController(Dictionary<TurretType, bool> availableTurretsToBuyDic)
            : base(availableTurretsToBuyDic)
        {
            foreach (var turret in AvailableTurretsToBuyDic)
                AvailableTurretsToBuy.AddLast(turret.Key);

            SelectedTurret = LastSelectedTurret = AvailableTurretsToBuy.Count == 0 ? null : AvailableTurretsToBuy.First;
        }


        public override void Next()
        {
            base.Next();

            LastSelectedTurret = SelectedTurret;
        }


        public override void Previous()
        {
            base.Previous();

            LastSelectedTurret = SelectedTurret;
        }


        public override void Update(CelestialBody celestialBody)
        {
            if (celestialBody == null || celestialBody.FirstOnPath || AvailableTurretsToBuy.Count == 0)
            {
                if (SelectedTurret != null)
                    LastSelectedTurret = SelectedTurret;
                
                SelectedTurret = null;
                return;
            }

            SelectedTurret = LastSelectedTurret;
        }
    }
}
