namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyNearHitAnimation
    {
        private double EnemyPerc;

        private CelestialBody celestialBody;
        private Simulator Simulator;
        private List<Enemy> Enemies;
        private Path Path;

        private Dictionary<Size, Image> NearHitMasks;
        private Image SelectedMask;
        private Metronome Metronome;

        private double MinEnemyPerc;
        private Vector2 MinMaxFrequencyMs;
        private double Alpha;


        public CelestialBodyNearHitAnimation(Simulator simulator, List<Enemy> enemies, Path path)
        {
            Simulator = simulator;
            Enemies = enemies;
            Path = path;

            NearHitMasks = new Dictionary<Size, Image>()
            {
                { Size.Small, CreateMaskImage(GetMaskName(Size.Small)) },
                { Size.Normal, CreateMaskImage(GetMaskName(Size.Normal)) },
                { Size.Big, CreateMaskImage(GetMaskName(Size.Big)) },
            };

            EnemyPerc = 0;
            Alpha = 0;
            MinEnemyPerc = 0.7;
            MinMaxFrequencyMs = new Vector2(500, 2000);
            Metronome = Metronome.Create(CurveType.Sine, Preferences.TargetElapsedTimeMs, Preferences.TargetElapsedTimeMs);
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set
            {
                celestialBody = value;

                if (celestialBody == null)
                    return;

                SelectedMask = NearHitMasks[celestialBody.Size];
                SelectedMask.VisualPriority = celestialBody.VisualPriority - 0.000001;
            }
        }


        public bool Visible
        {
            get { return EnemyPerc >= MinEnemyPerc; }
        }


        public void Draw()
        {
            if (celestialBody == null || !celestialBody.Alive)
                return;

            ComputeNearestEnemyPerc();

            if (!Visible)
            {
                Alpha = Math.Max(0, Alpha - 0.005);

                SelectedMask.Alpha = (byte) (Alpha * 255);
                Metronome.Initialize();

                if (Alpha != 0)
                    AddToScene();

                return;
            }

            double enemyPerc = GetEnemyRelativePerc();

            if (enemyPerc < 0.6)
            {
                double current = enemyPerc / 0.6;
                double delta = MathHelper.Clamp((float) (current - Alpha), -0.005f, 0.005f);

                Alpha += delta;
            }

            else
            {
                Metronome.FrequencyMs = Math.Max(MinMaxFrequencyMs.X, MinMaxFrequencyMs.Y * (1 - enemyPerc));
                Metronome.Update();

                Alpha = Metronome.CurvePercThisTick;
            }

            AddToScene();
        }


        private void AddToScene()
        {
            SelectedMask.Alpha = (byte) (Alpha * 255);
            SelectedMask.Position = CelestialBody.Position;

            Simulator.Scene.Add(SelectedMask);
        }


        private string GetMaskName(Size size)
        {
            return "CBMask1" + ((size == Size.Small) ? 1 : (size == Size.Normal) ? 2 : 3).ToString();
        }


        private Image CreateMaskImage(string maskName)
        {
            return new Image(maskName)
            {
                SizeX = 6,
                Color = Colors.Default.PlanetNearHit,
                Blend = BlendType.Add,
                Alpha = 0
            };
        }


        private void ComputeNearestEnemyPerc()
        {
            EnemyPerc = 0;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy e = Enemies[i];

                double displacementPerc = Path.GetPercentage(e.Displacement);

                if (displacementPerc > EnemyPerc)
                    EnemyPerc = displacementPerc;
            }
        }


        private double GetEnemyRelativePerc()
        {
            return (EnemyPerc - MinEnemyPerc) / (1 - MinEnemyPerc);
        }
    }
}
