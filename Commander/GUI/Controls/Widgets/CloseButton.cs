﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CloseButton
    {
        public NoneHandler Handler;
        public Scene Scene; 
        
        private Image Button;
        private Text ButtonX;
        private Circle ButtonCircle;


        public CloseButton(Vector3 position, double visualPriority)
        {
            Button = new Image("checkbox", position)
            {
                VisualPriority = visualPriority + 0.0000001,
                SizeX = 3
            };

            ButtonX = new Text("X", "Pixelite", position)
            {
                VisualPriority = visualPriority,
                SizeX = 2
            }.CenterIt();

            ButtonCircle = new Circle(position, 8);
        }


        public bool DoClick(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, ButtonCircle))
            {
                if (Handler != null)
                    Handler();

                return true;
            }

            return false;
        }


        public void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Button.Alpha = (byte) from;
            ButtonX.Alpha = (byte) from;

            Scene.VisualEffects.Add(Button, effect);
            Scene.VisualEffects.Add(ButtonX, effect);
        }


        public void Draw()
        {
            Scene.Add(Button);
            Scene.Add(ButtonX);
        }
    }
}