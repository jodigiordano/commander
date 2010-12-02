namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class BulleTexte : Bulle
    {
        public IVisible Texte;
        public double TempsAffichage;
        public double TempsFadeOut;

        public Vector3 Position
        {
            set
            {
                Dimension.X = (int) value.X;
                Dimension.Y = (int) value.Y;
            }
        }

        public BulleTexte(Simulation simulation, IVisible texte, Vector3 position, double tempsAffichage, float prioriteAffichage)
            : base(simulation, new Rectangle(), prioriteAffichage)
        {
            this.Texte = texte;
            this.Texte.PrioriteAffichage = prioriteAffichage - 0.01f;
            this.Position = position;
            this.TempsAffichage = tempsAffichage;
            this.TempsFadeOut = double.MaxValue;

            determinerTaille();
            determinerPosition();

            this.Visible = false;
        }

        public bool Termine
        {
            get { return TempsAffichage <= 0; }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TempsAffichage -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (TempsAffichage <= TempsFadeOut)
            {
                TempsFadeOut = double.NaN;
                doHide(TempsAffichage);
            }

            Visible = Texte.Couleur.A != 0; //TempsAffichage > 0;

            determinerTaille();
            determinerPosition();
        }


        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
               return;

            base.Draw(gameTime);

            Texte.Position = new Vector3(Dimension.X, Dimension.Y, 0);

            Simulation.Scene.ajouterScenable(this.Texte);
        }

        private void determinerTaille()
        {
            this.Dimension.Width = this.Texte.Rectangle.Width + 4;
            this.Dimension.Height = this.Texte.Rectangle.Height + 4;
        }

        private void determinerPosition()
        {
            bool tropADroite = Dimension.X + Dimension.Width + 50 > 640 - Preferences.DeadZoneXbox.X;
            bool tropBas = Dimension.Y + Dimension.Height > 370 - Preferences.DeadZoneXbox.Y;

            if (tropADroite && tropBas)
            {
                Dimension.X += -Dimension.Width - 50;
                Dimension.Y += -Dimension.Height - 10;
                PositionBla = 2;
            }

            else if (tropADroite)
            {
                Dimension.X += -Dimension.Width - 50;
                PositionBla = 1;
            }

            else if (tropBas)
            {
                Dimension.Y += -Dimension.Height - 50;
                PositionBla = 3;
            }

            else
            {
                Dimension.X += 50;
                Dimension.Y += -10;
                PositionBla = 0;
            }

            Dimension.X = (int) MathHelper.Clamp(Dimension.X, -640 + Preferences.DeadZoneXbox.X, 640 - Preferences.DeadZoneXbox.X - Dimension.Width / 2);
            Dimension.Y = (int) MathHelper.Clamp(Dimension.Y, -370 + Preferences.DeadZoneXbox.Y + Dimension.Height / 2, 370 - Preferences.DeadZoneXbox.Y - Dimension.Height / 2);

        }


        public override void doShow(double temps)
        {
            base.doShow(temps);

            Texte.Couleur.A = 0;
            Simulation.Scene.Effets.ajouter(Texte, EffetsPredefinis.fadeInFrom0(255, 0, temps));
        }


        public override void doHide(double temps)
        {
            base.doHide(temps);

            Simulation.Scene.Effets.ajouter(Texte, EffetsPredefinis.fadeOutTo0(255, 0, temps));
        }
    }
}
