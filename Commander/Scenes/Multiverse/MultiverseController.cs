namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Simulation;


    class MultiverseController
    {
        public bool WorldIsReady;
        public int WorldToJumpTo;

        private XmlSerializer MultiverseMessageSerializer;
        private List<DownloadWorldProtocol> SyncingWorlds;


        public MultiverseController()
        {
            MultiverseMessageSerializer = new XmlSerializer(typeof(MultiverseMessage));
            SyncingWorlds = new List<DownloadWorldProtocol>();
            WorldToJumpTo = -1;
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
                    if (sw.WorldId == WorldToJumpTo)
                        WorldIsReady = true;

                    SyncingWorlds.RemoveAt(i);
                }
            }
        }


        public MultiverseMessage GetServerAnswer(string result)
        {
            using (var reader = new StringReader(result))
                return (MultiverseMessage) MultiverseMessageSerializer.Deserialize(reader);
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


        private bool IsPlayerWorld(int id)
        {
            return id == Main.PlayersController.MultiverseData.WorldId;
        }


        private bool IsWorldExistsLocally(int id)
        {
            return Main.WorldsFactory.MultiverseWorldExistsOnDisk(id);
        }


        private bool IsWorldLoadedIntoMemory(int id)
        {
            return Main.WorldsFactory.MultiverseWorlds.ContainsKey(id);
        }
    }
}
