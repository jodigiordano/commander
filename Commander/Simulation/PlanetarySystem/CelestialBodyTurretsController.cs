namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    
    class CelestialBodyTurretsController
    {
        public Simulator Simulator;
        public bool FakeHasGravitationalTurret;
        public bool FakeHasGravitationalTurretLv2;
        public Circle InnerTurretZone;
        public Circle OuterTurretZone;
        public bool ShowTurretsZone;
        public List<Turret> Turrets;
        public Turret StartingPathTurret;
        private Image TurretsZoneImage;

        private CelestialBody CelestialBody;


        public CelestialBodyTurretsController(CelestialBody celestialBody)
        {
            CelestialBody = celestialBody;

            Turrets = new List<Turret>();

            FakeHasGravitationalTurretLv2 = false;
            InnerTurretZone = new Circle(CelestialBody.Position, 0);
            OuterTurretZone = new Circle(CelestialBody.Position, 0);
            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = new Color(255, 255, 255, 100);
            TurretsZoneImage.VisualPriority = VisualPriorities.Default.TurretRange;
            ShowTurretsZone = false;
        }


        public void Update()
        {
            InnerTurretZone.Position = CelestialBody.Position;
            OuterTurretZone.Position = CelestialBody.Position;
        }


        public void Draw()
        {
            if (ShowTurretsZone)
            {
                TurretsZoneImage.Position = OuterTurretZone.Position;
                TurretsZoneImage.SizeX = (OuterTurretZone.Radius / 100) * 2;

                Simulator.Scene.Add(TurretsZoneImage);
            }
        }


        public void AddToStartingPath(bool visible)
        {
            if (StartingPathTurret != null)
                return;

            var t = Simulator.TurretsFactory.Create(TurretType.Gravitational);

            t.CanSell = visible;
            t.CanUpdate = visible;
            t.Level = 1;
            t.BackActiveThisTickOverride = true;
            t.Visible = visible;
            t.CelestialBody = CelestialBody;
            t.RelativePosition = CelestialBody.Circle.NearestPointToCircumference(
                new Vector3(
                    Main.Random.Next((int) -CelestialBody.Circle.Radius * 2, (int) CelestialBody.Circle.Radius * 2),
                    Main.Random.Next((int) -CelestialBody.Circle.Radius * 2, (int) CelestialBody.Circle.Radius * 2), 0));

            Turrets.Add(t);

            StartingPathTurret = t;
        }


        public void RemoveFromStartingPath()
        {
            if (StartingPathTurret == null)
                return;

            Turrets.Remove(StartingPathTurret);

            StartingPathTurret = null;
        }


        public bool HasGravitationalTurret
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational)
                        return true;

                return false;
            }
        }


        public bool HasLevel2GravitationalTurret
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational && Turrets[i].Level >= 2)
                        return true;

                return false;
            }
        }


        public Turret GravitationalTurret
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational)
                        return Turrets[i];

                return null;
            }
        }

        internal void SetSize(int size)
        {
            if (size <= (int) Size.Small)
            {
                InnerTurretZone.Radius = size;
                OuterTurretZone.Radius = size * 1.4f;
            }

            else if (size <= (int) Size.Normal)
            {
                InnerTurretZone.Radius = size * 0.9f;
                OuterTurretZone.Radius = size * 1.2f;
            }

            else
            {
                InnerTurretZone.Radius = size * 0.9f;
                OuterTurretZone.Radius = size * 1.0f;
            }
        }
    }
}
