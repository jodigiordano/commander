namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;


    class MultiverseController
    {
        public event NoneHandler LoggedIn;
        public event NoneHandler LoggedOut;

        private XmlSerializer MultiverseMessageSerializer;
        private List<ServerProtocol> RunningProtocols;


        public MultiverseController()
        {
            MultiverseMessageSerializer = new XmlSerializer(typeof(MultiverseMessage));
            RunningProtocols = new List<ServerProtocol>();
        }


        public void Initialize()
        {
            foreach (var s in RunningProtocols)
                s.Cancel();

            RunningProtocols.Clear();
        }


        public void Update()
        {
            for (int i = RunningProtocols.Count - 1; i > -1; i--)
            {
                var protocol = RunningProtocols[i];

                protocol.Update();

                if (protocol.Completed)
                    RunningProtocols.RemoveAt(i);
            }
        }


        public void SaveWorld(ServerProtocolHandler callback)
        {
            var protocol = new SaveWorldProtocol(Main.PlayersController.MultiverseData.WorldId);
            protocol.Terminated += new ServerProtocolHandler(callback);
            protocol.Start();

            RunningProtocols.Add(protocol);
        }


        public MultiverseMessage GetServerAnswer(string result)
        {
            using (var reader = new StringReader(result))
                return (MultiverseMessage) MultiverseMessageSerializer.Deserialize(reader);
        }


        //public void JumpToWorld(string username, string fromScene)
        //{
        //    WorldToJumpToByUsername = username;

        //    // world is not the one of the user
        //    if (!IsPlayerWorld(username))
        //    {
        //        SyncingWorlds.Add(new DownloadWorldByUsernameProtocol(username, false));
        //        Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
        //        return;
        //    }

        //    // world is the one of the user but do not exists on disk
        //    else if (!IsWorldExistsLocally(Main.PlayersController.MultiverseData.WorldId))
        //    {
        //        SyncingWorlds.Add(new DownloadWorldByUsernameProtocol(username, false));
        //        Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
        //        return;
        //    }

        //    JumpToWorldDirectly(fromScene);
        //}


        public void JumpToWorld(int id, string fromScene)
        {
            // world doesn't exist on disk => download
            if (!IsWorldExistsLocally(id))
            {
                var protocol = new DownloadWorldProtocol(id, false);
                var scene = (WorldDownloadingScene) Core.Visual.Visuals.GetScene("WorldDownloading");
                scene.From = fromScene;
                scene.Initialize();
                protocol.Terminated += new ServerProtocolHandler(scene.DoDownloadTerminated);
                protocol.Start();
                RunningProtocols.Add(protocol);
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");

                return;
            }

            // world exists on disk but may be outdated => check & download
            if (!IsPlayerWorld(id))
            {
                var protocol = new DownloadWorldProtocol(id, true);
                var scene = (WorldDownloadingScene) Core.Visual.Visuals.GetScene("WorldDownloading");
                scene.From = fromScene;
                scene.Initialize();
                protocol.Terminated += new ServerProtocolHandler(scene.DoDownloadTerminated);
                protocol.Start();
                RunningProtocols.Add(protocol);
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");

                return;
            }

            JumpToWorldDirectly(id, fromScene);
        }


        public void JumpToWorldDirectly(int id, string fromScene)
        {
            // Set the world toLocal the WorldScene
            var world = Main.WorldsFactory.GetWorld(id);

            world.MultiverseMode = true;
            world.EditingMode = IsPlayerWorld(id);

            if (Main.CurrentWorld != null)
                Main.CurrentWorld.Simulator.OnWorldChange();

            Main.SetCurrentWorld(world, false);

            Core.Visual.Visuals.Transite(fromScene, "WorldAnnunciation");
        }


        public void Login(string username, string password, ServerProtocolHandler handler)
        {
            var protocol = new LoginProtocol(username, password);
            protocol.Terminated += new ServerProtocolHandler(handler);
            protocol.Terminated += new ServerProtocolHandler(NotifyLoggedIn);
            protocol.Start();

            RunningProtocols.Add(protocol);
        }


        public void Register(string username, string password, string email, ServerProtocolHandler handler)
        {
            var protocol = new RegisterProtocol(username, password, email);
            protocol.Terminated += new ServerProtocolHandler(handler);
            protocol.Start();

            RunningProtocols.Add(protocol);
        }


        public void LogOut()
        {
            Main.PlayersController.UpdateMultiverse("", "", -1);
            NotifyLoggedOut();
        }


        private bool IsPlayerWorld(int id)
        {
            return id == Main.PlayersController.MultiverseData.WorldId;
        }


        private bool IsPlayerWorld(string username)
        {
            return username == Main.PlayersController.MultiverseData.Username;
        }


        private bool IsWorldExistsLocally(int id)
        {
            return Main.WorldsFactory.MultiverseWorldExistsOnDisk(id) && Main.WorldsFactory.MultiverseWorldIsValidOnDisk(id);
        }


        private bool IsWorldLoadedIntoMemory(int id)
        {
            return Main.WorldsFactory.MultiverseWorlds.ContainsKey(id);
        }


        private void NotifyLoggedIn(ServerProtocol protocol)
        {
            if (protocol.State != ServerProtocol.ProtocolState.EndedWithSuccess)
                return;

            if (LoggedIn != null)
                LoggedIn();
        }


        private void NotifyLoggedOut()
        {
            if (LoggedOut != null)
                LoggedOut();
        }
    }
}
