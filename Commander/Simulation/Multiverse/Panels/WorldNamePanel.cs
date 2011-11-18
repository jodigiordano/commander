namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldNamePanel : VerticalPanel
    {
        private TextBox WorldName;
        private TextBoxGroup TBGroup;


        public WorldNamePanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(500, 150), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("World name");

            Alpha = 0;

            WorldName = new TextBox(new Text("A", "Pixelite") { SizeX = 2 }.AbsoluteSize.Y + 10, 400) { ClickHandler = DoTextInput };

            AddWidget("worldname", WorldName);

            EnableInput();

            TBGroup = new TextBoxGroup();
            TBGroup.Add(WorldName);

            CenterWidgets = true;

            Padding = new Vector2(0, 60);

            KeyboardClosed += new NoneHandler(DoNewName);
        }


        public override void Open()
        {
            base.Open();

            WorldName.Value = Main.CurrentWorld.World.Name;
            EnableInput();
        }


        private void DoTextInput(PanelWidget p, Commander.Player player)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, false); /* Inputs.MasterPlayer.InputType == InputType.Gamepad */
        }


        private void DoNewName()
        {
            Main.CurrentWorld.World.Name = WorldName.Value;
        }
    }
}
