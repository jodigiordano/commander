namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;

    enum Setting
    {
        WindowsDemo,
        WindowsFull,
        Xbox360
    }

    class Preferences
    {
        public const int TempsEntreDeuxChangementMusique = 300;
        public static Vector2 DeadZoneXbox = new Vector2(20, 30);

#if DEBUG
        public static bool FullScreen = false;
#else
        public static bool FullScreen = true;
#endif

#if DEBUG
        public static bool HomeMadeValidation = false;
#else
        public static bool HomeMadeValidation = true;
#endif

#if WINDOWS && TRIAL
        public static Setting Target = Setting.WindowsDemo;
#elif WINDOWS
        public static Setting Target = Setting.WindowsFull;
#else
        public static Setting Target = Setting.Xbox360;
#endif

        public static string ProductName = "commander";

        public const float PrioriteGUIVictoireDefaite = 0f; //(Sous-composants, + [0.0 .. 0.09]) donc [0.0 .. 0.09]
        public const float PrioriteGUIHistoire = 0.1f;  //(Sous-composants, + [0.0 .. 0.09]) donc [0.1 .. 0.19]
        public const float PrioriteGUIPanneauGeneral = 0.2f; //(Sous-composants, + [0.0 .. 0.09]) donc [0.2 .. 0.29]
        public const float PrioriteGUIPanneauCorpsCeleste = 0.3f; //(Sous-composants, + [0.0 .. 0.09]) donc [0.3 .. 0.39]
        public const float PrioriteGUIPanneauDebug = 0f;
        public const float PrioriteGUIConsoleEditeur = 0f;
        public const float PrioriteGUIMenuPrincipal = 0.03f;
        public const float PrioriteSimulationCorpsCeleste = 0.8f; //(Sous-composants, + [0.0 .. 0.01]) donc [0.9 .. 0.91]
        public const float PrioriteFondEcran = 1f;
        public const float PrioriteTransitionScene = 0f;

        public const float PrioriteSimulationTourelle = PrioriteSimulationCorpsCeleste - 0.2f; // 0.6

        public const float PrioriteGUIEtoiles = PrioriteFondEcran - 0.01f;
        public const float PrioriteSimulationChemin = PrioriteFondEcran - 0.02f;
        public const float PrioriteGUIVueAvanceeTrajectoiresCorpsCeleste = PrioriteFondEcran - 0.03f;
        public const float PrioriteGUIVUeAvanceeZonesActivations = PrioriteFondEcran - 0.04f;
        public const float PrioriteSimulationCeintureAsteroides = PrioriteFondEcran - 0.05f;

        public const float PrioriteSimulationEnnemi = PrioriteSimulationChemin - 0.00001f; //PrioriteSimulationCorpsCeleste + 0.1f; // 0.9

        public const float PrioriteGUIVUeAvanceePointsVieEnnemis = PrioriteSimulationEnnemi - 0.01f;
        public const float PrioriteGUISelectionCorpsCeleste = PrioriteSimulationCorpsCeleste + 0.01f;
        public const float PrioriteGUIPointsVieJoueur = PrioriteGUIPanneauGeneral + 0.01f;
    }
}
