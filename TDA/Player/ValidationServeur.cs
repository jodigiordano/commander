namespace TDA
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Xna.Framework;
    using System.Text;
    using System.Threading;

    class ValidationServeur
    {
        private const string SCRIPT_URL = "http://www.ephemeregames.com/utilities/verifykey.php";

        private WebClient ClientWeb;
        private string RequeteUrl;
        private double Timeout;
        private bool ByPass;
        private Thread ThreadConnection;

        public bool ValidationTerminee  { get; private set; }
        public bool Valide              { get; private set; }
        public string Message           { get; private set; }
        public bool ErreurSurvenue      { get; private set; }
        public bool DelaiExpire         { get { return Timeout <= 0; } }


        public ValidationServeur(string product, string productKey)
        {
            ByPass = !Preferences.HomeMadeValidation;

            if (ByPass)
            {
                ValidationTerminee = true;
                Valide = true;
                ErreurSurvenue = false;
                Message = "";
                return;
            }

            RequeteUrl = SCRIPT_URL + "?productkey=" + productKey + "&product=" + product;

            ValidationTerminee = false;
            Valide = false;
            ErreurSurvenue = false;
            Message = "";

            ClientWeb = new WebClient();
            ClientWeb.DownloadDataCompleted += new DownloadDataCompletedEventHandler(telechargementTermine);

            Timeout = 10000;

            ThreadConnection = new Thread(doConnection);
            ThreadConnection.IsBackground = true;
        }


        public void Update(GameTime gameTime)
        {
            if (ByPass)
                return;

            Timeout = Math.Max(0, Timeout - gameTime.ElapsedGameTime.TotalMilliseconds);
        }


        public void valider()
        {
            if (ByPass)
                return;

            ThreadConnection.Start();
        }


        public void canceler()
        {
            if (ByPass)
                return;

            ClientWeb.CancelAsync();
        }


        private void doConnection()
        {
            ClientWeb.DownloadDataAsync(new Uri(RequeteUrl));
        }


        private void telechargementTermine(object sender, DownloadDataCompletedEventArgs e)
        {
            if (ByPass)
                return;

            ValidationTerminee = true;

            if (e.Error != null)
            {
                Valide = false;
                ErreurSurvenue = true;
                Message = "Server down.\n\nTry again later.";
                return;
            }

            else if (e.Cancelled)
            {
                Valide = false;
                ErreurSurvenue = true;
                Message = "Timed out. Please retry.";
                return;
            }

            string reponse = Encoding.Default.GetString(e.Result);

            switch (reponse)
            {
                case "error no serial":
                    Valide = false;
                    Message = "Invalid product key";
                    break;
                case "error database":
                    Valide = false;
                    ErreurSurvenue = true;
                    Message = "Server down.\n\nTry again later.";
                    break;
                case "success":
                    Valide = true;
                    Message = "Thank you!";
                    break;
                case "error invalid":
                    Valide = false;
                    Message = "Invalid product key";
                    break;
                default:
                    Valide = false;
                    ErreurSurvenue = true;
                    Message = "Server down.\n\nTry again later.";
                    break;
            }
        }
    }
}
