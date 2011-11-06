namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Simulation;


    class MultiverseController
    {
        public event NoneHandler LoggedIn;
        public event NoneHandler LoggedOut;

        public bool WorldIsReady;
        public int WorldToJumpTo;
        private string WorldToJumpToByUsername;

        private XmlSerializer MultiverseMessageSerializer;
        private List<DownloadWorldProtocol> SyncingWorlds;


        public MultiverseController()
        {
            MultiverseMessageSerializer = new XmlSerializer(typeof(MultiverseMessage));
            SyncingWorlds = new List<DownloadWorldProtocol>();
            WorldToJumpTo = -1;
            WorldToJumpToByUsername = "";
            WorldIsReady = false;
        }


        public void Initialize()
        {
            foreach (var s in SyncingWorlds)
                s.Cancel();

            SyncingWorlds.Clear();
        }


        public void Update()
        {
            for (int i = SyncingWorlds.Count - 1; i > -1; i--)
            {
                var sw = SyncingWorlds[i];

                if (sw.Completed)
                {
                    if (sw.Success && (sw.CreatedWorld.Id == WorldToJumpTo || sw.CreatedWorld.Author == WorldToJumpToByUsername))
                    {
                        WorldToJumpTo = sw.CreatedWorld.Id;
                        WorldIsReady = true;
                    }

                    SyncingWorlds.RemoveAt(i);
                }
            }
        }


        public MultiverseMessage GetServerAnswer(string result)
        {
            using (var reader = new StringReader(result))
                return (MultiverseMessage) MultiverseMessageSerializer.Deserialize(reader);
        }


        public void JumpToWorld(string username, string fromScene)
        {
            WorldToJumpToByUsername = username;

            // world is not the one of the user
            if (!IsPlayerWorld(username))
            {
                SyncingWorlds.Add(new DownloadWorldByUsernameProtocol(username, false));
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
                return;
            }

            // world is the one of the user but do not exists on disk
            else if (!IsWorldExistsLocally(Main.PlayersController.MultiverseData.WorldId))
            {
                SyncingWorlds.Add(new DownloadWorldByUsernameProtocol(username, false));
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
                return;
            }

            JumpToWorldDirectly(fromScene);
        }


        public void JumpToWorld(int id, string fromScene)
        {
            WorldToJumpTo = id;

            // world doesn't exist on disk => download
            if (!IsWorldExistsLocally(id))
            {
                SyncingWorlds.Add(new DownloadWorldProtocol(id, false));
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
                return;
            }

            // world exists on disk but may be outdated => check & download
            if (!IsPlayerWorld(id))
            {
                SyncingWorlds.Add(new DownloadWorldProtocol(id, true));
                Core.Visual.Visuals.Transite(fromScene, "WorldDownloading");
                return;
            }

            JumpToWorldDirectly(fromScene);
        }


        public void JumpToWorldDirectly(string fromScene)
        {
            // Set the world to the WorldScene
            var world = Main.WorldsFactory.GetWorld(WorldToJumpTo);

            world.EditorMode = true;
            world.EditorState = EditorState.Playtest;

            Main.SetCurrentWorld(world, true);

            Core.Visual.Visuals.Transite(fromScene, "WorldAnnunciation");
        }


        public void LogIn(string username, string password, string worldId)
        {
            Main.PlayersController.UpdateMultiverse(username, password, worldId);
            NotifyLoggedIn();
        }


        public void LogOut()
        {
            Main.PlayersController.UpdateMultiverse("", "", "0");
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
            return Main.WorldsFactory.MultiverseWorldExistsOnDisk(id);
        }


        private bool IsWorldLoadedIntoMemory(int id)
        {
            return Main.WorldsFactory.MultiverseWorlds.ContainsKey(id);
        }


        private void NotifyLoggedIn()
        {
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
