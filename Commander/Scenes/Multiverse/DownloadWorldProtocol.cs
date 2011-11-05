namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using EphemereGames.Commander.Simulation;

    
    class DownloadWorldProtocol
    {
        public int WorldId              { get; private set; }

        private enum ProtocolState
        {
            None,
            AskForLastUpdate,
            RequestFilesToDownload,
            DownloadingFiles,
            AllFilesDownloaded,
            EndedWithSuccess,
            EndedWithError
        }

        private WebClient Remote;
        private ProtocolState State;
        private bool DownloadOnlyIfNewer;

        private string WorldRemoteDirectory;

        private string CurrentFileToDownload;
        private List<string> FilesToDownload;
        private List<KeyValuePair<string, string>> FilesDownloaded;


        public DownloadWorldProtocol(int worldId, bool onlyIfNewer)
        {
            WorldId = worldId;
            DownloadOnlyIfNewer = onlyIfNewer;

            WorldRemoteDirectory = WorldsFactory.GetWorldMultiverseRemoteRelativeDirectory(WorldId);

            Remote = new WebClient();
            Remote.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataReceived);

            FilesToDownload = new List<string>();
            FilesDownloaded = new List<KeyValuePair<string, string>>();
            CurrentFileToDownload = "";

            if (DownloadOnlyIfNewer)
            {
                Remote.DownloadStringAsync(new Uri(GetLastUpdateScriptUrl()));
                State = ProtocolState.AskForLastUpdate;
            }
            else
            {
                Remote.DownloadStringAsync(new Uri(GetLoadWorldScriptUrl()));
                State = ProtocolState.RequestFilesToDownload;
            }
        }


        public void Cancel()
        {
            Remote.CancelAsync();
        }


        public bool Completed
        {
            get { return State == ProtocolState.EndedWithSuccess || State == ProtocolState.EndedWithError; }
        }


        public bool Success
        {
            get { return State == ProtocolState.EndedWithSuccess; }
        }


        private void DataReceived(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                State = ProtocolState.EndedWithError;

                return;
            }

            var answer = Main.MultiverseController.GetServerAnswer(e.Result);

            // todo: possible errors.

            if (State == ProtocolState.AskForLastUpdate)
            {
                if (Main.WorldsFactory.GetWorldLastModification(WorldId).CompareTo(answer.Message) < 0)
                {
                    Remote.DownloadStringAsync(new Uri(GetLoadWorldScriptUrl()));

                    State = ProtocolState.RequestFilesToDownload;
                }

                else
                {
                    State = ProtocolState.EndedWithSuccess;
                }

                return;
            }

            if (State == ProtocolState.RequestFilesToDownload)
            {
                ParseFilesToDownload(answer);

                if (!VerifyProtocolEnded())
                    DownloadNextFile();

                State = ProtocolState.DownloadingFiles;

                return;
            }

            if (State == ProtocolState.DownloadingFiles && FilesToDownload.Count != 0)
            {
                FilesDownloaded.Add(new KeyValuePair<string, string>(CurrentFileToDownload, answer.Message));

                DownloadNextFile();

                return;
            }


            VerifyProtocolEnded();
        }


        private void DownloadNextFile()
        {
            if (FilesToDownload.Count == 0)
            {
                State = ProtocolState.AllFilesDownloaded;
                return;
            }

            CurrentFileToDownload = FilesToDownload[FilesToDownload.Count - 1];
            FilesToDownload.RemoveAt(FilesToDownload.Count - 1);

            Remote.DownloadStringAsync(new Uri(GetDownloadFileScriptUrl(CurrentFileToDownload)));
        }


        private void ParseFilesToDownload(MultiverseMessage message)
        {
            FilesToDownload.AddRange(message.Message.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
        }


        private bool VerifyProtocolEnded()
        {
            if (State == ProtocolState.DownloadingFiles && FilesToDownload.Count == 0)
            {
                CreateWorld();
                State = ProtocolState.EndedWithSuccess;
            }

            return Completed;
        }


        private void CreateWorld()
        {
            if (FilesDownloaded.Count == 0)
                Main.WorldsFactory.AddMultiverseWorld(Main.WorldsFactory.GetEmptyWorld(WorldId));
            else
                Main.WorldsFactory.AddMultiverseWorld(Main.WorldsFactory.LoadWorldFromStrings(FilesDownloaded));
        }


        private string GetLoadWorldScriptUrl()
        {
            return
                Preferences.WebsiteURL +
                Preferences.MultiverseScriptsURL +
                Preferences.LoadWorldScript + "?" +
                WorldsFactory.WorldToURLArgument(WorldId) + "&" +
                Main.PlayersController.MultiverseData.ToUrlArguments;
        }


        private string GetLastUpdateScriptUrl()
        {
            return
                Preferences.WebsiteURL +
                Preferences.MultiverseScriptsURL +
                Preferences.LastUpdateScript + "?" +
                WorldsFactory.WorldToURLArgument(WorldId) + "&" +
                Main.PlayersController.MultiverseData.ToUrlArguments;
        }


        private string GetDownloadFileScriptUrl(string file)
        {
            return
                Preferences.WebsiteURL +
                Preferences.MultiverseScriptsURL +
                Preferences.DownloadFileScript +
                "?file=" + WorldRemoteDirectory + @"/" + file + "&" +
                Main.PlayersController.MultiverseData.ToUrlArguments;
        }
    }
}
