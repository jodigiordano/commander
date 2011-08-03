namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Logo : IVisual
    {
        private Scene Scene;
        private Image EphemereBack;
        private Image EphemereFront;
        private Image Games;

        private byte MaxBackAlpha;
        private byte MaxFrontAlpha;

        private double TimeBeforeFadeColour;
        private bool FadeColourDone;

        public Logo(Scene scene, Vector3 position, double visualPriority)
        {
            Scene = scene;

            MaxBackAlpha = 210;
            MaxFrontAlpha = 190;

            EphemereBack = new Image("LogoBack", position);
            EphemereFront = new Image("LogoFront", position);
            Games = new Image("LogoGames", position + new Vector3(0, 75, 0));

            TimeBeforeFadeColour = 1000;
            FadeColourDone = false;
        }


        public byte Alpha
        {
            get
            {
                return EphemereBack.Alpha;
            }
            set
            {
                if (!FadeColourDone)
                    EphemereBack.Alpha = Math.Min(value, MaxBackAlpha);
                
                EphemereFront.Alpha = Math.Min(value, MaxFrontAlpha);
                Games.Alpha = value;
            }
        }


        public void Update()
        {
            TimeBeforeFadeColour -= Preferences.TargetElapsedTimeMs;

            if (!FadeColourDone && TimeBeforeFadeColour < 0)
            {
                FadeColour();
                FadeColourDone = true;
            }
        }


        public void Draw()
        {
            Scene.Add(EphemereBack);
            Scene.Add(EphemereFront);
            Scene.Add(Games);
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        private void FadeColour()
        {
            Scene.VisualEffects.Add(EphemereBack, Core.Visual.VisualEffects.FadeOutTo0(MaxBackAlpha, 0, 1000));
        }
    }
}
