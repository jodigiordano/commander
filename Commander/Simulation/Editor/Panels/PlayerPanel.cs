namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PlayerPanel : VerticalPanel
    {
        private Simulator Simulator;

        private NumericHorizontalSlider Lives;
        private NumericHorizontalSlider Cash;
        private NumericHorizontalSlider Minerals;
        private NumericHorizontalSlider LifePacks;
        private NumericHorizontalSlider BulletDamage;


        public PlayerPanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White)
        {
            Simulator = simulator;

            SetTitle("Player");

            Lives = new NumericHorizontalSlider("Starting lives", 0, 50, 0, 1, 350, 200) { ClickHandler = DoLives };
            Cash = new NumericHorizontalSlider("Starting money", 0, 50000, 0, 100, 350, 200) { ClickHandler = DoCash };
            Minerals = new NumericHorizontalSlider("Minerals", 0, 50000, 0, 100, 350, 200) { ClickHandler = DoMinerals };
            LifePacks = new NumericHorizontalSlider("Life packs", 0, 50, 0, 1, 350, 200) { ClickHandler = DoLifePacks };

            BulletDamage = new NumericHorizontalSlider("Bullet damage", -1, 100, 0, 1, 350, 200) { ClickHandler = DoBulletDamage };
            BulletDamage.AddAlias(-1, "automatic");

            AddWidget("Lives", Lives);
            AddWidget("Cash", Cash);
            //AddWidget("Minerals", Minerals);
            //AddWidget("LifePacks", LifePacks);
            AddWidget("BulletDamage", BulletDamage);

            Alpha = 0;

            Initialize();
        }


        public override void Open()
        {
            base.Open();

            Lives.Value = Simulator.Data.Level.Descriptor.Player.Lives;
            Cash.Value = Simulator.Data.Level.Descriptor.Player.Money;
            Minerals.Value = Simulator.Data.Level.Descriptor.Minerals.Cash;
            LifePacks.Value = Simulator.Data.Level.Descriptor.Minerals.LifePacks;
            BulletDamage.Value = (int) (Simulator.Data.Level.Descriptor.Player.BulletDamage);
        }


        private void DoLives(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorPlayerCommand("AddOrRemoveLives")
            {
                LifePoints = Lives.Value
            });
        }


        private void DoCash(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorPlayerCommand("AddOrRemoveCash")
            {
                Cash = Cash.Value
            });
        }


        private void DoMinerals(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorPlayerCommand("AddOrRemoveMinerals")
            {
                Minerals = Minerals.Value
            });
        }


        private void DoBulletDamage(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorPlayerCommand("AddOrRemoveBulletDamage")
            {
                BulletDamage = BulletDamage.Value
            });
        }


        private void DoLifePacks(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorPlayerCommand("AddOrRemoveLifePacks")
            {
                LifePacks = LifePacks.Value
            });
        }
    }
}
