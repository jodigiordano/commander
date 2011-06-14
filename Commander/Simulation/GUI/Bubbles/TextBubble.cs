namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextBubble : Bubble
    {
        public Text Texte;
        public double TempsAffichage;
        public double TempsFadeOut;
        public bool Visible;

        public Vector3 Position
        {
            set
            {
                Dimension.X = (int) value.X;
                Dimension.Y = (int) value.Y;
            }
        }

        public TextBubble(Simulation simulation, Text texte, Vector3 position, double tempsAffichage, double prioriteAffichage)
            : base(simulation, new Rectangle(), prioriteAffichage)
        {
            this.Texte = texte;
            this.Texte.VisualPriority = prioriteAffichage - 0.01f;
            this.Position = position;
            this.TempsAffichage = tempsAffichage;
            this.TempsFadeOut = double.MaxValue;

            ComputeSize();
            ComputePosition();

            Visible = false;
        }

        public bool Termine
        {
            get { return TempsAffichage <= 0; }
        }

        public void Update(GameTime gameTime)
        {
            TempsAffichage -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (TempsAffichage <= TempsFadeOut)
            {
                TempsFadeOut = double.NaN;
                FadeOut(TempsAffichage);
            }

            Visible = Texte.Color.A != 0; //TempsAffichage > 0;

            ComputeSize();
            ComputePosition();
        }


        //public override void Show()
        //{
        //    base.Show();

        //    Simulation.Scene.Add(Texte);
        //}


        //public override void Hide()
        //{
        //    base.Hide();

        //    Simulation.Scene.Remove(Texte);
        //}


        public override void Draw()
        {
            base.Draw();

            Texte.Position = new Vector3(Dimension.X, Dimension.Y, 0);
            Simulation.Scene.Add(Texte);
        }


        public override void FadeIn(double temps)
        {
            base.FadeIn(temps);

            Texte.Color.A = 0;
            Simulation.Scene.VisualEffects.Add(Texte, VisualEffects.FadeInFrom0(255, 0, temps));
        }


        public override void FadeOut(double temps)
        {
            base.FadeOut(temps);

            Simulation.Scene.VisualEffects.Add(Texte, VisualEffects.FadeOutTo0(255, 0, temps));
        }


        private void ComputeSize()
        {
            Vector2 size = Texte.TextSize;

            this.Dimension.Width = (int) size.X + 4;
            this.Dimension.Height = (int) size.Y + 4;
        }


        private void ComputePosition()
        {
            bool tropADroite = Dimension.X + Dimension.Width + 50 > 640 - Preferences.Xbox360DeadZoneV2.X;
            bool tropBas = Dimension.Y + Dimension.Height > 370 - Preferences.Xbox360DeadZoneV2.Y;

            if (tropADroite && tropBas)
            {
                Dimension.X += -Dimension.Width - 50;
                Dimension.Y += -Dimension.Height - 10;
                BlaPosition = 2;
            }

            else if (tropADroite)
            {
                Dimension.X += -Dimension.Width - 50;
                BlaPosition = 1;
            }

            else if (tropBas)
            {
                Dimension.Y += -Dimension.Height - 50;
                BlaPosition = 3;
            }

            else
            {
                Dimension.X += 50;
                Dimension.Y += -10;
                BlaPosition = 0;
            }

            Dimension.X = (int) MathHelper.Clamp(Dimension.X, -640 + Preferences.Xbox360DeadZoneV2.X, 640 - Preferences.Xbox360DeadZoneV2.X - Dimension.Width / 2);
            Dimension.Y = (int) MathHelper.Clamp(Dimension.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Dimension.Height / 2, 370 - Preferences.Xbox360DeadZoneV2.Y - Dimension.Height / 2);
        }
    }
}
