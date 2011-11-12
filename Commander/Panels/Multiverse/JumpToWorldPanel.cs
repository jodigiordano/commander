namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class JumpToWorldPanel : VerticalPanel
    {
        private TextBox ById;
        private TextBox ByUsername;
        private PushButton Submit;
        private Label Message;

        private TextBoxGroup TBGroup;


        public JumpToWorldPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(500, 250), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Jump to world");

            Alpha = 0;

            ById = new TextBox(new Text("by id", "Pixelite") { SizeX = 2 }, 250, 200) { ClickHandler = DoTextInput };
            ByUsername = new TextBox(new Text("by username", "Pixelite") { SizeX = 2 }, 250, 200) { ClickHandler = DoTextInput };
            Submit = new PushButton(new Text("jump!", "Pixelite") { SizeX = 2 }, 250) { ClickHandler = DoSubmit };
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("byId", ById);
            AddWidget("byUsername", ByUsername);
            AddWidget("submit", Submit);
            AddWidget("message", Message);

            EnableInput();

            TBGroup = new TextBoxGroup();
            TBGroup.Add(ById);
            TBGroup.Add(ByUsername);
        }


        public override void Open()
        {
            base.Open();

            ByUsername.Value = "";
            ById.Value = "";
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
            Jump();
        }


        private void Jump()
        {
            if (!VerifyData())
                return;

            if (ById.Value.Length != 0)
                Main.MultiverseController.JumpToWorld(int.Parse(ById.Value), "Multiverse");
            else
                Main.MultiverseController.JumpToWorld(ByUsername.Value, "Multiverse");
            
            Message.Value = "Jumping... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
        }


        private bool VerifyData()
        {
            int byIdValue = 0;

            if (ById.Value.Length == 0 && ByUsername.Value.Length == 0)
            {
                Message.Value = "you must fill at least one field.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            if (ById.Value.Length == 0 && (ByUsername.Value.Length < 4 || ByUsername.Value.Length > 40))
            {
                Message.Value = "username must be between 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            else if (ByUsername.Value.Length == 0)
            {
                bool numeric = int.TryParse(ById.Value, out byIdValue);

                if (!numeric)
                {
                    Message.Value = "id must be a numeric value greater than 0.";
                    Message.Color = Colors.Panel.Error;
                    return false;
                }

                else if (byIdValue <= 0)
                {
                    Message.Value = "id must be greater than 0.";
                    Message.Color = Colors.Panel.Error;
                    return false;
                }
            }

            return true;
        }
    }
}
