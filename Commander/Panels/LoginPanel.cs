namespace EphemereGames.Commander
{
    using System;
    using System.Net;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LoginPanel : VerticalPanel
    {
        public event PanelTextBoxStringHandler VirtualKeyboardAsked;

        private WebClient Client;

        private TextBox Username;
        private TextBox Password;
        private PushButton Submit;
        private Label Message;

        private string HashedPassword;


        public LoginPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(500, 250), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Login");
            Type = PanelType.Login;

            Alpha = 0;

            Username = new TextBox(new Text("username", "Pixelite") { SizeX = 2 }, 200, 200);
            Password = new TextBox(new Text("password", "Pixelite") { SizeX = 2 }, 200, 200);
            Submit = new PushButton(new Text("log in", "Pixelite") { SizeX = 2 }, 200);
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("username", Username);
            AddWidget("password", Password);
            AddWidget("submit", Submit);
            AddWidget("message", Message);

            Client = new WebClient();
            Client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoginCompleted);

            EnableInput();
        }


        private void DoTextInput(PanelWidget p)
        {
            if (VirtualKeyboardAsked != null)
                VirtualKeyboardAsked(this, (TextBox) p, p.Name);
        }


        private void DoSubmit(PanelWidget p)
        {
            Login();
        }


        private void Login()
        {
            var scriptUrl = Preferences.WebsiteURL + Preferences.MultiverseScriptsURL + Preferences.LoginScript;

            if (!VerifyData())
                return;

            HashedPassword = PlayersController.GetSHA256Hash(Password.Value + Preferences.Salt);

            var data =
                "?username=" + Username.Value +
                "&password=" + HashedPassword;

            Client.DownloadStringAsync(new Uri(scriptUrl + data));
            Message.Value = "Login... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private void LoginCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == "")
            {
                if (e.Error != null)
                    Message.Value = "Error: " + e.Error.Message;
                else
                    Message.Value = "Error: something went wrong!";

                Message.Color = Colors.Panel.Error;
                EnableInput();

                return;
            }

            var answer = Main.Multiverse.GetServerAnswer(e.Result);

            switch (answer.Type)
            {
                case MultiverseMessageType.Error:
                    Message.Value = "Error: " + answer.Message;
                    Message.Color = Colors.Panel.Error;
                    EnableInput();
                    break;

                case MultiverseMessageType.NewPlayer:
                    Message.Value = "Alright! Have fun!";
                    Message.Color = Colors.Panel.Ok;
                    Main.PlayersController.UpdateMultiverse(Username.Value, HashedPassword, answer.Message);
                    break;
            }
        }


        private bool VerifyData()
        {
            if (Username.Value.Length == 0 || Password.Value.Length == 0)
            {
                Message.Value = "you must fill all fields.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            if (Username.Value.Length < 4 || Username.Value.Length > 40)
            {
                Message.Value = "username must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Password.Value.Length < 4 || Password.Value.Length > 40)
            {
                Message.Value = "password must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            return true;
        }


        private void DisableInput()
        {
            Username.ClickHandler = null;
            Password.ClickHandler = null;
            Submit.ClickHandler = null;
        }


        public void EnableInput()
        {
            Username.ClickHandler = DoTextInput;
            Password.ClickHandler = DoTextInput;
            Submit.ClickHandler = DoSubmit;
        }
    }
}
