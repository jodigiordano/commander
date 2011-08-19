namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GameBarPanel : Panel
    {
        private Simulator Simulator;

        private ImageLabel CashWidget;
        //private Text CashText;
        private ImageLabel LivesWidget;
        //private Text LivesText;
        private ImageLabel RemainingWavesWidget;
        private ImageLabel TimeNextWaveText;
        private NextWaveWidget NextWaveWidget;


        private int remainingWaves;
        private double timeNextWave;


        public GameBarPanel(Simulator simulator, double visualPriority)
            : base(simulator.Scene, new Vector3(0, (-simulator.Scene.Height / 2) + 34, 0), new Vector2(simulator.Scene.Width, 45), visualPriority, Color.White)
        {
            Simulator = simulator;

            ShowCloseButton = false;
            ShowFrame = false;
            BackgroundAlpha = 100;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
            {
                Position += new Vector3(0, Preferences.Xbox360DeadZoneV2.Y, 0);
                Padding = new Vector2(10 + Preferences.Xbox360DeadZoneV2.X, 0);
            }

            CashWidget = new ImageLabel(new Image("ScoreMoney") { SizeX = 4 }, new Text("Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(50, 0, 0)
            };

            LivesWidget = new ImageLabel(new Image("ScoreLives") { SizeX = 4 }, new Text("Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(250, 0, 0)
            };

            TimeNextWaveText = new ImageLabel(new Image("ScoreTime") { SizeX = 4 }, new Text("Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(850, 0, 0)
            };

            RemainingWavesWidget = new ImageLabel(new Image("ScoreWaves") { SizeX = 4 }, new Text("Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(700, 0, 0)
            };

            NextWaveWidget = new NextWaveWidget()
            {
                Position = Position + new Vector3(1000, 0, 0)
            };

            AddWidget("Cash", CashWidget);
            AddWidget("Lives", LivesWidget);
            AddWidget("Remaining", RemainingWavesWidget);
            AddWidget("Time", TimeNextWaveText);
            AddWidget("NextWave", NextWaveWidget);
        }


        public int Cash
        {
            set { CashWidget.SetData(value.ToString()); }
        }


        public int Lives
        {
            set { LivesWidget.SetData(value.ToString()); }
        }


        public int RemainingWaves
        {
            get { return remainingWaves; }
            set
            {
                remainingWaves = value;

                RemainingWavesWidget.SetData(value.ToString());

                if (remainingWaves == 0)
                {
                    RemoveWidget("Remaining");
                    RemoveWidget("Time");
                    RemoveWidget("NextWave");
                }
            }
        }


        public double TimeNextWave
        {
            get { return timeNextWave; }
            set
            {
                timeNextWave = value;

                TimeNextWaveText.SetData(String.Format("{0:0.0}", TimeNextWave / 1000));
            }
        }


        public WaveDescriptor NextWaveComposition
        {
            set
            {
                NextWaveWidget.Composition = value;
            }
        }
    }
}
