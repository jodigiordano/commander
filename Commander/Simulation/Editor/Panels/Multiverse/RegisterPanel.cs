namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class RegisterPanel : VerticalPanel
    {
        private TextBox Username;
        private TextBox Password1;
        private TextBox Password2;
        private TextBox Email;
        private PushButton Submit;
        private Label Message;

        private TextBoxGroup TBGroup;

        private Commander.Player RegisterPlayer;


        public RegisterPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(800, 450), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Register");

            Alpha = 0;

            Username = new TextBox(new Text("username", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Password1 = new TextBox(new Text("password", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Password2 = new TextBox(new Text("password (again)", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput };
            Email = new TextBox(new Text("email", "Pixelite") { SizeX = 2 }, 300, 400) { ClickHandler = DoTextInput, CapInputToSize = false };
            Submit = new PushButton(new Text("register", "Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoSubmit };
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("username", Username);
            AddWidget("password1", Password1);
            AddWidget("password2", Password2);
            AddWidget("email", Email);
            AddWidget("submit", Submit);
            AddWidget("message", Message);

            EnableInput();

            TBGroup = new TextBoxGroup();
            TBGroup.Add(Username);
            TBGroup.Add(Password1);
            TBGroup.Add(Password2);
            TBGroup.Add(Email);
        }


        public override void Open()
        {
            base.Open();

            Username.Value = "";
            Password1.Value = "";
            Password2.Value = "";
            Email.Value = "";
            Message.Value = "";
            EnableInput();
        }


        private void DoTextInput(PanelWidget p, Commander.Player player)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, Inputs.MasterPlayer.InputType == InputType.Gamepad);
        }


        private void DoSubmit(PanelWidget p, Commander.Player player)
        {
            RegisterPlayer = player;
            Register();
        }


        private void Register()
        {
            if (!VerifyData())
                return;

            Main.MultiverseController.Register(Username.Value, Password1.Value, Email.Value, RegistrationCompleted);
            Message.Value = "Registering... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private void RegistrationCompleted(ServerProtocol protocol)
        {
            if (protocol.State == ServerProtocol.ProtocolState.EndedWithError)
            {
                Message.Value = "error: ";
                Message.Color = Colors.Panel.Error;
                EnableInput();

                switch (protocol.ErrorState)
                {
                    case ServerProtocol.ProtocolErrorState.EmailNotValid: Message.Value += "email not valid."; break;
                    case ServerProtocol.ProtocolErrorState.PasswordLength: Message.Value += "password length must be between 4 and 40."; break;
                    case ServerProtocol.ProtocolErrorState.UsernameLength: Message.Value += "username length must be between 4 and 40."; break;
                    case ServerProtocol.ProtocolErrorState.UsernameAlreadyTaken: Message.Value += "username already taken."; break;
                    case ServerProtocol.ProtocolErrorState.FileNotUploaded: Message.Value += "registered with errors."; break;
                    default: Message.Value += "something went wrong!"; break;
                }

                return;
            }

            Message.Value = "Alright! Have fun!";
            Message.Color = Colors.Panel.Ok;

            CloseButtonHandler(this, RegisterPlayer);
        }


        private bool VerifyData()
        {
            if (Username.Value.Length == 0 || Password1.Value.Length == 0 ||
                Password2.Value.Length == 0 || Email.Value.Length == 0)
            {
                Message.Value = "you must fill all the fields.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            if (Username.Value.Length < 4 || Username.Value.Length > 40)
            {
                Message.Value = "username length must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Password1.Value.Length < 4 || Password2.Value.Length < 4 ||
                Password1.Value.Length > 40 || Password2.Value.Length > 40)
            {
                Message.Value = "password length must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Password1.Value != Password2.Value)
            {
                Message.Value = "passwords mismatch.";
                Message.Color = Colors.Panel.Error;
                return false;
            }


            if (Email.Value.Length > 200)
            {
                Message.Value = "invalid email.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            try
            {
                //var email = new MailAddress(Email.Value);
            }

            catch
            {
                Message.Value = "invalid email.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            return true;
        }
    }
}
