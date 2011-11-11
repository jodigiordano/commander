namespace EphemereGames.Commander.Simulation.Player
{
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

        private SimPlayer Player;

        private Vector3 LastPositionCelestialBody;
        private Vector3 LastPositionTurret;

        private Simulator Simulator;

        public SelectedCelestialBodyController(Simulator simulator, SimPlayer player)
        {
            Simulator = simulator;
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
            // The simulator is in "cannot select celestial bodies mode"
            if (!Simulator.CanSelectCelestialBodies)
            {
                Initialize();
                return;
            }

            // The player is moving (or have the intention to)
            if (!Simulator.DemoMode && !Simulator.EditorEditingMode && Player.MovementInputThisTick)
            {
                Initialize();
                return;
            }

            // The player is firing
            if (!Simulator.DemoMode && Player.Firing)
            {
                Initialize();
                return;
            }


            if (Turret != null && Turret.Alive && Physics.CircleCicleCollision(Player.Circle, Turret.Circle))
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

            if (Simulator.EditorEditingMode &&
                Player.Position.X > Simulator.Data.Battlefield.Inner.Right - 100 &&
                Player.Position.Y < Simulator.Data.Battlefield.Inner.Top + 100)
            {
                if (PreviousCelestialBody != null)
                    PreviousCelestialBody.PlayerCheckedIn = null;

                CelestialBody = PlanetarySystemController.GetAsteroidBelt(Simulator.Data.Level.PlanetarySystem);

                if (CelestialBody != null)
                    LastPositionCelestialBody = CelestialBody.Position;
            }
        }


        private void DoTurretSelection()
        {
            PreviousTurret = Turret;
            Turret = null;
            PreviousCelestialBody = CelestialBody;

            foreach (var cb in Simulator.Data.Level.PlanetarySystem)
            {
                if (!cb.Alive || !cb.CanSelect)
                    continue;

                foreach (var t in cb.Turrets)
                {
                    if (t.Visible && t.CanSelect && t.PlayerCheckedIn == null && t.Alive && Physics.CircleCicleCollision(Player.Circle, t.Circle))
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

            foreach (var cb in Simulator.Data.Level.PlanetarySystem)
            {
                if (!cb.Alive || !cb.CanSelect)
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
            if (Simulator.EditorMode && CelestialBody != null && CelestialBody is AsteroidBelt)
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

                if (Player.ActualSelection.EditingState == EditorEditingState.MovingCB)
                    return Vector3.Zero;

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
