﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class SelectedCelestialBodyController
    {
        public Vector3 LastPositionSelectedCelestialBody;
        public bool SelectedCelestialBodyChanged;
        public bool SelectedTurretChanged;

        private int SelectedCelestialBodyIndex;
        private int SelectedTurretCelestialBodyIndex;
        private int SelectedTurretIndex;
        private List<CorpsCeleste> CelestialBodies;
        private Cercle Cercle;


        public SelectedCelestialBodyController(List<CorpsCeleste> celestialBodies, Cercle cercle)
        {
            CelestialBodies = celestialBodies;
            Cercle = cercle;

            Initialize();
        }


        public void Initialize()
        {
            SelectedCelestialBodyIndex = -1;
            SelectedTurretIndex = -1;
            SelectedTurretCelestialBodyIndex = -1;

            LastPositionSelectedCelestialBody = Vector3.Zero;
        }


        public CorpsCeleste CelestialBody
        {
            get { return (SelectedTurretIndex == -1 && SelectedCelestialBodyIndex != -1) ? CelestialBodies[SelectedCelestialBodyIndex] : null; }
        }


        public Turret Turret
        {
            get { return (SelectedTurretCelestialBodyIndex != -1 && SelectedTurretIndex != -1) ? CelestialBodies[SelectedTurretCelestialBodyIndex].Turrets[SelectedTurretIndex] : null; }
        }


        public void UpdateSelection()
        {
            DoTurretSelection();
            DoCelestialBodySelection(); 
        }


        private void DoTurretSelection()
        {
            int previousIndex = SelectedTurretIndex;

            SelectedTurretIndex = -1;
            SelectedTurretCelestialBodyIndex = -1;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Alive || !CelestialBodies[i].Selectionnable)
                    continue;

                for (int j = 0; j < CelestialBodies[i].Turrets.Count; j++)
                    if (CelestialBodies[i].Turrets[j].Visible &&
                        EphemereGames.Core.Physique.Facade.collisionCercleCercle(Cercle, CelestialBodies[i].Turrets[j].Cercle))
                    {
                        SelectedCelestialBodyIndex = -1;
                        SelectedTurretCelestialBodyIndex = i;
                        SelectedTurretIndex = j;
                        break;
                    }
            }

            SelectedTurretChanged = SelectedTurretIndex != previousIndex;

            if (SelectedTurretChanged && SelectedTurretIndex != -1)
                LastPositionSelectedCelestialBody = CelestialBodies[SelectedTurretCelestialBodyIndex].Position;
        }


        private void DoCelestialBodySelection()
        {
            if (SelectedTurretIndex != -1)
                return;

            int previousIndex = SelectedCelestialBodyIndex;

            SelectedCelestialBodyIndex = -1;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Alive || !CelestialBodies[i].Selectionnable)
                    continue;

                if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(Cercle, CelestialBodies[i].Cercle))
                {
                    SelectedCelestialBodyIndex = i;
                    break;
                }
            }

            SelectedCelestialBodyChanged = SelectedCelestialBodyIndex != previousIndex;

            if (SelectedCelestialBodyChanged && SelectedCelestialBodyIndex != -1)
                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;
        }


        public Vector3 DoGlueMode()
        {
            if ( SelectedCelestialBodyIndex >= 0 && SelectedCelestialBodyIndex < CelestialBodies.Count)
            {
                Vector3 diff = CelestialBodies[SelectedCelestialBodyIndex].Position - LastPositionSelectedCelestialBody;

                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;

                return diff;
            }

            else if ( SelectedCelestialBodyIndex >= 0 && SelectedCelestialBodyIndex < CelestialBodies.Count )
            {
                Vector3 diff = CelestialBodies[SelectedTurretCelestialBodyIndex].Position - LastPositionSelectedCelestialBody;

                LastPositionSelectedCelestialBody = CelestialBodies[SelectedTurretCelestialBodyIndex].Position;

                return diff;
            }

            return Vector3.Zero;
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyIndex = -1;
            SelectedTurretCelestialBodyIndex = -1;
        }
    }
}
