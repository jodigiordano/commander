namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class SelectedTurretToBuyOnlyAvailablesController : SelectedTurretToBuyController
    {
        public SelectedTurretToBuyOnlyAvailablesController(Dictionary<TurretType, bool> availableTurretsToBuyDic)
            : base(availableTurretsToBuyDic)
        {

        }


        public override void Update(CelestialBody celestialBody)
        {
            if (celestialBody == null || celestialBody.FirstOnPath)
            {
                SelectedTurret = null;
                return;
            }

            bool previousSelection = SelectedTurret != null;

            AvailableTurretsToBuy.Clear();

            foreach (var turret in AvailableTurretsToBuyDic)
                if (turret.Value)
                    AvailableTurretsToBuy.AddLast(turret.Key);

            if (AvailableTurretsToBuy.Count > 0 && !previousSelection)
                SelectedTurret = AvailableTurretsToBuy.First;
            else if (AvailableTurretsToBuy.Count == 0)
                SelectedTurret = null;
            else if (previousSelection)
                SelectedTurret = AvailableTurretsToBuy.Find(SelectedTurret.Value);
        }
    }
}
