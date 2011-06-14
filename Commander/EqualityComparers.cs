﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class PlayerIndexComparer : IEqualityComparer<PlayerIndex>
    {
        private static PlayerIndexComparer instance;
        public static PlayerIndexComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new PlayerIndexComparer();
                return instance;
            }
        }


        public bool Equals(PlayerIndex x, PlayerIndex y)
        {
            return x == y;
        }


        public int GetHashCode(PlayerIndex obj)
        {
            return (int) obj;
        }
    }


    class ButtonsComparer : IEqualityComparer<Buttons>
    {
        private static ButtonsComparer instance;
        public static ButtonsComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new ButtonsComparer();
                return instance;
            }
        }


        public bool Equals(Buttons x, Buttons y)
        {
            return x == y;
        }


        public int GetHashCode(Buttons obj)
        {
            return (int) obj;
        }
    }


    class KeysComparer : IEqualityComparer<Keys>
    {
        private static KeysComparer instance;
        public static KeysComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new KeysComparer();
                return instance;
            }
        }


        public bool Equals(Keys x, Keys y)
        {
            return x == y;
        }


        public int GetHashCode(Keys obj)
        {
            return (int) obj;
        }
    }


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


    class TurretActionComparer : IEqualityComparer<TurretAction>
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


        public bool Equals(TurretAction x, TurretAction y)
        {
            return x == y;
        }


        public int GetHashCode(TurretAction obj)
        {
            return (int) obj;
        }
    }
    //class MouseButtonComparer : IEqualityComparer<MouseButton>
    //{
    //    private static MouseButtonComparer instance;
    //    public static MouseButtonComparer Default
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new MouseButtonComparer();
    //            return instance;
    //        }
    //    }


    //    public bool Equals(MouseButton x, MouseButton y)
    //    {
    //        return x == y;
    //    }


    //    public int GetHashCode(MouseButton obj)
    //    {
    //        return (int) obj;
    //    }
    //}
}