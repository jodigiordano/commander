namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using EphemereGames.Commander.Simulation;

    
    class DownloadWorldProtocol
    {
        public World CreatedWorld       { get; private set; }
        public string ErrorMessage      { get; private set; }

        protected enum ProtocolState
        {
            None,
            AskForWorldId,
            AskForLastUpdate,
            RequestFilesToDownload,
            DownloadingFiles,
            AllFilesDownloaded,
            EndedWithSuccess,
            EndedWithError
        }

        protected WebClient Remote;
        protected ProtocolState State;
        private bool DownloadOnlyIfNewer;

        private string CurrentFileToDownload;
        private List<string> FilesToDownload;
        private List<KeyValuePair<string, string>> FilesDownloaded;

        protected int WorldId;


        public DownloadWorldProtocol(int worldId, bool onlyIfNewer)
        {
            WorldId = worldId;
            DownloadOnlyIfNewer = onlyIfNewer;

            Remote = new WebClient();
            Remote.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DataReceived);

            FilesToDownload = new List<string>();
            FilesDownloaded = new List<KeyValuePair<string, string>>();
            CurrentFileToDownload = "";

            Initialize();
        }


        protected virtual void Initialize()
        {
            GetWorldRemotely();
        }


        protected void GetWorldRemotely()
        {
            if (DownloadOnlyIfNewer)
            {
                State = ProtocolState.AskForLastUpdate;
                Remote.DownloadStringAsync(new Uri(LastUpdateScriptUrl));
            }
            else
            {
                State = ProtocolState.RequestFilesToDownload;
                Remote.DownloadStringAsync(new Uri(LoadWorldScriptUrl));
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
                ErrorMessage = "A server error occured. Please try again!";
                State = ProtocolState.EndedWithError;

                return;
            }

            var answer = Main.MultiverseController.GetServerAnswer(e.Result);

            if (VerifyErrors(answer))
            {
                State = ProtocolState.EndedWithError;

                return;
            }

            NextStep(answer);
        }


        protected virtual void NextStep(MultiverseMessage previous)
        {
            if (State == ProtocolState.AskForLastUpdate)
            {
                if (NeedUpdate(previous.Message))
                {
                    Remote.DownloadStringAsync(new Uri(LoadWorldScriptUrl));

                    State = ProtocolState.RequestFilesToDownload;
                }

                else
                {
                    CreatedWorld = Main.WorldsFactory.GetWorld(WorldId);
                    State = ProtocolState.EndedWithSuccess;
                }

                return;
            }

            if (State == ProtocolState.RequestFilesToDownload)
            {
                ParseFilesToDownload(previous);

                if (!VerifyProtocolEnded())
                    DownloadNextFile();

                State = ProtocolState.DownloadingFiles;

                return;
            }

            if (State == ProtocolState.DownloadingFiles && FilesToDownload.Count != 0)
            {
                FilesDownloaded.Add(new KeyValuePair<string, string>(CurrentFileToDownload, previous.Message));

                DownloadNextFile();

                return;
            }

            FilesDownloaded.Add(new KeyValuePair<string, string>(CurrentFileToDownload, previous.Message));
            VerifyProtocolEnded();
        }


        private bool NeedUpdate(string toConvert)
        {
            var remoteTimestamp = FormatTimestamp(toConvert);

            return Main.WorldsFactory.GetWorldLastModification(WorldId).CompareTo(remoteTimestamp) < 0;
        }


        private string FormatTimestamp(string old)
        {
           StringBuilder sb = new StringBuilder();

           foreach (char c in old)
              if (c != ' ' && c != '-' && c != ':')
                 sb.Append(c);

           return sb.ToString();
        }


        private bool VerifyErrors(MultiverseMessage answer)
        {
            // todo
            return false;
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
                CreatedWorld = CreateWorld();
                State = ProtocolState.EndedWithSuccess;
            }

            return Completed;
        }


        private World CreateWorld()
        {
            World w;

            if (FilesDownloaded.Count == 0)
                w = Main.WorldsFactory.GetEmptyWorld(WorldId, "unknown");
            else
                w = Main.WorldsFactory.LoadWorldFromStrings(FilesDownloaded);

            Main.WorldsFactory.AddMultiverseWorld(w);

            return w;
        }


        private string LoadWorldScriptUrl
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.LoadWorldScript + "?" +
                    WorldsFactory.WorldToURLArgument(WorldId) + "&" +
                    Main.PlayersController.MultiverseData.ToUrlArguments;
            }
        }


        private string LastUpdateScriptUrl
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.LastUpdateScript + "?" +
                    WorldsFactory.WorldToURLArgument(WorldId) + "&" +
                    Main.PlayersController.MultiverseData.ToUrlArguments;
            }
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


        private string WorldRemoteDirectory
        {
            get { return WorldsFactory.GetWorldMultiverseRemoteRelativeDirectory(WorldId); }
        }
    }
}
