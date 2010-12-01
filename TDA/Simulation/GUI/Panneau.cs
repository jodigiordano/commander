namespace TDA
{
    using System;
    using System.Collections.Generic;
    using RainingSundays.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Utilities;

    class Panneau : DrawableGameComponent, IRepresentable
    {
        public IVisible representation              { get; set; }
        public Vector3 Position                     { get; set; }
        public Vector2 Taille                       { get; set; }


        public int Pointage                             { get; set; }
        public int ReserveUnites                        { get; set; }
        public int VaguesRestantes                      { get; set; }
        public double TempsProchaineVague               { get; set; }

        private Partie Partie;
        private IVisible WidgetPointage;
        private IVisible WidgetReserveUnites;
        private IVisible WidgetVaguesRestantes;
        private IVisible WidgetProchaineVagueDans;
        private IVisible WidgetOmbre;

        public Panneau(Partie partie, Vector3 position, Vector2 taille)
            : base(partie.Main)
        {
            this.Partie = partie;
            Position = position;
            Taille = taille;
            Pointage = 0;
            ReserveUnites = 0;

            WidgetPointage = new IVisible
            (
                "pointage: " + Pointage,
                Partie.Main.Content.Load<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 10, 0),
                partie
            );
            WidgetPointage.PrioriteAffichage = 0;

            WidgetReserveUnites = new IVisible
            (
                "ressources: " + Pointage,
                Partie.Main.Content.Load<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 30, 0),
                partie
            );
            WidgetReserveUnites.PrioriteAffichage = 0;

            WidgetVaguesRestantes = new IVisible
            (
                "vagues restantes: " + VaguesRestantes,
                Partie.Main.Content.Load<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 50, 0),
                partie
            );
            WidgetVaguesRestantes.PrioriteAffichage = 0;

            WidgetProchaineVagueDans = new IVisible
            (
                "prochaine vague: " + TempsProchaineVague,
                Partie.Main.Content.Load<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 70, 0),
                partie
            );
            WidgetProchaineVagueDans.PrioriteAffichage = 0;


            WidgetOmbre = new IVisible
            (
                Partie.Main.Content.Load<Texture2D>("pixelBlanc"),
                Position,
                partie
            );
            WidgetOmbre.TailleVecteur = Taille;
            WidgetOmbre.Couleur = new Color(Color.Black, 230);
            WidgetOmbre.PrioriteAffichage = 0.01f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            WidgetPointage.Texte = "pointage: " + Pointage;
            WidgetReserveUnites.Texte = "ressources: " + ReserveUnites;
            WidgetVaguesRestantes.Texte = "vagues restantes: " + VaguesRestantes;
            WidgetProchaineVagueDans.Texte = "prochaine vague: " + (TempsProchaineVague / 1000.0).ToString("F2") + " sec.";

            Partie.ajouterScenable(WidgetPointage);
            Partie.ajouterScenable(WidgetReserveUnites);
            Partie.ajouterScenable(WidgetOmbre);
            Partie.ajouterScenable(WidgetVaguesRestantes);
            Partie.ajouterScenable(WidgetProchaineVagueDans);
        }
    }
}
