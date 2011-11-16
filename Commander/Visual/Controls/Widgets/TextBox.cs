namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class TextBox : PanelWidget
    {
        private Text Label;
        private Image Box;
        private Image BlinkingCursor;
        private Text Data;

        public int SpaceForLabel;
        public bool CapInputToSize;

        private Particle Selection;
        private Vector3 position;

        private bool focus;
        private double FocusTimer;

        private int MaxInputSize;


        public TextBox(Text label, int spaceForLabel, int spaceForBox)
            : this(label.AbsoluteSize.Y + 10, spaceForBox)
        {
            Label = label;
            SpaceForLabel = spaceForLabel;
            Data = new Text(Label.FontName) { SizeX = Label.SizeX, Origin = Vector2.Zero, Color = Color.Black };

            MaxInputSize = GetMaxInputSize();
        }


        public TextBox(float boxHeight, int spaceForBox)
        {
            Label = new Text("Pixelite") { SizeX = 2 };
            Box = new Image("PixelBlanc") { Size = new Vector2(spaceForBox, boxHeight), Origin = Vector2.Zero };
            Data = new Text("Pixelite") { SizeX = 2, Origin = Vector2.Zero, Color = Color.Black };
            BlinkingCursor = new Image("PixelBlanc") { Size = new Vector2(10, boxHeight - 10), Origin = Vector2.Zero, Color = Color.Black };
            focus = false;
            FocusTimer = 0;
            CapInputToSize = true;

            MaxInputSize = GetMaxInputSize();
        }


        public bool Focus
        {
            get { return focus; }
            set
            {
                focus = value;
                FocusTimer = 0;
            }
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"hoverRectangle");

            var emitter = (RectEmitter) Selection.Model[0];

            emitter.Width = Box.AbsoluteSize.X + 5;
            emitter.Height = Box.AbsoluteSize.Y + 5;
            emitter.TriggerOffset = Box.AbsoluteSize / 2;
        }


        public override double VisualPriority
        {
            get
            {
                return Box.VisualPriority;
            }

            set
            {
                Data.VisualPriority = value;
                Label.VisualPriority = value;
                BlinkingCursor.VisualPriority = value;
                Box.VisualPriority = value + 0.0000001;
                Selection.VisualPriority = value + 0.0000002;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;

                Label.Position = value;
                Box.Position = Label.Data.Length == 0 ? value : Label.Position + new Vector3(SpaceForLabel, 0, 0);
                Data.Position = Box.Position + new Vector3(10, 5, 0);
                BlinkingCursor.Position = new Vector3(Data.Position.X + Data.AbsoluteSize.X + 5, Data.Position.Y, 0);

                // Center label
                Label.Position += new Vector3(0, (Box.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);
            }
        }


        public string Value
        {
            get { return Data.Data.ToLowerInvariant(); }
            set
            {
                if (CapInputToSize)
                    Data.Data = value.Substring(0, Math.Min(value.Length, MaxInputSize));
                else
                    Data.Data = value;

                BlinkingCursor.Position = new Vector3(Data.Position.X + Data.AbsoluteSize.X + 5, Data.Position.Y, 0);
            }
        }


        public void DoBackspace()
        {
            Value = Value.Substring(0, Math.Max(0, Value.Length - 1));
        }


        public override byte Alpha
        {
            get { return Box.Alpha; }
            set { Box.Alpha = Label.Alpha = Data.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3(Box.AbsoluteSize.X + (Label.Data.Length == 0 ? 0 : SpaceForLabel), Box.AbsoluteSize.Y, 0); }
            set { }
        }


        protected override bool Click(Commander.Player p)
        {
            if (Physics.CircleRectangleCollision(p.Circle, Box.GetRectangle()))
                return true;

            return false;
        }


        protected override bool Hover(Commander.Player p)
        {
            if (Physics.CircleRectangleCollision(p.Circle, Box.GetRectangle()))
            {
                Selection.Trigger(ref Box.position);

                Sticky = true;
                return true;
            }


            Sticky = false;
            return false;
        }


        public override void Draw()
        {
            Scene.Add(Label);
            Scene.Add(Box);
            Scene.Add(Data);

            if (Focus)
            {
                if (FocusTimer++ % 30 < 15)
                    Scene.Add(BlinkingCursor);
            }
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Label.Alpha = (byte) from;
            Box.Alpha = (byte) from;
            Data.Alpha = (byte) from;
            BlinkingCursor.Alpha = (byte) from;

            Scene.VisualEffects.Add(Label, effect);
            Scene.VisualEffects.Add(Box, effect);
            Scene.VisualEffects.Add(Data, effect);
            Scene.VisualEffects.Add(BlinkingCursor, effect);
        }


        private int GetMaxInputSize()
        {
            return (int) Math.Max(0, (Box.AbsoluteSize.X / new Text("A", Data.FontName) { SizeX = Data.SizeX }.AbsoluteSize.X) - 1);
        }
    }
}
