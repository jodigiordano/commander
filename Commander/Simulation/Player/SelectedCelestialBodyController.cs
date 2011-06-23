namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SelectedCelestialBodyController
    {
        public bool SelectedCelestialBodyChanged;
        public bool SelectedTurretChanged;

        private int SelectedCelestialBodyIndex;
        private int SelectedTurretCelestialBodyIndex;
        private int SelectedTurretIndex;
        private List<CelestialBody> CelestialBodies;
        private Circle Cercle;

        private Vector3 LastPositionSelectedCelestialBody;
        private Vector3 LastPositionSelectedTurret;


        public SelectedCelestialBodyController(List<CelestialBody> celestialBodies, Circle cercle)
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
            LastPositionSelectedTurret = Vector3.Zero;
        }


        public CelestialBody CelestialBody
        {
            get { return (SelectedTurretIndex == -1 && SelectedCelestialBodyIndex != -1) ? CelestialBodies[SelectedCelestialBodyIndex] : null; }
        }


        public Turret Turret
        {
            get { return (SelectedTurretCelestialBodyIndex != -1 && SelectedTurretIndex != -1) ? CelestialBodies[SelectedTurretCelestialBodyIndex].Turrets[SelectedTurretIndex] : null; }
        }


        public void Update()
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
                CelestialBody cb = CelestialBodies[i];

                if (!cb.Alive || !cb.Selectionnable)
                    continue;

                for (int j = 0; j < cb.Turrets.Count; j++)
                {
                    Turret t = cb.Turrets[j];

                    if (t.Visible && Physics.CircleCicleCollision(Cercle, t.Circle))
                    {
                        SelectedCelestialBodyIndex = -1;
                        SelectedTurretCelestialBodyIndex = i;
                        SelectedTurretIndex = j;
                        break;
                    }
                }
            }

            SelectedTurretChanged = SelectedTurretIndex != previousIndex;

            if (SelectedTurretChanged && SelectedTurretCelestialBodyIndex != -1 && SelectedTurretIndex != -1)
                LastPositionSelectedTurret = CelestialBodies[SelectedTurretCelestialBodyIndex].Turrets[SelectedTurretIndex].Position;
        }


        private void DoCelestialBodySelection()
        {
            if (SelectedTurretIndex != -1)
                return;

            int previousIndex = SelectedCelestialBodyIndex;

            SelectedCelestialBodyIndex = -1;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                CelestialBody cb = CelestialBodies[i];

                if (!cb.Alive || !cb.Selectionnable)
                    continue;

                if (Physics.CircleCicleCollision(Cercle, cb.Circle))
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
            CelestialBody cb = (SelectedCelestialBodyIndex >= 0 && SelectedCelestialBodyIndex < CelestialBodies.Count) ? CelestialBodies[SelectedCelestialBodyIndex] : null;
            Turret t = (SelectedTurretCelestialBodyIndex >= 0 && SelectedTurretCelestialBodyIndex < CelestialBodies.Count && SelectedTurretIndex >= 0 && SelectedTurretIndex < CelestialBodies[SelectedTurretCelestialBodyIndex].Turrets.Count) ? CelestialBodies[SelectedTurretCelestialBodyIndex].Turrets[SelectedTurretIndex] : null;

            if (t != null)
            {
                Vector3 diff = t.Position - LastPositionSelectedTurret;

                LastPositionSelectedTurret = t.Position;

                return diff;
            }

            else if (cb != null)
            {
                Vector3 diff = cb.Position - LastPositionSelectedCelestialBody;

                LastPositionSelectedCelestialBody = cb.Position;

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
