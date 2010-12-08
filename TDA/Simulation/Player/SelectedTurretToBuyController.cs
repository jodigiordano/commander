namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class SelectedTurretToBuyController
    {
        // Out
        public Dictionary<Tourelle, bool> AvailableTurretsInScenario { get; private set; }

        public Tourelle TurretToBuy
        {
            get { return (SelectedTurret == null) ? null : SelectedTurret.Value; }
        }


        private LinkedListNode<Tourelle> SelectedTurret;
        private LinkedList<Tourelle> AvailableTurretsToBuy;
        private CommonStash CommonStash;


        public SelectedTurretToBuyController(CommonStash commonStash)
        {
            CommonStash = commonStash;

            SelectedTurret = null;
            AvailableTurretsInScenario = new Dictionary<Tourelle, bool>();
            AvailableTurretsToBuy = new LinkedList<Tourelle>();
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


        public void Update(CorpsCeleste celestialBody, Emplacement turretSpot)
        {
            bool previousSelection = SelectedTurret != null;

            AvailableTurretsInScenario.Clear();
            AvailableTurretsToBuy.Clear();

            if (celestialBody == null || turretSpot == null || turretSpot.EstOccupe)
            {
                SelectedTurret = null;
                return;
            }


            for (int i = 0; i < celestialBody.TourellesPermises.Count; i++)
            {
                Tourelle turret = celestialBody.TourellesPermises[i];

                if (turret.PrixAchat <= CommonStash.Cash)
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
