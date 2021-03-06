﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Label : PanelWidget
    {
        private Text Text;


        public Label(Text text)
        {
            Text = text;
        }


        public void SetData(string data)
        {
            Text.Data = data;
        }


        public override double VisualPriority
        {
            get
            {
                return Text.VisualPriority;
            }

            set
            {
                Text.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Text.Position;
            }

            set
            {
                Text.Position = value;
            }
        }


        public override byte Alpha
        {
            get { return Text.Alpha; }
            set { Text.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get
            {
                return new Vector3(Text.AbsoluteSize, 0);
            }

            set { }
        }


        protected override bool Click(Circle circle)
        {
            return false;
        }


        protected override bool Hover(Circle circle)
        {
            return false;
        }


        public override void Draw()
        {
            Scene.Add(Text);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Text.Alpha = (byte) from;

            Scene.VisualEffects.Add(Text, effect);
        }
    }
}
