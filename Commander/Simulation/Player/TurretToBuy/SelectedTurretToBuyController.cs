namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class SelectedTurretToBuyController
    {
        protected LinkedListNode<TurretType> SelectedTurret;
        protected LinkedList<TurretType> AvailableTurretsToBuy;
        protected Dictionary<TurretType, bool> AvailableTurretsToBuyDic;


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


        public virtual void Next()
        {
            //if (SelectedTurret == null)
            //    return;

            SelectedTurret = (SelectedTurret.Next == null) ? AvailableTurretsToBuy.First : SelectedTurret.Next;
        }


        public virtual void Previous()
        {
            //if (SelectedTurret == null)
            //    return;

            SelectedTurret = (SelectedTurret.Previous == null) ? AvailableTurretsToBuy.Last : SelectedTurret.Previous;
        }


        public abstract void Update(CelestialBody celestialBody);
    }
}
