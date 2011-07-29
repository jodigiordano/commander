namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    public class PlayerIndexComparer : IEqualityComparer<PlayerIndex>
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


    public class ButtonsComparer : IEqualityComparer<Buttons>
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


    public class KeysComparer : IEqualityComparer<Keys>
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


    public class MouseButtonComparer : IEqualityComparer<MouseButton>
    {
        private static MouseButtonComparer instance;
        public static MouseButtonComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new MouseButtonComparer();
                return instance;
            }
        }


        public bool Equals(MouseButton x, MouseButton y)
        {
            return x == y;
        }


        public int GetHashCode(MouseButton obj)
        {
            return (int) obj;
        }
    }
}
