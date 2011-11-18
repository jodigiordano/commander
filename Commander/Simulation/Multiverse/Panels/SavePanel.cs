namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SavePanel : VerticalPanel
    {
        private PushButton Submit;
        private Label Message;

        private Commander.Player SavePlayer;


        public SavePanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(500, 200), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Save world");

            Alpha = 0;

            Message = new Label(new Text("Pixelite") { SizeX = 2 });
            Submit = new PushButton(new Text("retry", "Pixelite") { SizeX = 2 }, 200) { ClickHandler = DoSubmit };

            AddWidget("submit", Submit);
            AddWidget("message", Message);
        }


        public override void Open()
        {
            base.Open();

            Save();
        }


        private void DoSubmit(PanelWidget p, Commander.Player player)
        {
            SavePlayer = player;
            Save();
        }


        private void Save()
        {
            Main.WorldsFactory.SaveWorldOnDisk(Main.PlayersController.MultiverseData.WorldId);
            Main.MultiverseController.SaveWorld(CompletedCallback);

            Message.Value = "Saving... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private void CompletedCallback(ServerProtocol protocol)
        {
            if (protocol.State == ServerProtocol.ProtocolState.EndedWithError)
            {
                Message.Value = "Remote save failed.\n\nPlease retry.";
                Message.Color = Colors.Panel.Error;
                EnableInput();

                return;
            }

            Message.Value = "World saved!";
            Message.Color = Colors.Panel.Ok;

            CloseButtonHandler(this, SavePlayer);
        }
    }
}
