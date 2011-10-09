namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class StartingPathMenu
    {
        public CelestialBody CelestialBody;
        public ContextualMenu Menu;
        
        
        private Simulator Simulator;
        private double VisualPriority;

        private TextContextualMenuChoice CallTheNextWave;
        //private NextWaveContextualMenuChoice NextWaveCompositionChoice;
        //private ColoredTextContextualMenuChoice RemainingWavesChoice;
        private ColoredTextContextualMenuChoice RemainingEnemiesChoice;
        //private ColoredTextContextualMenuChoice TimeNextWaveChoice;

        private int remainingWaves;
        private int activeWaves;
        private double timeNextWave;

        private SimPlayer checkedIn;


        public StartingPathMenu(Simulator simulator, double visualPriority)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;

            
            //RemainingWavesChoice = new ColoredTextContextualMenuChoice("RemainingWaves", new ColoredText(new List<string>() { "", "", "" }, new Color[] { Color.White, Color.White, Color.White }, @"Pixelite", Vector3.Zero) { SizeX = 2 });
            RemainingEnemiesChoice = new ColoredTextContextualMenuChoice("RemainingEnemies", new ColoredText(new List<string>() { "", "" }, new Color[] { Color.White, Color.White }, @"Pixelite", Vector3.Zero) { SizeX = 2 });
            //NextWaveCompositionChoice = new NextWaveContextualMenuChoice("NextWaveComposition");
            //TimeNextWaveChoice = new ColoredTextContextualMenuChoice("TimeRemainingChoice", new ColoredText(new List<string>() { "", "", "" }, new Color[] { Color.White, Color.White, Color.White }, @"Pixelite", Vector3.Zero) { SizeX = 2 });
            CallTheNextWave = new TextContextualMenuChoice("CallTheNextWave", new Text("I'm ready! Bring it on!", @"Pixelite") { SizeX = 2 });

            var choices = new List<ContextualMenuChoice>();

            //choices.Add(RemainingWavesChoice);
            //choices.Add(TimeNextWaveChoice);
            //choices.Add(RemainingEnemiesChoice);
            //choices.Add(NextWaveCompositionChoice);
            choices.Add(CallTheNextWave);

            Menu = new ContextualMenu(Simulator, VisualPriority, Color.White, choices, 5) { SelectedIndex = 0 };

            remainingWaves = 0;
            timeNextWave = 0;
            activeWaves = 0;

            CheckedIn = null;
        }


        public Color Color
        {
            set
            {
                Menu.Color = value;
            }
        }


        public SimPlayer CheckedIn
        {
            get { return checkedIn; }
            set
            {
                checkedIn = value;

                if (value != null)
                {
                    //RemainingWavesChoice.SetColors(new Color[] { Color.White, value.Color, Color.White });
                    //TimeNextWaveChoice.SetColors(new Color[] { Color.White, value.Color, Color.White });
                    RemainingEnemiesChoice.SetColor(1, value.Color);
                    //NextWaveCompositionChoice.Color = value.Color;


                    if (value.Color == Colors.Spaceship.Yellow)
                        CallTheNextWave.SetColor(Colors.Spaceship.Selected);
                    else
                        CallTheNextWave.SetColor(Color.White);
                }
            }
        }


        public List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage(Commander.Player p)
        {
            return RemainingWaves > 0 ?
            Simulator.HelpBar.GetPredefinedMessage(p, HelpBarMessage.CallNextWave) :
            Simulator.HelpBar.GetPredefinedMessage(p, HelpBarMessage.None);
        }



        public bool Visible
        {
            get { return Menu.Visible; }
            set { Menu.Visible = value; }
        }


        public Bubble Bubble
        {
            get { return Menu.Bubble; }
        }


        public Vector3 Position
        {
            set { Menu.Position = value; }
        }


        public void Update()
        {
            if (CheckedIn != null)
                Position = CheckedIn.Position;
        }


        public int RemainingWaves
        {
            get
            {
                return remainingWaves;
            }

            set
            {
                remainingWaves = value;

                //RemainingWavesChoice.SetData(new List<string>(){"Reporting ", remainingWaves.ToString(), " remaining waves."});

                if (value == 0)
                {
                    Menu.Clear();
                    Menu.AddChoice(RemainingEnemiesChoice);
                    Menu.SelectedIndex = -1;
                }
            }
        }


        public double TimeNextWave
        {
            get
            {
                return timeNextWave;
            }

            set
            {
                timeNextWave = value;

                //TimeNextWaveChoice.SetData(new List<string>() { "Next one due in ", String.Format("{0:0.00}", value / 1000.0), " seconds." });
            }
        }

         
        public int RemainingEnemies
        {
            set
            {
                if (RemainingWaves != 0)
                    return;

                RemainingEnemiesChoice.SetData(new List<string>() { "Remaining enemies: ", value.ToString() });
            }
        }


        public WaveDescriptor NextWaveComposition
        {
            set
            {
                //NextWaveCompositionChoice.Composition = value;
            }
        }


        public int ActiveWaves
        {
            set
            {
                bool change = activeWaves != value;

                activeWaves = value;

                if (change)
                    CallTheNextWave.SetData(value >= 3 ? "3 waves at once maximum!" : "I'm ready! Bring it on!");
            }
        }


        public void Draw()
        {
            if (!Visible)
                return;

            if (CelestialBody == null)
                return;

            Menu.Draw();
        }
    }
}
