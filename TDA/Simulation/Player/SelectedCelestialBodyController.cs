namespace EphemereGames.Commander
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
        public bool SelectedTurretSpotChanged;

        private int SelectedCelestialBodyIndex;
        private int SelectedTurretSpotIndex;
        private List<CorpsCeleste> CelestialBodies;
        private Cercle Cercle;


        public SelectedCelestialBodyController(List<CorpsCeleste> celestialBodies, Cercle cercle)
        {
            CelestialBodies = celestialBodies;
            Cercle = cercle;

            SelectedCelestialBodyIndex = -1;
            SelectedTurretSpotIndex = -1;

            LastPositionSelectedCelestialBody = Vector3.Zero;
        }


        public CorpsCeleste CelestialBody
        {
            get { return (SelectedCelestialBodyIndex != -1) ? CelestialBodies[SelectedCelestialBodyIndex] : null; }
        }


        public Emplacement TurretSpot
        {
            get { return (SelectedCelestialBodyIndex != -1 && SelectedTurretSpotIndex != -1) ? CelestialBodies[SelectedCelestialBodyIndex].Emplacements[SelectedTurretSpotIndex] : null; }
        }


        public void UpdateSelection()
        {
            doTurretSpotSelection();
            doCelestialBodySelection(); 
        }


        private void doTurretSpotSelection()
        {
            int previousIndex = SelectedTurretSpotIndex;

            SelectedTurretSpotIndex = -1;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Selectionnable)
                    continue;

                for (int j = 0; j < CelestialBodies[i].Emplacements.Count; j++)
                    if (EphemereGames.Core.Physique.Facade.collisionCercleRectangle(Cercle, CelestialBodies[i].Emplacements[j].Rectangle))
                    {
                        SelectedCelestialBodyChanged = SelectedCelestialBodyIndex != i;

                        SelectedCelestialBodyIndex = i;
                        SelectedTurretSpotIndex = j;
                        break;
                    }
            }

            SelectedTurretSpotChanged = SelectedTurretSpotIndex != previousIndex;

            if (SelectedCelestialBodyChanged && SelectedCelestialBodyIndex != -1)
                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;
        }


        private void doCelestialBodySelection()
        {
            if (SelectedTurretSpotIndex != -1)
                return;

            int previousIndex = SelectedCelestialBodyIndex;

            SelectedCelestialBodyIndex = -1;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Selectionnable)
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


        public Vector3 doGlueMode()
        {
            if (SelectedCelestialBodyIndex != -1)
            {
                Vector3 diff = CelestialBodies[SelectedCelestialBodyIndex].Position - LastPositionSelectedCelestialBody;

                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;

                return diff;
            }

            return Vector3.Zero;
        }


        public void doCelestialBodyDestroyed()
        {
            SelectedCelestialBodyIndex = -1;
        }
    }
}
