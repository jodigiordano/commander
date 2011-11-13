namespace EphemereGames.Commander.Simulation
{
    abstract class EditorPlayerCommand : EditorCommand
    {
        public EditorPlayerCommand(SimPlayer owner)
            : base(owner)
        {

        }
    }


    class EditorPlayerLifePointsCommand : EditorPlayerCommand
    {
        public int LifePoints;


        public EditorPlayerLifePointsCommand(SimPlayer owner, int lifePoints)
            : base(owner)
        {
            LifePoints = lifePoints;
        }
    }


    class EditorPlayerCashCommand : EditorPlayerCommand
    {
        public int Cash;


        public EditorPlayerCashCommand(SimPlayer owner, int cash)
            : base(owner)
        {
            Cash = cash;
        }
    }


    class EditorPlayerMineralsCommand : EditorPlayerCommand
    {
        public int Minerals;


        public EditorPlayerMineralsCommand(SimPlayer owner, int minerals)
            : base(owner)
        {
            Minerals = minerals;
        }
    }


    class EditorPlayerLifePacksCommand : EditorPlayerCommand
    {
        public int LifePacks;


        public EditorPlayerLifePacksCommand(SimPlayer owner, int lifePacks)
            : base(owner)
        {
            LifePacks = lifePacks;
        }
    }


    class EditorPlayerBulletDamageCommand : EditorPlayerCommand
    {
        public double BulletDamage;


        public EditorPlayerBulletDamageCommand(SimPlayer owner, int bulletDamage)
            : base(owner)
        {
            BulletDamage = bulletDamage;
        }
    }
}
