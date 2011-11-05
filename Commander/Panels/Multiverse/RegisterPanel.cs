namespace EphemereGames.Commander
{
    using System;
    using System.Net;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class RegisterPanel : VerticalPanel
    {
        private WebClient Client;

        private TextBox Username;
        private TextBox Password1;
        private TextBox Password2;
        private TextBox Email;
        private PushButton Submit;
        private Label Message;

        private string HashedPassword;
        private TextBoxGroup TBGroup;


        public RegisterPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(800, 400), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Register");
            Type = PanelType.Register;

            Alpha = 0;

            Username = new TextBox(new Text("username", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Password1 = new TextBox(new Text("password", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Password2 = new TextBox(new Text("password (again)", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Email = new TextBox(new Text("email", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Submit = new PushButton(new Text("register", "Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoSubmit };
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("username", Username);
            AddWidget("password1", Password1);
            AddWidget("password2", Password2);
            AddWidget("email", Email);
            AddWidget("submit", Submit);
            AddWidget("message", Message);

            Client = new WebClient();
            Client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RegistrationCompleted);

            EnableInput();

            TBGroup = new TextBoxGroup();
            TBGroup.Add(Username);
            TBGroup.Add(Password1);
            TBGroup.Add(Password2);
            TBGroup.Add(Email);
        }


        private void DoTextInput(PanelWidget p)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, Inputs.MasterPlayer.InputType == InputType.Gamepad);
        }


        private void DoSubmit(PanelWidget p)
        {
            Register();
        }


        private void Register()
        {
            var scriptUrl = Preferences.WebsiteURL + Preferences.MultiverseScriptsURL + Preferences.NewUserScript;

            if (!VerifyData())
                return;

            HashedPassword = PlayersController.GetSHA256Hash(Password1.Value + Preferences.Salt);

            var data =
                "?username=" + Username.Value +
                "&password=" + HashedPassword +
                "&email=" + Email.Value;
            
            Client.DownloadStringAsync(new Uri(scriptUrl + data));
            Message.Value = "Registering... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private void RegistrationCompleted(object sender, DownloadStringCompletedEventArgs e)
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

                    var id = Main.PlayersController.MultiverseData.WorldId;

                    Main.LevelsFactory.AddMultiverseWorld(Main.LevelsFactory.GetEmptyWorld(id));
                    break;
            }
        }


        private bool VerifyData()
        {
            if (Username.Value.Length == 0 || Password1.Value.Length == 0 ||
                Password2.Value.Length == 0 || Email.Value.Length == 0)
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


            if (Password1.Value.Length < 4 || Password2.Value.Length < 4 ||
                Password1.Value.Length > 40 || Password2.Value.Length > 40)
            {
                Message.Value = "password must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Password1.Value != Password2.Value)
            {
                Message.Value = "password mismatch.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Email.Value.Length > 200)
            {
                Message.Value = "email invalid.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            try
            {
                //var email = new MailAddress(Email.Value);
            }

            catch
            {
                Message.Value = "email invalid.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            return true;
        }
    }
}
