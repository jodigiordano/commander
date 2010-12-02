namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;

    class SelectedCelestialBodyController
    {
        public Vector3 LastPositionSelectedCelestialBody;

        private Simulation Simulation;
        private int SelectedCelestialBodyIndex;
        private int SelectedTurretSpotIndex;
        private bool SelectedCelestialBodyIndexChanged;
        private bool SelectedTurretSpotIndexChanged;
        private List<CorpsCeleste> CelestialBodies;
        private Curseur Cursor;

        public delegate void SelectedCelestialBodyChangedHandler(CorpsCeleste celestialBody);
        public event SelectedCelestialBodyChangedHandler SelectedCelestialBodyChanged;

        public delegate void SelectedTurretSpotChangedHandler(Emplacement turretSpot);
        public event SelectedTurretSpotChangedHandler SelectedTurretSpotChanged;


        public SelectedCelestialBodyController(Simulation simulation, List<CorpsCeleste> celestialBodies, Curseur cursor)
        {
            Simulation = simulation;
            CelestialBodies = celestialBodies;
            Cursor = cursor;

            SelectedCelestialBodyIndex = -1;
            SelectedTurretSpotIndex = -1;

            LastPositionSelectedCelestialBody = new Vector3(float.NaN);
        }


        private void notifyCelestialBodyChanged()
        {
            if (SelectedCelestialBodyChanged != null)
                SelectedCelestialBodyChanged((SelectedCelestialBodyIndex == -1) ? null : CelestialBodies[SelectedCelestialBodyIndex]);
        }


        private void notifyTurretSpotChanged()
        {
            if (SelectedTurretSpotChanged != null)
                SelectedTurretSpotChanged((SelectedTurretSpotIndex == -1) ? null : CelestialBodies[SelectedCelestialBodyIndex].Emplacements[SelectedTurretSpotIndex]);
        }


        public void Update(GameTime gameTime)
        {
            doTurretSpotSelection();
            doCelestialBodySelection();
            doGlueMode();

            if (SelectedCelestialBodyIndexChanged)
                notifyCelestialBodyChanged();

            if (SelectedTurretSpotIndexChanged)
                notifyTurretSpotChanged();        
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
                        SelectedCelestialBodyIndexChanged = SelectedCelestialBodyIndex != i;

                        SelectedCelestialBodyIndex = i;
                        SelectedTurretSpotIndex = j;
                        break;
                    }
            }

            SelectedTurretSpotIndexChanged = SelectedTurretSpotIndex != previousIndex;
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

            SelectedCelestialBodyIndexChanged = SelectedCelestialBodyIndex != previousIndex;
        }


        private void doGlueMode()
        {
            if (SelectedCelestialBodyIndex != -1 && SelectedCelestialBodyIndexChanged)
                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;

            if (SelectedCelestialBodyIndex != -1)
            {
                Cursor.Position += (CelestialBodies[SelectedCelestialBodyIndex].Position - LastPositionSelectedCelestialBody);

                LastPositionSelectedCelestialBody = CelestialBodies[SelectedCelestialBodyIndex].Position;
            }
        }


        public void doCelestialBodyDestroyed()
        {
            int previousIndex = SelectedCelestialBodyIndex;

            SelectedCelestialBodyIndex = -1;

            if (SelectedCelestialBodyIndex != previousIndex)
                notifyCelestialBodyChanged();
        }
    }
}
