namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyNearHitAnimation
    {
        public EnemiesData EnemiesData;

        private CelestialBody celestialBody;
        private Simulator Simulator;

        private Dictionary<Size, Image> NearHitMasks;
        private Image SelectedMask;
        private Metronome Metronome;

        private double MinEnemyPerc;
        private Vector2 MinMaxFrequencyMs;
        private double Alpha;


        public CelestialBodyNearHitAnimation(Simulator simulator)
        {
            Simulator = simulator;

            NearHitMasks = new Dictionary<Size, Image>()
            {
                { Size.Small, CreateMaskImage(GetMaskName(Size.Small)) },
                { Size.Normal, CreateMaskImage(GetMaskName(Size.Normal)) },
                { Size.Big, CreateMaskImage(GetMaskName(Size.Big)) },
            };

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
            get { return EnemiesData.EnemyNearHitPerc >= MinEnemyPerc; }
        }


        public void Draw()
        {
            if (celestialBody == null || !celestialBody.Alive)
                return;

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


        private double GetEnemyRelativePerc()
        {
            return (EnemiesData.EnemyNearHitPerc - MinEnemyPerc) / (1 - MinEnemyPerc);
        }
    }
}
