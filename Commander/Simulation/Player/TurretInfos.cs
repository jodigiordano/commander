namespace EphemereGames.Commander.Simulation.Player
{
    class TurretInfos
    {
        private Turret selectedTurret;
        private TurretMenu TurretMenu;


        public TurretInfos(TurretMenu turretMenu)
        {
            selectedTurret = null;
            TurretMenu = turretMenu;
        }


        public Turret SelectedTurret
        {
            get { return selectedTurret; }

            set
            {
                if (SelectedTurret != null && SelectedTurret != value)
                {
                    SelectedTurret.ShowRange = false;
                    SelectedTurret.ShowRangePreview = false;
                }

                selectedTurret = value;

                if (SelectedTurret != null)
                {
                    SelectedTurret.ShowRange = true;
                    SelectedTurret.ShowRangePreview = TurretMenu.Selection == TurretChoice.Update;
                }
            }
        }
    }
}
