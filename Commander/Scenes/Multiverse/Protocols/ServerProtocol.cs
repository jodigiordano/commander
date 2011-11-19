namespace EphemereGames.Commander
{
    using System;
    using System.ComponentModel;
    using System.Net;


    abstract class ServerProtocol
    {
        public ProtocolState State                          { get; protected set; }
        public ProtocolErrorState ErrorState                { get; private set; }

        public event ServerProtocolHandler Terminated;


        public enum ProtocolState
        {
            None,
            Executing,
            EndedWithSuccess,
            EndedWithError
        }

        public enum ProtocolErrorState
        {
            None,
            IncorrectCredentials,
            WorldNotFound,
            ServerError,
            EmailNotValid,
            UsernameLength,
            PasswordLength,
            UsernameAlreadyTaken,
            FileNotUploaded,
            FileNotFound
        }

        private WebClient Remote;
        private double TimeoutCounter;
        private double Timeout;


        public ServerProtocol()
        {
            Remote = new WebClient();
            Remote.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDataReceived);
            Remote.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloaded);
            Remote.UploadFileCompleted += new UploadFileCompletedEventHandler(FileUploaded);

            State = ProtocolState.None;
            ErrorState = ProtocolErrorState.None;

            Timeout = 20000;
            TimeoutCounter = 0;
        }

        
        public abstract void Start();


        public void Cancel()
        {
            Remote.CancelAsync();
            State = ProtocolState.EndedWithError;
            NotifyProtocolTerminated();
        }


        public bool Completed
        {
            get { return State == ProtocolState.EndedWithSuccess || State == ProtocolState.EndedWithError; }
        }


        public void Update()
        {
            if (Completed)
                return;

            TimeoutCounter += Preferences.TargetElapsedTimeMs;

            if (TimeoutCounter > Timeout)
                Cancel();
        }


        protected void AddQuery(string query)
        {
            Remote.DownloadStringAsync(new Uri(query));
        }


        protected void DownloadFile(string fromRemote, string toLocal)
        {
            Remote.DownloadFileAsync(new Uri(fromRemote), toLocal);
        }


        protected void UploadFile(string toRemote, string fromLocal)
        {
            Remote.UploadFileAsync(new Uri(toRemote), fromLocal);
        }


        void FileUploaded(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                State = ProtocolState.EndedWithError;
                ErrorState = ProtocolErrorState.ServerError;
                NotifyProtocolTerminated();

                return;
            }

            DoNextStep(new MultiverseMessage() { Type = MultiverseMessageType.FileUploaded });
        }


        private void FileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                State = ProtocolState.EndedWithError;

                if (e.Error.Message.Contains("404"))
                    ErrorState = ProtocolErrorState.FileNotFound;
                else
                    ErrorState = ProtocolErrorState.ServerError;

                NotifyProtocolTerminated();

                return;
            }

            DoNextStep(new MultiverseMessage() { Type = MultiverseMessageType.FileDownloaded });
        }


        private void StringDataReceived(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                State = ProtocolState.EndedWithError;
                ErrorState = ProtocolErrorState.ServerError;
                NotifyProtocolTerminated();

                return;
            }

            DataReceived(e.Result);
        }


        private void DataReceived(string response)
        {
            var answer = Main.MultiverseController.GetServerAnswer(response);

            if (VerifyErrors(answer))
            {
                State = ProtocolState.EndedWithError;
                NotifyProtocolTerminated();

                return;
            }

            DoNextStep(answer);
        }


        protected virtual void DoNextStep(MultiverseMessage previous)
        {
            TimeoutCounter = 0;

            State = ProtocolState.EndedWithSuccess;
            DoProtocolEndedWithSuccess();
            NotifyProtocolTerminated();
        }


        private bool VerifyErrors(MultiverseMessage answer)
        {
            if (answer.Type != MultiverseMessageType.Error)
                return false;

            switch (answer.Message)
            {
                case "credentials": ErrorState = ProtocolErrorState.IncorrectCredentials; break;
                case "world not found": ErrorState = ProtocolErrorState.WorldNotFound; break;
                case "world_not_found": ErrorState = ProtocolErrorState.WorldNotFound; break;
                case "server down.": ErrorState = ProtocolErrorState.ServerError; break;
                case "server down": ErrorState = ProtocolErrorState.ServerError; break;
                case "email not valid": ErrorState = ProtocolErrorState.EmailNotValid; break;
                case "username length": ErrorState = ProtocolErrorState.UsernameLength; break;
                case "password length": ErrorState = ProtocolErrorState.PasswordLength; break;
                case "username already taken": ErrorState = ProtocolErrorState.UsernameAlreadyTaken; break;
                case "file not uploaded": ErrorState = ProtocolErrorState.FileNotUploaded; break;
            }

            return true;
        }


        protected virtual void DoProtocolEndedWithSuccess() { }


        private void NotifyProtocolTerminated()
        {
            if (Terminated != null)
                Terminated(this);
        }
    }
}
