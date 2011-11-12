namespace EphemereGames.Commander
{
    using System;
    using System.Net;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LoginPanel : VerticalPanel
    {
        private WebClient Client;

        private TextBox Username;
        private TextBox Password;
        private PushButton Submit;
        private Label Message;

        private string HashedPassword;

        private TextBoxGroup TBGroup;


        public LoginPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(500, 250), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Login");

            Alpha = 0;

            Username = new TextBox(new Text("username", "Pixelite") { SizeX = 2 }, 200, 200) { ClickHandler = DoTextInput };
            Password = new TextBox(new Text("password", "Pixelite") { SizeX = 2 }, 200, 200) { ClickHandler = DoTextInput };
            Submit = new PushButton(new Text("log in", "Pixelite") { SizeX = 2 }, 200) { ClickHandler = DoSubmit };
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("username", Username);
            AddWidget("password", Password);
            AddWidget("submit", Submit);
            AddWidget("message", Message);

            Client = new WebClient();
            Client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoginCompleted);

            EnableInput();

            TBGroup = new TextBoxGroup();
            TBGroup.Add(Username);
            TBGroup.Add(Password);
        }


        public override void Open()
        {
            base.Open();

            Username.Value = "";
            Password.Value = "";
            Message.Value = "";
            EnableInput();
        }


        private void DoTextInput(PanelWidget p)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, Inputs.MasterPlayer.InputType == InputType.Gamepad);
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

            var answer = Main.MultiverseController.GetServerAnswer(e.Result);

            switch (answer.Type)
            {
                case MultiverseMessageType.Error:
                    Message.Value = "Error: " + answer.Message;
                    Message.Color = Colors.Panel.Error;
                    EnableInput();
                    break;

                case MultiverseMessageType.Login:
                    Message.Value = "Alright! Have fun!";
                    Message.Color = Colors.Panel.Ok;
                    Main.MultiverseController.LogIn(Username.Value, HashedPassword, answer.Message);
                    CloseButtonHandler(this);
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
    }
}
