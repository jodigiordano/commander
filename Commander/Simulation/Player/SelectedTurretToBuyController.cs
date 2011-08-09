﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class SelectedTurretToBuyController
    {
        private LinkedListNode<TurretType> SelectedTurret;
        private LinkedList<TurretType> AvailableTurretsToBuy;
        private Dictionary<TurretType, bool> AvailableTurretsToBuyDic;


        public SelectedTurretToBuyController(Dictionary<TurretType, bool> availableTurretsToBuyDic)
        {
            SelectedTurret = null;
            AvailableTurretsToBuy = new LinkedList<TurretType>();
            AvailableTurretsToBuyDic = availableTurretsToBuyDic;
        }


        public TurretType TurretToBuy
        {
            get { return (SelectedTurret == null) ? TurretType.None : SelectedTurret.Value; }
        }


        public void Next()
        {
            if (SelectedTurret == null)
                return;

            SelectedTurret = (SelectedTurret.Next == null) ? AvailableTurretsToBuy.First : SelectedTurret.Next;
        }


        public void Previous()
        {
            if (SelectedTurret == null)
                return;

            SelectedTurret = (SelectedTurret.Previous == null) ? AvailableTurretsToBuy.Last : SelectedTurret.Previous;
        }


        public void Update(CelestialBody celestialBody)
        {
            bool previousSelection = SelectedTurret != null;

            AvailableTurretsToBuy.Clear();

            if (celestialBody == null || celestialBody.FirstOnPath)
            {
                SelectedTurret = null;
                return;
            }

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
