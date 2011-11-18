namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldDownloadingScene : CommanderScene
    {
        public string From;

        private Text Downloading;
        private Image Background;

        private bool TransitionInProgress;

        private bool ReadyToJump;
        private bool JumpBack;
        private int WorldId;
        private double JumpCounter;


        public WorldDownloadingScene() :
            base("WorldDownloading")
        {
            Downloading = new Text("Pixelite")
            {
                SizeX = 4,
                Color = Color.White
            };

            Background = new Image("WhiteBg", Vector3.Zero) { VisualPriority = 1 };

            ReadyToJump = false;
            JumpBack = false;
        }


        public override void Initialize()
        {
            Downloading.Data = "Downloading... please wait.";
            Downloading.Color = Colors.Panel.Waiting;
            Downloading.CenterIt();
            ReadyToJump = false;
            JumpBack = false;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (Visuals.InTransition || TransitionInProgress)
                return;

            if (ReadyToJump)
            {
                JumpCounter -= Preferences.TargetElapsedTimeMs;

                if (JumpCounter > 0)
                    return;

                if (JumpBack)
                    TransiteTo(From);
                else
                    Main.MultiverseController.JumpToWorldDirectly(WorldId, "WorldDownloading");

                TransitionInProgress = true;
            }
        }


        protected override void UpdateVisual()
        {
            Add(Downloading);
            Add(Background);
        }


        public override void OnFocus()
        {
            TransitionInProgress = false;

            Background.Color = Color.Transparent;

            VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 500));
        }


        public void DoDownloadTerminated(ServerProtocol protocol)
        {
            ReadyToJump = true;

            JumpCounter = 1000;
            WorldId = ((DownloadWorldProtocol) protocol).WorldId;

            if (protocol.State == ServerProtocol.ProtocolState.EndedWithSuccess)
            {
                Downloading.Data = "Done! Jumping...";
                Downloading.CenterIt();
                Downloading.Color = Colors.Panel.Ok;
                JumpBack = false;
            }

            else
            {
                var text = "An error occurred";

                switch (protocol.ErrorState)
                {
                    case ServerProtocol.ProtocolErrorState.IncorrectCredentials: text = "wrong credentials"; break;
                    case ServerProtocol.ProtocolErrorState.FileNotFound:
                    case ServerProtocol.ProtocolErrorState.WorldNotFound:
                        text = "world not found";
                        break;
                    case ServerProtocol.ProtocolErrorState.ServerError: text = "server down"; break;
                }

                Downloading.Data = text + "... Jumping back.";
                Downloading.CenterIt();
                Downloading.Color = Colors.Panel.Error;
                JumpBack = true;
            }
        }
    }
}
