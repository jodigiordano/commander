namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class StartingPathMenu : CommanderContextualMenu
    {
        private TextContextualMenuChoice CallTheNextWave;
        private ColoredTextContextualMenuChoice RemainingEnemiesChoice;

        private int activeWaves;

        private int LastEnemiesToReleaseCount;


        public StartingPathMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            RemainingEnemiesChoice = new ColoredTextContextualMenuChoice("RemainingEnemies", new ColoredText(new List<string>() { "", "" }, new Color[] { Color.White, Color.White }, @"Pixelite", Vector3.Zero) { SizeX = 2 });
            CallTheNextWave = new TextContextualMenuChoice("CallTheNextWave", new Text("I'm ready! Bring it on!", @"Pixelite") { SizeX = 2 });

            CallTheNextWave.SetColor(owner.InnerPlayer.CMCanColor);

            AddChoice(CallTheNextWave);

            activeWaves = 0;
            LastEnemiesToReleaseCount = -1;
        }


        public override void OnOpen()
        {
            Owner.ActualSelection.CallNextWave = true;
        }


        public override void OnClose()
        {
            Owner.ActualSelection.CallNextWave = false;
        }


        public override void NextChoice()
        {
            base.NextChoice();

            Owner.ActualSelection.CallNextWave = true;
        }


        public override void PreviousChoice()
        {
            base.PreviousChoice();

            Owner.ActualSelection.CallNextWave = true;
        }


        protected int RemainingWaves
        {
            get { return Simulator.Data.RemainingWaves; }
        }


        public override List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            return RemainingWaves > 0 ?
            Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.CallNextWave) :
            Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.None);
        }

         
        private int RemainingEnemies
        {
            set
            {
                if (RemainingWaves != 0)
                    return;

                RemainingEnemiesChoice.SetData(new List<string>() { "Remaining enemies: ", value.ToString() });
            }
        }


        private int ActiveWaves
        {
            set
            {
                bool change = activeWaves != value;

                activeWaves = value;

                if (change)
                    CallTheNextWave.SetData(value >= 3 ? "3 waves at once maximum!" : "I'm ready! Bring it on!");
            }
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    !Simulator.EditingMode &&
                    Simulator.GameMode &&
                    RemainingWaves > 0 &&
                    Owner.ActualSelection.CelestialBody != null &&
                    Owner.ActualSelection.CelestialBody.FirstOnPath;
            }

            set { base.Visible = value; }
        }


        public override void Update()
        {
            // sync enemies toLocal release
            int enemiesToReleaseCount = 0;

            foreach (var w in Simulator.Data.ActiveWaves)
                enemiesToReleaseCount += w.EnemiesToCreateCount;

            if (enemiesToReleaseCount != LastEnemiesToReleaseCount)
                RemainingEnemies = enemiesToReleaseCount;

            LastEnemiesToReleaseCount = enemiesToReleaseCount;

            // sync remaining time for next wave
            ActiveWaves = Simulator.Data.ActiveWaves.Count;
        }
    }
}
