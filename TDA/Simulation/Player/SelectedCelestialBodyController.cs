namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;

    class SelectedCelestialBodyController
    {
        public Vector3 LastPositionSelectedCelestialBody;
        public bool SelectedCelestialBodyChanged;
        public bool SelectedTurretSpotChanged;

        private int SelectedCelestialBodyIndex;
        private int SelectedTurretSpotIndex;
        private List<CorpsCeleste> CelestialBodies;
        private Cursor Cursor;


        public SelectedCelestialBodyController(List<CorpsCeleste> celestialBodies, Cursor cursor)
        {
            CelestialBodies = celestialBodies;
            Cursor = cursor;

            SelectedCelestialBodyIndex = -1;
            SelectedTurretSpotIndex = -1;

            LastPositionSelectedCelestialBody = new Vector3(float.NaN);
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

            if (!Cursor.Actif)
                return;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Selectionnable)
                    continue;

                for (int j = 0; j < CelestialBodies[i].Emplacements.Count; j++)
                    if (Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, CelestialBodies[i].Emplacements[j].Rectangle))
                    {
                        SelectedCelestialBodyChanged = SelectedCelestialBodyIndex != i;

                        SelectedCelestialBodyIndex = i;
                        SelectedTurretSpotIndex = j;
                        break;
                    }
            }

            SelectedTurretSpotChanged = SelectedTurretSpotIndex != previousIndex;
        }


        private void doCelestialBodySelection()
        {
            if (SelectedTurretSpotIndex != -1)
                return;

            int previousIndex = SelectedCelestialBodyIndex;

            SelectedCelestialBodyIndex = -1;

            if (!Cursor.Actif)
                return;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                if (!CelestialBodies[i].Selectionnable)
                    continue;

                if (Core.Physique.Facade.collisionCercleCercle(Cursor.Cercle, CelestialBodies[i].Cercle))
                {
                    SelectedCelestialBodyIndex = i;
                    break;
                }
            }

            SelectedCelestialBodyChanged = SelectedCelestialBodyIndex != previousIndex;
        }


        public Vector3 doGlueMode()
        {
            if (SelectedCelestialBodyIndex != -1 && SelectedCelestialBodyChanged)
                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;

            if (SelectedCelestialBodyIndex != -1)
            {
                Vector3 diff = CelestialBodies[SelectedCelestialBodyIndex].Position - LastPositionSelectedCelestialBody;

                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;

                return diff;
            }

            return Vector3.Zero;
        }
    }
}
