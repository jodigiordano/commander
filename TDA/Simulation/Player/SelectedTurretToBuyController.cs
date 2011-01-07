namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class SelectedTurretToBuyController
    {
        // Out
        public Dictionary<Turret, bool> AvailableTurretsInScenario { get; private set; }

        public Turret TurretToBuy
        {
            get { return (SelectedTurret == null) ? null : SelectedTurret.Value; }
        }


        private LinkedListNode<Turret> SelectedTurret;
        private LinkedList<Turret> AvailableTurretsToBuy;
        private CommonStash CommonStash;


        public SelectedTurretToBuyController(CommonStash commonStash)
        {
            CommonStash = commonStash;

            SelectedTurret = null;
            AvailableTurretsInScenario = new Dictionary<Turret, bool>();
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


        public void Update(CorpsCeleste celestialBody)
        {
            bool previousSelection = SelectedTurret != null;

            AvailableTurretsInScenario.Clear();
            AvailableTurretsToBuy.Clear();

            if (celestialBody == null)
            {
                SelectedTurret = null;
                return;
            }


            for (int i = 0; i < celestialBody.TourellesPermises.Count; i++)
            {
                Turret turret = celestialBody.TourellesPermises[i];

                if (turret.BuyPrice <= CommonStash.Cash)
                {
                    AvailableTurretsToBuy.AddLast(turret);
                    AvailableTurretsInScenario.Add(turret, true);
                }
                else
                {
                    AvailableTurretsInScenario.Add(turret, false);
                }
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
