namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class EnemyCheckBox : ImageCheckBox
    {
        public EnemyType Enemy;


        public EnemyCheckBox(string label, EnemyType type, float sizeX)
            : base(new Image(label) { SizeX = sizeX })
        {
            Enemy = type;
        }
    }
}
