﻿namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class JumpToWorldPanel : VerticalPanel
    {
        public string From;

        private TextBox ById;
        private TextBox ByUsername;
        private PushButton Submit;
        private Label Message;

        private TextBoxGroup TBGroup;
        private Commander.Player Player;


        public JumpToWorldPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(400, 250), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Jump to world");

            Alpha = 0;

            ById = new TextBox(new Text("id", "Pixelite") { SizeX = 2 }, 150, 200) { ClickHandler = DoTextInput };
            ByUsername = new TextBox(new Text("by username", "Pixelite") { SizeX = 2 }, 250, 200) { ClickHandler = DoTextInput };
            Submit = new PushButton(new Text("jump!", "Pixelite") { SizeX = 2 }, 150) { ClickHandler = DoSubmit };
            Message = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("byId", ById);
            //AddWidget("byUsername", ByUsername);
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


        private void DoTextInput(PanelWidget p, Commander.Player player)
        {
            var tb = (TextBox) p;

            TBGroup.SwitchTo(tb);
            ActivateKeyboardInput(TBGroup, false);
        }


        private void DoSubmit(PanelWidget p, Commander.Player player)
        {
            Player = player;
            Jump();
        }


        private void Jump()
        {
            if (!VerifyData())
                return;

            if (ById.Value.Length != 0)
                Main.MultiverseController.JumpToWorld(int.Parse(ById.Value), From);
            //else
            //    Main.MultiverseController.JumpToWorld(ByUsername.Value, From);
            
            Message.Value = "Jumping... please wait.";
            Message.Color = Colors.Panel.Waiting;
            DisableInput();
            CloseButtonHandler(this, Player);
        }


        private bool VerifyData()
        {
            int byIdValue = 0;

            if (ById.Value.Length == 0 && ByUsername.Value.Length == 0)
            {
                Message.Value = "you must fill the field.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            if (ById.Value.Length == 0 && (ByUsername.Value.Length < 4 || ByUsername.Value.Length > 40))
            {
                Message.Value = "username length must be\n\nbetween 4 and 40.";
                Message.Color = Colors.Panel.Error;
                return false;
            }

            else if (ByUsername.Value.Length == 0)
            {
                bool numeric = int.TryParse(ById.Value, out byIdValue);

                if (!numeric)
                {
                    Message.Value = "id must be a numeric\n\nvalue greater than 0.";
                    Message.Color = Colors.Panel.Error;
                    return false;
                }

                else if (byIdValue <= 0)
                {
                    Message.Value = "id must be greater\n\nthan 0.";
                    Message.Color = Colors.Panel.Error;
                    return false;
                }
            }

            return true;
        }
    }
}