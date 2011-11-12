namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    [XmlRoot(ElementName = "InfiniteWaves")]
    public class InfiniteWavesDescriptor
    {
        public List<EnemyType> Enemies;
        public int StartingDifficulty;
        public int DifficultyIncrement;
        public Vector2 MinMaxEnemiesPerWave;
        public int MineralsPerWave;
        public bool FirstOneStartNow;

        // optional
        public bool Upfront;
        public int NbWaves;

        public InfiniteWavesDescriptor()
        {
            Enemies = new List<EnemyType>();
            StartingDifficulty = 1;
            DifficultyIncrement = 0;
            MinMaxEnemiesPerWave = new Vector2(1, 1);
            FirstOneStartNow = false;
            Upfront = false;
            NbWaves = 1;
        }
    }

}
