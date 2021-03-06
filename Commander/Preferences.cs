﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class Preferences
    {
        public const int TimeBetweenTwoMusics = 300;
        public static float TargetElapsedTimeMs = 16;
        public static Vector2 BackBuffer = new Vector2(1280, 720);

#if DEBUG
        public static bool Debug = true;
        public static bool FullScreen = false;
#else
        public static bool Debug = false;
        public static bool FullScreen = true;
#endif


#if WINDOWS && TRIAL
        public static Vector2 DeadZoneV2 = new Vector2(0, 0);
        public static Vector3 DeadZone = new Vector3(0, 0, 0);
        public static Setting Target = Setting.WindowsDemo;
#elif WINDOWS
        public static Vector2 DeadZoneV2 = new Vector2(0, 0);
        public static Vector3 DeadZone = new Vector3(0, 0, 0);
        public static Setting Target = Setting.WindowsFull;
#else
        public static Vector2 DeadZoneV2 = new Vector2(20, 30);
        public static Vector3 DeadZone = new Vector3(20, 30, 0);
        public static Setting Target = Setting.Xbox360;
#endif

        public static string ProductName = "commander";

        public const float PrioriteGUIHistoire = 0.2f;  //(Sous-composants, + [0.0 .. 0.09]) donc [0.1 .. 0.19]
        public const float PrioriteGUIPanneauGeneral = 0.4f; //(Sous-composants, + [0.0 .. 0.09]) donc [0.2 .. 0.29]
        public const float PrioriteGUIPanneauCorpsCeleste = 0.3f; //(Sous-composants, + [0.0 .. 0.09]) donc [0.3 .. 0.39]
        public const float PrioriteGUIPanneauDebug = 0f;
        public const float PrioriteGUIConsoleEditeur = 0f;
        public const float PrioriteGUIMenuPrincipal = 0.03f;
        public const float PrioriteFondEcran = 1f;
        public const float PrioriteTransitionScene = 0f;

        public const float PrioriteGUIEtoiles = PrioriteFondEcran - 0.01f;
        public const float PrioriteGUIVueAvanceeTrajectoiresCorpsCeleste = PrioriteFondEcran - 0.03f;
        public const float PrioriteGUIVUeAvanceeZonesActivations = PrioriteFondEcran - 0.04f;
        public const float PrioriteSimulationCeintureAsteroides = PrioriteFondEcran - 0.05f;
        public const float PrioriteGUIPointsVieJoueur = PrioriteGUIPanneauGeneral + 0.01f;


        public const string WebsiteURL = "http://commander.ephemeregames.com";
        public const string GeneralNewsURL = "/feeds/news.rss";
        public const string UpdatesNewsURL = "/feeds/updates.rss";
        public const string DLCNewsURL = "/feeds/dlc.rss";
    }
}
