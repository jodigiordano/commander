namespace EphemereGames.Commander.Colors
{
    using Microsoft.Xna.Framework;


    class Default
    {
        public static readonly Color NeutralBright = new Color(255, 255, 255);
        public static readonly Color NeutralDark = new Color(0, 0, 0);
        public static readonly Color NeutralGray = new Color(150, 150, 150);
        public static readonly Color AlienBright = new Color(234, 196, 28);
        public static readonly Color HumansBright = new Color(53, 171, 255);
        public static readonly Color Pink = new Color(255, 0, 220, 255);
        public static readonly Color PlanetNearHit = new Color(255, 0, 0, 255);
        public static readonly Color GamePaused = new Color(255, 0, 150);
        public static readonly Color Teleport = new Color(73, 119, 255);
    }


    class Spaceship
    {
        public static readonly Color Pink = new Color(255, 0, 136);
        public static readonly Color Yellow = new Color(255, 227, 48);

        public static readonly Color Orange = new Color(255, 115, 40);
        public static readonly Color Blue = new Color(95, 71, 255);
        public static readonly Color Green = new Color(128, 255, 63);

        public static readonly Color CMCanDefault = Color.White;
        public static readonly Color CMCannotDefault = Color.Red;
        public static readonly Color CMCanAlt = new Color(50, 137, 0);
        public static readonly Color CMCannotAlt = new Color(81, 20, 20);
    }


    class Panel
    {
        public static readonly Color Cancel = new Color(255, 0, 0);
        public static readonly Color Submit = new Color(0, 255, 0);
        public static readonly Color Waiting = new Color(255, 255, 255);
        public static readonly Color Error = new Color(255, 0, 0);
        public static readonly Color Ok = new Color(0, 255, 0);
    }
}