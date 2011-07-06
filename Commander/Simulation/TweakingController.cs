namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;


    class TweakingController
    {
        public TurretsFactory TurretsFactory;
        public PowerUpsFactory PowerUpsFactory;
        public EnemiesFactory EnemiesFactory;
        public MineralsFactory MineralsFactory;
        public BulletsFactory BulletsFactory;

        public event NoneHandler TweakingDataChanged;

        private Simulator Simulator;


        public TweakingController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {

        }


        public void Sync()
        {
            TurretsFactory.Initialize();
            PowerUpsFactory.Initialize();
            EnemiesFactory.Initialize();
            MineralsFactory.Initialize();
            BulletsFactory.Initialize();

            NotifyTweakingDataChanged();
        }


        private void NotifyTweakingDataChanged()
        {
            if (TweakingDataChanged != null)
                TweakingDataChanged();
        }
    }
}
