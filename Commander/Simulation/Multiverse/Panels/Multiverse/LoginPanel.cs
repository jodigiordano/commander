namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LoginPanel : VerticalPanel
    {
        private TextBox Username;
        private TextBox Password;
        private PushButton Submit;
        private Label Message;

        private string HashedPassword;

        private TextBoxGroup TBGroup;
        private Commander.Player LoginPlayer;


        public LoginPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(500, 300), VisualPriorities.Default.Panel, Color.White)
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


        private void DoTextInput(PanelWidget p, Commander.Player player)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, false);
        }


        private void DoSubmit(PanelWidget p, Commander.Player player)
        {
            LoginPlayer = player;
            Login();
        }


        private void Login()
        {
            if (!VerifyData())
                return;

            Main.MultiverseController.Login(Username.Value, Password.Value, LoginCompleted);
            Message.Value = "Login... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private void LoginCompleted(ServerProtocol protocol)
        {
            if (protocol.State == ServerProtocol.ProtocolState.EndedWithError)
            {
                Message.Value = "error: ";
                Message.Color = Colors.Panel.Error;
                EnableInput();

                switch (protocol.ErrorState)
                {
                    case ServerProtocol.ProtocolErrorState.PasswordLength: Message.Value += "password length must be\n\nbetween 4 and 40."; break;
                    case ServerProtocol.ProtocolErrorState.UsernameLength: Message.Value += "username length must be\n\nbetween 4 and 40."; break;
                    case ServerProtocol.ProtocolErrorState.IncorrectCredentials: Message.Value += "wrong username or password."; break;
                    default: Message.Value += "something went wrong!"; break;
                }

                return;
            }

            Message.Value = "Alright! Have fun!";
            Message.Color = Colors.Panel.Ok;

            CloseButtonHandler(this, LoginPlayer);
        }


        private bool VerifyData()
        {
            if (Username.Value.Length == 0 || Password.Value.Length == 0)
            {
                Message.Value = "you must fill all the fields.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            if (Username.Value.Length < 4 || Username.Value.Length > 40)
            {
                Message.Value = "username length must be\n\nbetween 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Password.Value.Length < 4 || Password.Value.Length > 40)
            {
                Message.Value = "password length must be\n\nbetween 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            return true;
        }
    }
}
