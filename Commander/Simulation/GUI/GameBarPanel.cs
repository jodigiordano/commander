namespace EphemereGames.Commander.Simulation
{
    using System;
using System.Collections.Generic;
using EphemereGames.Core.Visual;
using Microsoft.Xna.Framework;


    class GameBarPanel : Panel
    {
        private Simulator Simulator;

        private ImageLabel CashWidget;
        private ImageLabel LivesWidget;
        private ImageLabel RemainingWavesWidget;
        private ImageLabel TimeNextWaveText;
        private NextWaveWidget NextWaveWidget;

        private int remainingWaves;
        private double timeNextWave;

        private Dictionary<string, List<KeyValuePair<string, PanelWidget>>> HBMessages;


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

            CashWidget = new ImageLabel(new Image("ScoreMoney") { SizeX = 4 }, new Text(@"Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(50, 0, 0),
                CanHover = true
            };

            LivesWidget = new ImageLabel(new Image("ScoreLives") { SizeX = 4 }, new Text(@"Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(250, 0, 0),
                CanHover = true
            };

            TimeNextWaveText = new ImageLabel(new Image("ScoreTime") { SizeX = 4 }, new Text(@"Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(850, 0, 0),
                CanHover = true
            };

            RemainingWavesWidget = new ImageLabel(new Image("ScoreWaves") { SizeX = 4 }, new Text(@"Pixelite") { SizeX = 3 })
            {
                Position = Position + new Vector3(700, 0, 0),
                CanHover = true
            };

            NextWaveWidget = new NextWaveWidget()
            {
                Position = Position + new Vector3(1050, 0, 0)
            };

            AddWidget(@"Cash", CashWidget);
            AddWidget(@"Lives", LivesWidget);
            AddWidget(@"Remaining", RemainingWavesWidget);
            AddWidget(@"Time", TimeNextWaveText);
            AddWidget(@"NextWave", NextWaveWidget);

            HBMessages = new Dictionary<string, List<KeyValuePair<string, PanelWidget>>>()
            {
                { @"None", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text("") { SizeX = 2 })) } },
                { @"Cash", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining money", @"Pixelite") { SizeX = 2 })) } },
                { @"Lives", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining lives", @"Pixelite") { SizeX = 2 })) } },
                { @"Remaining", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining waves", @"Pixelite") { SizeX = 2 })) } },
                { @"Time", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining time before next wave", @"Pixelite") { SizeX = 2 })) } },
                { @"NextWave", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Composition of the next wave", @"Pixelite") { SizeX = 2 })) } }
            };
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


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            if (LastHoverWidget == null)
                return HBMessages[@"none"];

            List<KeyValuePair<string, PanelWidget>> msg; 

            bool there = HBMessages.TryGetValue(LastHoverWidget.Name, out msg);

            if (there)
                return msg;
            else
                return HBMessages[@"none"];
        }
    }
}
