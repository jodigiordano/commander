namespace EphemereGames.Commander.Simulation
{
    class AudioController
    {
        public EnemiesData EnemiesData;

        private Simulator Simulator;

        public AudioController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            Core.XACTAudio.XACTAudio.SetGlobalVariable("DistanceFromPlanet", 0);
            Core.XACTAudio.XACTAudio.SetGlobalVariable("NumberOfAliens", 0);
        }


        public void Update()
        {
            Core.XACTAudio.XACTAudio.SetGlobalVariable("NumberOfAliens", (float) EnemiesData.EnemyCountPerc);
            Core.XACTAudio.XACTAudio.SetGlobalVariable("DistanceFromPlanet", (float) EnemiesData.EnemyNearHitPerc);
        }
    }
}
