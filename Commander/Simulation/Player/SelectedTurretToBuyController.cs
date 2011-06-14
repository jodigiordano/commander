namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class SelectedTurretToBuyController
    {
        public Dictionary<TurretType, bool> AvailableTurrets;

        public Turret TurretToBuy
        {
            get { return (SelectedTurret == null) ? null : SelectedTurret.Value; }
        }


        private LinkedListNode<Turret> SelectedTurret;
        private LinkedList<Turret> AvailableTurretsToBuy;
        private CommonStash CommonStash;
        private Simulation Simulation;


        public SelectedTurretToBuyController(Simulation simulation, CommonStash commonStash)
        {
            Simulation = simulation;
            CommonStash = commonStash;

            SelectedTurret = null;
            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);
            AvailableTurretsToBuy = new LinkedList<Turret>();
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


        public Turret FirstAvailable
        {
            get { return (AvailableTurretsToBuy.First != null) ? AvailableTurretsToBuy.First.Value : null; }
        }


        public Turret LastAvailable
        {
            get { return (AvailableTurretsToBuy.Last != null) ? AvailableTurretsToBuy.Last.Value : null; }
        }


        public void SetSelectedTurret(Turret turret)
        {
            SelectedTurret = AvailableTurretsToBuy.Find(turret);
        }


        public void Update(CelestialBody celestialBody)
        {
            bool previousSelection = SelectedTurret != null;

            AvailableTurretsToBuy.Clear();
            AvailableTurrets.Clear();

            if (celestialBody == null)
            {
                SelectedTurret = null;
                return;
            }

            foreach (var turret in Simulation.TurretsFactory.Availables.Values)
            {
                if (turret.BuyPrice <= CommonStash.Cash)
                    AvailableTurretsToBuy.AddLast(turret);

                AvailableTurrets.Add(turret.Type, turret.BuyPrice <= CommonStash.Cash);
            }

            if (AvailableTurretsToBuy.Count > 0 && !previousSelection)
                SelectedTurret = AvailableTurretsToBuy.First;
            else if (AvailableTurretsToBuy.Count == 0)
                SelectedTurret = null;
            else if (previousSelection)
                SelectedTurret = AvailableTurretsToBuy.Find(SelectedTurret.Value);
        }
    }
}
