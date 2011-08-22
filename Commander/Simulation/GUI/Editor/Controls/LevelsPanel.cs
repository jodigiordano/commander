namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LevelsPanel : VerticalPanel
    {
        public LevelDescriptor ClickedLevel;

        private Label SelectedLevelLabel;
        private GridPanel Levels;
        private Dictionary<LevelDescriptor, PushButton> PushButtons;


        public LevelsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SelectedLevelLabel = new Label(new Text("Selected level: none", @"Pixelite") { SizeX = 2 });
            PushButtons = new Dictionary<LevelDescriptor, PushButton>();

            Levels = new GridPanel(scene, position, size, visualPriority, color)
            {
                NbColumns = 10
            };
            Levels.OnlyShowWidgets = true;

            AddWidget("SelectedLevel", SelectedLevelLabel);
            AddWidget("Levels", Levels);
        }


        public void Initialize()
        {
            PushButtons.Clear();
            Levels.ClearWidgets();

            foreach (var d in Main.LevelsFactory.Descriptors.Values)
            {
                var button = new PushButton();
                PushButtons.Add(d, button);
                Levels.AddWidget(d.Infos.Id.ToString(), button);
            }
        }


        public override void SetClickHandler(PanelWidgetHandler handler)
        {
            ClickHandler = handler;
        }


        protected override bool Click(Circle circle)
        {
            if (!base.Click(circle))
                return false;

            foreach (var b in PushButtons)
                if (b.Value.DoClick(circle))
                {
                    ClickedLevel = b.Key;
                    return true;
                }

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            foreach (var b in PushButtons)
                if (b.Value.DoHover(circle))
                {
                    SelectedLevelLabel.SetData("Selected level: " + b.Key.Infos.Mission);
                    return true;
                }

            SelectedLevelLabel.SetData("Selected level: none");
            return false;
        }
    }
}
