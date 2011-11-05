namespace EphemereGames.Commander
{
    using System;
    using System.Net;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class RegisterPanel : VerticalPanel
    {
        public event PanelTextBoxStringHandler VirtualKeyboardAsked;

        private WebClient Client;

        private TextBox Username;
        private TextBox Password1;
        private TextBox Password2;
        private TextBox Email;
        private PushButton Submit;
        private Label Message;

        private string HashedPassword;
        private KeyboardInput KeyboardInput;


        public RegisterPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(800, 400), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Register");
            Type = PanelType.Register;

            Alpha = 0;

            Username = new TextBox(new Text("username", "Pixelite") { SizeX = 2 }, 300, 400);
            Password1 = new TextBox(new Text("password", "Pixelite") { SizeX = 2 }, 300, 400);
            Password2 = new TextBox(new Text("password (again)", "Pixelite") { SizeX = 2 }, 300, 400);
            Email = new TextBox(new Text("email", "Pixelite") { SizeX = 2 }, 300, 400);
            Submit = new PushButton(new Text("register", "Pixelite") { SizeX = 2 }, 300);
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

            KeyboardInput = new KeyboardInput()
            {
                DesactivatedCallback = KeyboardInputDesactivatedCallback
            };
        }


        private void DoTextInput(PanelWidget p)
        {
            //if (VirtualKeyboardAsked != null)
            //    VirtualKeyboardAsked(this, (TextBox) p, p.Name);

            var textbox = (TextBox) p;

            ToggleTextBoxFocus(textbox);

            KeyboardInput.TextBoxFocus = textbox;
            KeyboardInput.Active = true;
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


        private void DisableInput()
        {
            Username.ClickHandler = null;
            Password1.ClickHandler = null;
            Password2.ClickHandler = null;
            Email.ClickHandler = null;
            Submit.ClickHandler = null;
        }


        public void EnableInput()
        {
            Username.ClickHandler = DoTextInput;
            Password1.ClickHandler = DoTextInput;
            Password2.ClickHandler = DoTextInput;
            Email.ClickHandler = DoTextInput;
            Submit.ClickHandler = DoSubmit;
        }


        private void ToggleTextBoxFocus(TextBox box)
        {
            Username.Focus = false;
            Password1.Focus = false;
            Password2.Focus = false;
            Email.Focus = false;

            if (box != null)
                box.Focus = true;
        }


        private void KeyboardInputDesactivatedCallback()
        {
            ToggleTextBoxFocus(null);
        }


        public override void Draw()
        {
            base.Draw();

            KeyboardInput.Update();
        }
    }
}
