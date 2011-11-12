﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.Text;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GameBarPanel : Panel
    {
        public bool ShowOnForegroundLayer;

        private Simulator Simulator;

        private ImageLabel CashWidget;
        private ImageLabel LivesWidget;
        private ImageLabel RemainingWavesWidget;
        private ImageLabel TimeNextWaveText;
        private NextWaveWidget NextWaveWidget;

        private int remainingWaves;
        private double timeNextWave;

        private Dictionary<string, List<KeyValuePair<string, PanelWidget>>> HBMessages;
        
        private StringBuilder TimeNextWaveBuilder;


        public GameBarPanel(Simulator simulator, float height, double visualPriority)
            : base(simulator.Scene, Vector3.Zero, new Vector2(simulator.Scene.CameraView.Width, height), visualPriority, Color.White)
        {
            Simulator = simulator;

            ShowCloseButton = false;
            ShowFrame = false;
            BackgroundAlpha = 100;
            
            float widgetSize = Preferences.Target == Setting.ArcadeRoyale ? 2 : 4;

            CashWidget = new ImageLabel(new Image("ScoreMoney") { SizeX = widgetSize }, new Text(@"Pixelite") { SizeX = widgetSize - 1 })
            {
                CanHover = true
            };

            LivesWidget = new ImageLabel(new Image("ScoreLives") { SizeX = widgetSize }, new Text(@"Pixelite") { SizeX = widgetSize - 1 })
            {
                CanHover = true
            };

            TimeNextWaveText = new ImageLabel(new Image("ScoreTime") { SizeX = widgetSize }, new Text(@"Pixelite") { SizeX = widgetSize - 1 })
            {
                CanHover = true
            };

            RemainingWavesWidget = new ImageLabel(new Image("ScoreWaves") { SizeX = widgetSize }, new Text(@"Pixelite") { SizeX = widgetSize - 1 })
            {
                CanHover = true
            };

            NextWaveWidget = new NextWaveWidget(widgetSize);

            HBMessages = new Dictionary<string, List<KeyValuePair<string, PanelWidget>>>()
            {
                { @"None", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text("") { SizeX = 2 })) } },
                { @"Cash", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining money", @"Pixelite") { SizeX = 2 })) } },
                { @"Lives", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining lives", @"Pixelite") { SizeX = 2 })) } },
                { @"Remaining", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining waves", @"Pixelite") { SizeX = 2 })) } },
                { @"Time", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Remaining time before next wave", @"Pixelite") { SizeX = 2 })) } },
                { @"NextWave", new List<KeyValuePair<string, PanelWidget>>() { new KeyValuePair<string, PanelWidget>(@"msg", new Label(new Text(@"Composition of the next wave", @"Pixelite") { SizeX = 2 })) } }
            };

            TimeNextWaveBuilder = new StringBuilder();

            ShowOnForegroundLayer = false;
        }


        public override void Initialize()
        {
            ClearWidgets();

            AddWidget(@"Cash", CashWidget);
            AddWidget(@"Lives", LivesWidget);
            AddWidget(@"Remaining", RemainingWavesWidget);
            AddWidget(@"Time", TimeNextWaveText);
            AddWidget(@"NextWave", NextWaveWidget);
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

                TimeNextWaveBuilder.Length = 0;
                TimeNextWaveBuilder.AppendNumber((int) TimeNextWave / 1000);

                TimeNextWaveText.SetData(TimeNextWaveBuilder.ToString());
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


        public override void Draw()
        {
            if (ShowOnForegroundLayer)
            {
                Scene.BeginForeground();

                Position = new Vector3(-Preferences.BackBuffer.X / 2, -Preferences.BackBuffer.Y / 2 + 10, 0);
            }

            else
            {
                Position = new Vector3(Simulator.Scene.CameraView.Left, Simulator.Scene.CameraView.Top + 10, 0);
            }

            if (Preferences.Target == Setting.ArcadeRoyale)
            {
                CashWidget.Position = Position + new Vector3(50, 0, 0);
                LivesWidget.Position = Position + new Vector3(150, 0, 0);
                RemainingWavesWidget.Position = Position + new Vector3(300, 0, 0);
                TimeNextWaveText.Position = Position + new Vector3(375, 0, 0);
                NextWaveWidget.Position = Position + new Vector3(450, 0, 0);
            }

            else
            {
                CashWidget.Position = Position + new Vector3(50, 0, 0);
                LivesWidget.Position = Position + new Vector3(250, 0, 0);
                RemainingWavesWidget.Position = Position + new Vector3(700, 0, 0);
                TimeNextWaveText.Position = Position + new Vector3(850, 0, 0);
                NextWaveWidget.Position = Position + new Vector3(1050, 0, 0);
            }

            base.Draw();

            if (ShowOnForegroundLayer)
                Scene.EndForeground();
        }
    }
}