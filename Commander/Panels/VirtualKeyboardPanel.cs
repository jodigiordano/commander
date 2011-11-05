namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VirtualKeyboardPanel : VerticalPanel
    {
        public PanelType PanelToReopenOnClose;

        public bool SaveInput { get; private set; }

        private TextBox textBox;
        private Label Input;


        public VirtualKeyboardPanel(Scene scene, Vector3 position)
            : base(scene, position, new Vector2(1000, 700), VisualPriorities.Default.Panel, Color.White)
        {
            SetTitle("Input");

            Alpha = 0;
            //CenterWidgets = true;

            Input = new Label(new Text("Pixelite") { SizeX = 2 });

            AddWidget("input", Input);
            AddWidget("separator", new VerticalSeparatorWidget() { MaxAlpha = 0 });
            AddWidget("keyboard", GetKeyboard());
            AddWidget("keyboard_others", GetOtherChars());
            AddWidget("submit", GetSubmit());
        }


        public override void  Initialize()
        {
 	        base.Initialize();

            SaveInput = false;
        }


        public override void SetTitle(string title)
        {
            base.SetTitle("Input: " + title);
        }


        public TextBox TextBox
        {
            get { return textBox; }
            set
            {
                textBox = value;
                Input.Value = textBox.Value;
            }
        }


        public string Value
        {
            get { return Input.Value; }
        }


        private void DoEnterText(PanelWidget widget)
        {
            Input.Value += widget.Name;
        }


        private void DoArobas(PanelWidget widget)
        {
            Input.Value += "@";
        }


        private void DoDotCom(PanelWidget widget)
        {
            Input.Value += ".com";
        }


        private void DoBackSpace(PanelWidget widget)
        {
            Input.Value = Input.Value.Substring(0, Math.Max(0, Input.Value.Length - 1));
        }


        private void DoClear(PanelWidget widget)
        {
            Input.Value = "";
        }


        private void DoCancel(PanelWidget widget)
        {
            SaveInput = false;
            
            if (CloseButtonHandler != null)
                CloseButtonHandler(widget);
        }


        private void DoSubmit(PanelWidget widget)
        {
            SaveInput = true;

            if (CloseButtonHandler != null)
                CloseButtonHandler(widget);
        }


        private Panel GetKeyboard()
        {
            var p = new GridPanel(Scene, Position, new Vector2(Size.X, 500), VisualPriority, Color)
            {
                OnlyShowWidgets = true,
                NbColumns = 10
            };

            char[] chars = "qwertyuiopasdfghjkl;zxcvbnm,./1234567890~!@#$%^&*()_+-=".ToCharArray();

            foreach (var c in chars)
                p.AddWidget(c.ToString(), new PushButton(new Text(c.ToString(), "Pixelite") { SizeX = 2 }, 25)
                {
                    ClickHandler = DoEnterText
                });

            return p;
        }


        private Panel GetOtherChars()
        {
            var p = new HorizontalPanel(Scene, Position, new Vector2(Size.X, 100), VisualPriority, Color)
            {
                OnlyShowWidgets = true,
                DistanceBetweenTwoChoices = 50
            };

            p.AddWidget("backspace", new PushButton(new Text("backspace", "Pixelite") { SizeX = 2 }, 150)
            {
                ClickHandler = DoBackSpace
            });

            p.AddWidget("arobas", new PushButton(new Text("@ (at sign)", "Pixelite") { SizeX = 2 }, 175)
            {
                ClickHandler = DoArobas
            });

            p.AddWidget("dotcom", new PushButton(new Text(".com", "Pixelite") { SizeX = 2 }, 100)
            {
                ClickHandler = DoDotCom
            });

            return p;
        }


        private Panel GetSubmit()
        {
            var p = new HorizontalPanel(Scene, Position, new Vector2(Size.X, 100), VisualPriority, Color)
            {
                OnlyShowWidgets = true,
                DistanceBetweenTwoChoices = 50
            };

            p.AddWidget("clear", new PushButton(new Text("clear", "Pixelite") { SizeX = 2 }, 100)
            {
                ClickHandler = DoClear
            });

            p.AddWidget("cancel", new PushButton(new Text("cancel", "Pixelite") { Color = Colors.Panel.Cancel, SizeX = 2 }, 100)
            {
                ClickHandler = DoCancel
            });

            p.AddWidget("ok", new PushButton(new Text("ok", "Pixelite") { Color = Colors.Panel.Submit, SizeX = 2 }, 100)
            {
                ClickHandler = DoSubmit
            });

            return p;
        }
    }
}
