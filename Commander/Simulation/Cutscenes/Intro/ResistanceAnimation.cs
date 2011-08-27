namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using Microsoft.Xna.Framework;


    class ResistanceAnimation
    {
        enum ResistanceState
        {
            None,
            Arrival,
            Firing,
            Destruction
        }


        public List<HumanBattleship> Battleships;
        private Simulator Simulator;
        private Mothership Mothership;
        private ResistanceState State;

        private double TimeBeforeArrival;
        private double TimeBeforeFiring;
        private double TimeBeforeDestruction;


        public ResistanceAnimation(Simulator simulator, Mothership mothership)
        {
            Simulator = simulator;
            Mothership = mothership;

            Battleships = new List<HumanBattleship>();

            var b = new HumanBattleship(Simulator, "HumanBattleship1", VisualPriorities.Cutscenes.IntroHumanBattleships)
            {
                Position = new Vector3(0, 2000, 0)
            };
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Basic, 1, new Vector3(-18, -25, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Basic, 1, new Vector3(-18, -10, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.RailGun, 2, new Vector3(20, -25, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Missile, 4, new Vector3(20, -10, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Basic, 2, new Vector3(0, 0, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.SlowMotion, 1, new Vector3(0, -20, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.SlowMotion, 2, new Vector3(0, 15, 0) * 4, true, false, false, false));
            Battleships.Add(b);

            b = new HumanBattleship(Simulator, "HumanBattleship2", VisualPriorities.Cutscenes.IntroHumanBattleships)
            {
                Position = new Vector3(0, 2000, 0)
            };
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Gravitational, 5, new Vector3(-18, -15, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.RailGun, 3, new Vector3(-18, 0, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Gravitational, 6, new Vector3(18, -15, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.RailGun, 7, new Vector3(18, 0, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.SlowMotion, 6, new Vector3(-18, 15, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Basic, 7, new Vector3(18, 15, 0) * 4, true, false, false, false));
            Battleships.Add(b);

            b = new HumanBattleship(Simulator, "HumanBattleship3", VisualPriorities.Cutscenes.IntroHumanBattleships)
            {
                Position = new Vector3(0, 2000, 0)
            };
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Missile, 3, new Vector3(18, -10, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.RailGun, 5, new Vector3(-18, -10, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.SlowMotion, 5, new Vector3(9, 20, 0) * 4, true, false, false, false));
            b.AddTurret(Simulator.TurretsFactory.Create(TurretType.Gravitational, 3, new Vector3(-9, 20, 0) * 4, true, false, false, false));
            Battleships.Add(b);

            foreach (var ship in Battleships)
                Simulator.AddSpaceship(ship);

            State = ResistanceState.None;

            TimeBeforeArrival = IntroCutscene.Timing["HumanBattleshipsArrival"];
            TimeBeforeFiring = IntroCutscene.Timing["HumanBattleshipsFiring"];
            TimeBeforeDestruction = IntroCutscene.Timing["HumanBattleshipsDestruction"];
        }


        public void Update()
        {
            TimeBeforeArrival -= Preferences.TargetElapsedTimeMs;
            TimeBeforeFiring -= Preferences.TargetElapsedTimeMs;
            TimeBeforeDestruction -= Preferences.TargetElapsedTimeMs;

            foreach (var b in Battleships)
                b.Update();

            switch (State)
            {
                case ResistanceState.None:
                    if (TimeBeforeArrival <= 0)
                    {
                        MoveBattleships();

                        State = ResistanceState.Firing;
                    }

                    break;
                case ResistanceState.Firing:
                    if (TimeBeforeFiring <= 0)
                        FireBattleships();

                    if (TimeBeforeDestruction < 0)
                    {
                        StopFireBattleships();
                        ScaredBattleships();

                        State = ResistanceState.Destruction;
                    }

                    break;
                case ResistanceState.Destruction:
                    break;
            }
        }


        public void Draw()
        {
            foreach (var hbs in Battleships)
                hbs.Draw();
        }


        private void MoveBattleships()
        {
            for (int i = 0; i < Battleships.Count; i++)
            {
                var b = Battleships[i];
                var positions = ArrivalPositions[i];

                b.Position = positions.Key;
                b.Direction = new Vector3(0, -1, 0);

                var effect = Core.Physics.PhysicalEffects.Arrival(positions.Value, 0, 2000);

                Simulator.Scene.PhysicalEffects.Add(b, effect);
            }
        }


        private void FireBattleships()
        {
            foreach (var b in Battleships)
                b.SetTarget(Mothership);
        }


        private void StopFireBattleships()
        {
            foreach (var b in Battleships)
                b.SetTarget(null);
        }


        private void ScaredBattleships()
        {
            foreach (var b in Battleships)
                Simulator.Scene.PhysicalEffects.Add(b, Core.Physics.PhysicalEffects.Move(b.Position + new Vector3(0, 35, 0), 2000, 6000));
        }


        private static List<KeyValuePair<Vector3, Vector3>> ArrivalPositions = new List<KeyValuePair<Vector3, Vector3>>()
        {
            new KeyValuePair<Vector3, Vector3>(new Vector3(-400, 800, 0), new Vector3(-400, 350, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(0, 800, 0), new Vector3(0, 275, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(400, 800, 0), new Vector3(400, 350, 0))
        };
    }
}
