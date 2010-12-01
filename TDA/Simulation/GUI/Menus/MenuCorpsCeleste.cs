namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class PanneauCorpsCeleste : DrawableGameComponent
    {
        public Emplacement EmplacementSelectionne;
        public CorpsCeleste CorpsSelectionne;

        public Dictionary<Tourelle, bool> TourellesAchat;
        public Tourelle TourellePourAchatSelectionne;

        public Dictionary<int, bool> OptionsPourTourelleAchetee;
        public int OptionPourTourelleAcheteeSelectionne;

        public Dictionary<PowerUp, bool> OptionsPourCorpsCelesteSelectionne;
        public PowerUp OptionPourCorpsCelesteSelectionne;

        private Vector3 Position;
        private Vector2 Taille;
        private Simulation Simulation;

        private MenuEmplacement WidgetEmplacementSelectionne;
        private MenuPowerUps WidgetCorpsCelesteSelectionne;

        public Bulle Bulle;


        public PanneauCorpsCeleste(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;

            this.Taille = Vector2.Zero;

            Bulle = new Bulle(Simulation, new Rectangle(), Preferences.PrioriteGUIPanneauCorpsCeleste + 0.05f);

        }


        public override void Initialize()
        {
            WidgetEmplacementSelectionne = new MenuEmplacement(Simulation, Bulle, null, Vector3.Zero, null, null, Preferences.PrioriteGUIPanneauCorpsCeleste);
            WidgetCorpsCelesteSelectionne = new MenuPowerUps(Simulation, Bulle, null, Vector3.Zero, null, Preferences.PrioriteGUIPanneauCorpsCeleste);
        }


        public override void Update(GameTime gameTime)
        {
            if (CorpsSelectionne == null)
                return;

            this.Position = (EmplacementSelectionne == null) ? CorpsSelectionne.Position : EmplacementSelectionne.Position;

            determinerTaillePanneau();

            bool tropADroite = Position.X + this.Taille.X + 50 > 640 - Preferences.DeadZoneXbox.X;
            bool tropBas = Position.Y + this.Taille.Y > 370 - Preferences.DeadZoneXbox.Y;

            if (tropADroite && tropBas)
            {
                this.Position += new Vector3(-this.Taille.X - 50, -this.Taille.Y - 10, 0);
                Bulle.PositionBla = 2;
            }

            else if (tropADroite)
            {
                this.Position += new Vector3(-this.Taille.X - 50, 0, 0);
                Bulle.PositionBla = 1;
            }

            else if (tropBas)
            {
                this.Position += new Vector3(0, -this.Taille.Y - 50, 0);
                Bulle.PositionBla = 3;
            }

            else
            {
                this.Position += new Vector3(50, -10, 0);
                Bulle.PositionBla = 0;
            }


            Bulle.Dimension = new Rectangle((int) Position.X, (int) Position.Y, (int) Taille.X, (int) Taille.Y);

            WidgetEmplacementSelectionne.Position = this.Position;
            WidgetCorpsCelesteSelectionne.Position = this.Position;
        }


        public void Refresh()
        {
            Bulle.Visible = CorpsSelectionne != null;

            if (CorpsSelectionne == null)
                return;

            this.Position = CorpsSelectionne.Position;

            WidgetCorpsCelesteSelectionne.CorpsCeleste = CorpsSelectionne;
            WidgetCorpsCelesteSelectionne.Options = OptionsPourCorpsCelesteSelectionne;
            WidgetCorpsCelesteSelectionne.OptionSelectionee = OptionPourCorpsCelesteSelectionne;

            WidgetEmplacementSelectionne.Emplacement = EmplacementSelectionne;
            WidgetEmplacementSelectionne.TourellesAchat = TourellesAchat;
            WidgetEmplacementSelectionne.OptionsTourellesAchetees = OptionsPourTourelleAchetee;
            WidgetEmplacementSelectionne.OptionPourTourelleAcheteeSelectionne = OptionPourTourelleAcheteeSelectionne;
            WidgetEmplacementSelectionne.TourellePourAchatSelectionnee = TourellePourAchatSelectionne;
        }


        public override void Draw(GameTime gameTime)
        {
            if (CorpsSelectionne != null && EmplacementSelectionne == null)
                WidgetCorpsCelesteSelectionne.Draw(null);

            if (EmplacementSelectionne != null)
                WidgetEmplacementSelectionne.Draw(null);
        }


        private void determinerTaillePanneau()
        {
            // Emplacement selectionne et occupe
            if (EmplacementSelectionne != null && EmplacementSelectionne.EstOccupe)
            {
                int nb = OptionsPourTourelleAchetee.Count;

                if (!EmplacementSelectionne.Tourelle.PeutVendre)
                    nb--;

                if (!EmplacementSelectionne.Tourelle.PeutMettreAJour)
                    nb--;

                this.Taille = new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
            }

            // Emplacement selectionne et non-occupe
            else if (EmplacementSelectionne != null && !EmplacementSelectionne.EstOccupe)
                this.Taille = (TourellesAchat.Count == 0) ? Vector2.Zero : new Vector2(190, 10 + TourellesAchat.Count * 40);


            // Emplacement non-selectionne
            else if (EmplacementSelectionne == null && CorpsSelectionne != null)
            {
                int nb = OptionsPourCorpsCelesteSelectionne.Count;

                if (!CorpsSelectionne.PeutAvoirDoItYourself)
                    nb--;

                if (!CorpsSelectionne.PeutDetruire)
                    nb--;

                if (!CorpsSelectionne.PeutAvoirCollecteur)
                    nb--;

                if (!CorpsSelectionne.PeutAvoirTheResistance)
                    nb--;

                this.Taille = new Vector2(190, (nb == 0) ? 0 : 10 + nb * 40);
            }
        }
    }
}
