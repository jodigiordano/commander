namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class AlienNextWaveAnimation
    {
        public double TimeNextWave;

        private CelestialBody celestialBody;
        private Simulator Simulator;

        private Dictionary<Size, Image> NearHitMasks;
        private Image SelectedMask;
        private Metronome Metronome;

        private double MinTimeKickIn;
        private double MinTimeFlash;
        private Vector2 MinMaxFrequencyMs;
        private double Alpha;


        public AlienNextWaveAnimation(Simulator simulator)
        {
            Simulator = simulator;

            NearHitMasks = new Dictionary<Size, Image>()
            {
                { Size.Small, CreateMaskImage(GetMaskName(Size.Small)) },
                { Size.Normal, CreateMaskImage(GetMaskName(Size.Normal)) },
                { Size.Big, CreateMaskImage(GetMaskName(Size.Big)) },
            };

            Alpha = 0;
            MinTimeKickIn = 10000;
            MinTimeFlash = 3000;
            TimeNextWave = double.MaxValue;
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
            get { return TimeNextWave < MinTimeKickIn && TimeNextWave > 0; }
        }


        public void Draw()
        {
            if (celestialBody == null || !celestialBody.Alive)
                return;

            if (!Visible)
            {
                Alpha = Math.Max(0, Alpha - 0.001);

                SelectedMask.Alpha = (byte) (Alpha * 255);
                Metronome.Initialize();
                return;
            }

            if (TimeNextWave > MinTimeFlash)
            {
                double percent = 1 - ((TimeNextWave - MinTimeFlash) / (MinTimeKickIn - MinTimeFlash));
                
                Alpha = percent;
            }

            else
            {
                Metronome.FrequencyMs = Math.Max(MinMaxFrequencyMs.X, MinMaxFrequencyMs.Y * (TimeNextWave / MinTimeKickIn));
                Metronome.Update();

                Alpha = Metronome.CurvePercThisTick;
            }

            SelectedMask.Alpha = (byte) (Alpha * 255);
            SelectedMask.Position = CelestialBody.Position;
            SelectedMask.Rotation = CelestialBody.Image.Rotation;

            Simulator.Scene.Add(SelectedMask);
        }


        private string GetMaskName(Size size)
        {
            return "CBMask2" + ((size == Size.Small) ? 1 : (size == Size.Normal) ? 2 : 3).ToString();
        }


        private Image CreateMaskImage(string maskName)
        {
            return new Image(maskName)
            {
                SizeX = 6,
                Color = Colors.Default.PlanetNearHit,
                Alpha = 0
            };
        }
    }
}
