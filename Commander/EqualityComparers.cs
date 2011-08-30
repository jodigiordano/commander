namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;


    class BulletTypeComparer : IEqualityComparer<BulletType>
    {
        private static BulletTypeComparer instance;
        public static BulletTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new BulletTypeComparer();
                return instance;
            }
        }


        public bool Equals(BulletType x, BulletType y)
        {
            return x == y;
        }


        public int GetHashCode(BulletType obj)
        {
            return (int) obj;
        }
    }


    class EnemyTypeComparer : IEqualityComparer<EnemyType>
    {
        private static EnemyTypeComparer instance;
        public static EnemyTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new EnemyTypeComparer();
                return instance;
            }
        }


        public bool Equals(EnemyType x, EnemyType y)
        {
            return x == y;
        }


        public int GetHashCode(EnemyType obj)
        {
            return (int) obj;
        }
    }


    class TurretTypeComparer : IEqualityComparer<TurretType>
    {
        private static TurretTypeComparer instance;
        public static TurretTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new TurretTypeComparer();
                return instance;
            }
        }


        public bool Equals(TurretType x, TurretType y)
        {
            return x == y;
        }


        public int GetHashCode(TurretType obj)
        {
            return (int) obj;
        }
    }


    class PowerUpTypeComparer : IEqualityComparer<PowerUpType>
    {
        private static PowerUpTypeComparer instance;
        public static PowerUpTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new PowerUpTypeComparer();
                return instance;
            }
        }


        public bool Equals(PowerUpType x, PowerUpType y)
        {
            return x == y;
        }


        public int GetHashCode(PowerUpType obj)
        {
            return (int) obj;
        }
    }


    class MineralTypeComparer : IEqualityComparer<MineralType>
    {
        private static MineralTypeComparer instance;
        public static MineralTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new MineralTypeComparer();
                return instance;
            }
        }


        public bool Equals(MineralType x, MineralType y)
        {
            return x == y;
        }


        public int GetHashCode(MineralType obj)
        {
            return (int) obj;
        }
    }


    class TurretActionComparer : IEqualityComparer<TurretChoice>
    {
        private static TurretActionComparer instance;
        public static TurretActionComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new TurretActionComparer();
                return instance;
            }
        }


        public bool Equals(TurretChoice x, TurretChoice y)
        {
            return x == y;
        }


        public int GetHashCode(TurretChoice obj)
        {
            return (int) obj;
        }
    }


    class EditorGeneralMenuChoiceComparer : IEqualityComparer<EditorGeneralMenuChoice>
    {
        private static EditorGeneralMenuChoiceComparer instance;
        public static EditorGeneralMenuChoiceComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new EditorGeneralMenuChoiceComparer();
                return instance;
            }
        }


        public bool Equals(EditorGeneralMenuChoice x, EditorGeneralMenuChoice y)
        {
            return x == y;
        }


        public int GetHashCode(EditorGeneralMenuChoice obj)
        {
            return (int) obj;
        }
    }


    class EditorPanelComparer : IEqualityComparer<EditorPanel>
    {
        private static EditorPanelComparer instance;
        public static EditorPanelComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new EditorPanelComparer();
                return instance;
            }
        }


        public bool Equals(EditorPanel x, EditorPanel y)
        {
            return x == y;
        }


        public int GetHashCode(EditorPanel obj)
        {
            return (int) obj;
        }
    }


    class HelpBarMessageComparer : IEqualityComparer<HelpBarMessage>
    {
        private static HelpBarMessageComparer instance;
        public static HelpBarMessageComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new HelpBarMessageComparer();
                return instance;
            }
        }


        public bool Equals(HelpBarMessage x, HelpBarMessage y)
        {
            return x == y;
        }


        public int GetHashCode(HelpBarMessage obj)
        {
            return (int) obj;
        }
    }


    class NewGameChoiceComparer : IEqualityComparer<NewGameChoice>
    {
        private static NewGameChoiceComparer instance;
        public static NewGameChoiceComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new NewGameChoiceComparer();
                return instance;
            }
        }


        public bool Equals(NewGameChoice x, NewGameChoice y)
        {
            return x == y;
        }


        public int GetHashCode(NewGameChoice obj)
        {
            return (int) obj;
        }
    }


    class PanelTypeComparer : IEqualityComparer<PanelType>
    {
        private static PanelTypeComparer instance;
        public static PanelTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new PanelTypeComparer();
                return instance;
            }
        }


        public bool Equals(PanelType x, PanelType y)
        {
            return x == y;
        }


        public int GetHashCode(PanelType obj)
        {
            return (int) obj;
        }
    }


    class NewsTypeComparer : IEqualityComparer<NewsType>
    {
        private static NewsTypeComparer instance;
        public static NewsTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new NewsTypeComparer();
                return instance;
            }
        }


        public bool Equals(NewsType x, NewsType y)
        {
            return x == y;
        }


        public int GetHashCode(NewsType obj)
        {
            return (int) obj;
        }
    }


    class HelpBarMessageTypeComparer : IEqualityComparer<HelpBarMessageType>
    {
        private static HelpBarMessageTypeComparer instance;
        public static HelpBarMessageTypeComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new HelpBarMessageTypeComparer();
                return instance;
            }
        }


        public bool Equals(HelpBarMessageType x, HelpBarMessageType y)
        {
            return x == y;
        }


        public int GetHashCode(HelpBarMessageType obj)
        {
            return (int) obj;
        }
    }
}
