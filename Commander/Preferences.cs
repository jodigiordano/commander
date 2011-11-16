namespace EphemereGames.Commander
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class Preferences
    {
        public const int TimeBetweenTwoMusics = 300;
        public static float TargetElapsedTimeMs = 16;

#if DEBUG
        public static bool Debug = true;
        public static bool FullScreen = false;
#else
        public static bool Debug = false;
        public static bool FullScreen = true;
#endif


#if WINDOWS && ARCADEROYALE
        public static Setting Target = Setting.ArcadeRoyale;
        public static Vector2 DeadZoneV2 = new Vector2(0, 0);
        public static Vector3 DeadZone = new Vector3(0, 0, 0);
        public static Vector2 BackBuffer = new Vector2(640, 480);
        public static Vector2 BattlefieldBoundaries = new Vector2(1280, 720);
        public const float BackBufferZoom = 0.5f;
#elif WINDOWS
        public static Setting Target = Setting.Windows;
        public static Vector2 DeadZoneV2 = new Vector2(0, 0);
        public static Vector3 DeadZone = new Vector3(0, 0, 0);
        public static Vector2 BackBuffer = new Vector2(1280, 720);
        public static Vector2 BattlefieldBoundaries = new Vector2(1280, 720);
        public const float BackBufferZoom = 1f;
#else
        public static Setting Target = Setting.Xbox360;
        public static Vector2 DeadZoneV2 = new Vector2(20, 30);
        public static Vector3 DeadZone = new Vector3(20, 30, 0);
        public static Vector2 BackBuffer = new Vector2(1280, 720);
        public static Vector2 BattlefieldBoundaries = new Vector2(1280, 720);
        public const float BackBufferZoom = 1f;
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

        public const float PrioriteGUIVueAvanceeTrajectoiresCorpsCeleste = PrioriteFondEcran - 0.03f;
        public const float PrioriteGUIVUeAvanceeZonesActivations = PrioriteFondEcran - 0.04f;
        public const float PrioriteSimulationCeintureAsteroides = PrioriteFondEcran - 0.05f;
        public const float PrioriteGUIPointsVieJoueur = PrioriteGUIPanneauGeneral + 0.01f;


        public const string Salt = "YourMom";


#if DEBUG
        public const string WebsiteURL = @"http://localhost/commander";
#else
        public const string WebsiteURL = @"http://commander.ephemeregames.com";
#endif

        public const string GeneralNewsURL = @"/feeds/news.rss";
        public const string UpdatesNewsURL = @"/feeds/updates.rss";
        public const string DLCNewsURL = @"/feeds/dlc.rss";
        public const string MultiverseWorldsURL = @"/multiverse/worlds";
        public const string MultiverseScriptsURL = @"/multiverse/scripts";
        public const string NewUserScript = @"/new_user.php";
        public const string LoginScript = @"/login.php";
        public const string LastUpdateScript = @"/last_update.php";
        public const string UsernameToWorldIdScript = @"/get_world_id.php";
        public const string SaveWorldScript = @"/save_world.php";

        public const int CampaignVersion = 1;
    }
}
