namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Core.Input;

    class Preferences
    {
        // Autre
        public const int TempsEntreDeuxChangementMusique = 300;

        // Dimensions
        public static Vector2 DeadZoneXbox = new Vector2(20, 30);

        // Priorités d'affichage

        // Fixes
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

        // Fixes relatives
        //public const float PrioriteSimulationEmplacement = PrioriteSimulationCorpsCeleste - 0.1f; // 0.7
        public const float PrioriteSimulationEnnemi = PrioriteSimulationCorpsCeleste + 0.1f; // 0.9
        public const float PrioriteSimulationTourelle = PrioriteSimulationCorpsCeleste - 0.2f; // 0.6
        //public const float PrioriteSimulationProjectile = PrioriteSimulationCorpsCeleste - 0.15f; // 0.65

        public const float PrioriteGUIEtoiles = PrioriteFondEcran - 0.01f;
        public const float PrioriteSimulationChemin = PrioriteFondEcran - 0.02f;
        public const float PrioriteGUIVueAvanceeTrajectoiresCorpsCeleste = PrioriteFondEcran - 0.03f;
        public const float PrioriteGUIVUeAvanceeZonesActivations = PrioriteFondEcran - 0.04f;
        public const float PrioriteSimulationCeintureAsteroides = PrioriteFondEcran - 0.05f;

        public const float PrioriteGUIVUeAvanceePointsVieEnnemis = PrioriteSimulationEnnemi - 0.01f;
        public const float PrioriteGUISelectionCorpsCeleste = PrioriteSimulationCorpsCeleste + 0.01f;
        public const float PrioriteGUIPointsVieJoueur = PrioriteGUIPanneauGeneral + 0.01f;


        // Touches

#if XBOX || MANETTE_WINDOWS
        public const Buttons toucheSelection = Buttons.A;
        public const Buttons toucheRetour = Buttons.B;
        public const Buttons toucheRetourMenu = Buttons.Start;
        public const Buttons toucheRetourMenu2 = Buttons.Start;

        public const Buttons toucheChangerMusique = Buttons.DPadUp;

        public const Buttons toucheProchaineVague = Buttons.Y;
        public const Buttons toucheVueAvancee = Buttons.X;

        public const Buttons toucheSouris = Buttons.LeftStick;

        public const Buttons toucheSelectionSuivant = Buttons.RightTrigger;
        public const Buttons toucheSelectionPrecedent = Buttons.LeftTrigger;

        public const Buttons toucheDebug = Buttons.Back;
        public const Buttons toucheMasquerEditeur = Buttons.LeftShoulder;

#elif WINDOWS

        public const BoutonSouris toucheSelection = BoutonSouris.Gauche;
        public const BoutonSouris toucheRetour = BoutonSouris.Droite;
        public const Keys toucheRetourMenu = Keys.Enter;
        public const Keys toucheRetourMenu2 = Keys.Escape;

        public const Keys toucheChangerMusique = Keys.RightShift;

        public const Keys toucheProchaineVague = Keys.RightControl;
        public const BoutonSouris toucheVueAvancee = BoutonSouris.Milieu;

        public const BoutonSouris toucheSelectionSuivant = BoutonSouris.MilieuHaut;
        public const BoutonSouris toucheSelectionPrecedent = BoutonSouris.MilieuBas;

        public const Keys toucheDebug = Keys.F1;
        public const Keys toucheMasquerEditeur = Keys.F2;

#endif
    }
}
