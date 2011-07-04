namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SelectedCelestialBodyController
    {
        public bool SelectedCelestialBodyChanged;
        public bool SelectedTurretChanged;

        public Turret Turret                   { get; private set; }
        public CelestialBody CelestialBody     { get; private set; }

        private Turret PreviousTurret;
        private CelestialBody PreviousCelestialBody;

        private List<CelestialBody> CelestialBodies;
        private SimPlayer Player;

        private Vector3 LastPositionCelestialBody;
        private Vector3 LastPositionTurret;

        private Simulator Simulator;

        public SelectedCelestialBodyController(Simulator simulator, SimPlayer player, List<CelestialBody> celestialBodies)
        {
            Simulator = simulator;
            CelestialBodies = celestialBodies;
            Player = player;

            Initialize();
        }


        public void Initialize()
        {
            LastPositionCelestialBody = Vector3.Zero;
            LastPositionTurret = Vector3.Zero;

            if (Turret != null)
            {
                Turret.PlayerCheckedIn = null;
                Turret = null;
            }

            if (PreviousTurret != null)
            {
                PreviousTurret.PlayerCheckedIn = null;
                PreviousTurret = null;
            }

            if (CelestialBody != null)
            {
                CelestialBody.PlayerCheckedIn = null;
                CelestialBody = null;
            }

            if (PreviousCelestialBody != null)
            {
                PreviousCelestialBody.PlayerCheckedIn = null;
                PreviousCelestialBody = null;
            }
        }


        public void Update()
        {
            if (!Simulator.DemoMode && Player.NextInput != Vector3.Zero)
            {
                if (Turret != null)
                {
                    Turret.PlayerCheckedIn = null;
                    Turret = null;
                }

                if (CelestialBody != null)
                {
                    CelestialBody.PlayerCheckedIn = null;
                    CelestialBody = null;
                }

                return;
            }

            if (Turret != null && Physics.CircleCicleCollision(Player.Circle, Turret.Circle))
            {
                SelectedTurretChanged = false;
                return;
            }

            SelectedTurretChanged = true;

            DoTurretSelection();

            if (Turret != null)
                return;

            if (CelestialBody != null && Physics.CircleCicleCollision(Player.Circle, CelestialBody.Circle))
            {
                SelectedCelestialBodyChanged = false;
                return;
            }

            SelectedCelestialBodyChanged = true;

            DoCelestialBodySelection();

            if (Simulator.EditorMode &&
                Simulator.EditorState == EditorState.Editing &&
                Player.Position.X > Simulator.InnerTerrain.Right - 100 &&
                Player.Position.Y < Simulator.InnerTerrain.Top + 100)
            {
                if (PreviousCelestialBody != null)
                    PreviousCelestialBody.PlayerCheckedIn = null;

                CelestialBody = PlanetarySystemController.GetCelestialBodyWithLowestPathPriority(CelestialBodies);

                if (CelestialBody != null)
                    LastPositionCelestialBody = CelestialBody.Position;
            }
        }


        private void DoTurretSelection()
        {
            PreviousTurret = Turret;
            Turret = null;
            PreviousCelestialBody = CelestialBody;

            foreach (var cb in CelestialBodies)
            {
                if (!cb.Alive || !cb.Selectionnable)
                    continue;

                foreach (var t in cb.Turrets)
                {
                    if (t.Visible && t.PlayerCheckedIn == null && Physics.CircleCicleCollision(Player.Circle, t.Circle))
                    {
                        CelestialBody = null;
                        Turret = t;
                        Turret.PlayerCheckedIn = Player;

                        goto End;
                    }
                }
            }

            End:

            if (PreviousTurret != null)
                PreviousTurret.PlayerCheckedIn = null;

            if (PreviousCelestialBody != null)
                PreviousCelestialBody.PlayerCheckedIn = null;

            if (Turret != null)
                LastPositionTurret = Turret.Position;
        }


        private void DoCelestialBodySelection()
        {
            PreviousCelestialBody = CelestialBody;
            CelestialBody = null;

            foreach (var cb in CelestialBodies)
            {
                if (!cb.Alive || !cb.Selectionnable)
                    continue;

                if (cb.PlayerCheckedIn == null && Physics.CircleCicleCollision(Player.Circle, cb.Circle))
                {
                    CelestialBody = cb;
                    CelestialBody.PlayerCheckedIn = Player;

                    break;
                }
            }

            if (PreviousCelestialBody != null)
                PreviousCelestialBody.PlayerCheckedIn = null;

            if (CelestialBody != null)
                LastPositionCelestialBody = CelestialBody.Position;
        }


        public Vector3 DoGlueMode()
        {
            if (Simulator.EditorMode && CelestialBody != null && CelestialBody.Name == "Asteroid belt")
                return Vector3.Zero;

            if (Turret != null)
            {
                Vector3 diff = Turret.Position - LastPositionTurret;

                LastPositionTurret = Turret.Position;

                return diff;
            }

            else if (CelestialBody != null)
            {
                Vector3 diff = CelestialBody.Position - LastPositionCelestialBody;

                LastPositionCelestialBody = CelestialBody.Position;

                return diff;
            }

            return Vector3.Zero;
        }


        public void DoCelestialBodyDestroyed()
        {
            if (Turret != null)
            {
                Turret.PlayerCheckedIn = null;
                Turret = null;
            }

            if (CelestialBody != null)
            {
                CelestialBody.PlayerCheckedIn = null;
                CelestialBody = null;
            }
        }
    }
}
