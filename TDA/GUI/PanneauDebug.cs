namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class PanneauDebug : DrawableGameComponent, IRepresentable
    {
        public IVisible representation { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Taille { get; set; }

        public bool Visible = false;
        public int NbEnnemis;
        public int NbProjectiles;
        public int NbDetectionsCollisionsCeTick;

        private Partie Partie;
        private IVisible WidgetFPS;
        private IVisible WidgetRunningSlowly;
        private IVisible WidgetNbEnnemis;
        private IVisible WidgetNbProjectiles;
        private IVisible WidgetNbDetectionsCollisionsCeTick;
        private IVisible WidgetOmbre;

        public PanneauDebug(Partie partie, Vector3 position, Vector2 taille)
            : base(partie.Main)
        {
            this.Partie = partie;
            Position = position;
            Taille = taille;

            WidgetFPS = new IVisible
            (
                "FPS: ",
                Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 10, 0),
                partie
            );
            WidgetFPS.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug;

            WidgetRunningSlowly = new IVisible
            (
                "Running Slowly ?: ",
                Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 30, 0),
                partie
            );
            WidgetRunningSlowly.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug;

            WidgetNbEnnemis = new IVisible
            (
                "# Ennemis: ",
                Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 50, 0),
                partie
            );
            WidgetNbEnnemis.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug;

            WidgetNbProjectiles = new IVisible
            (
                "# Projectiles: ",
                Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 70, 0),
                partie
            );
            WidgetNbProjectiles.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug;

            WidgetNbDetectionsCollisionsCeTick = new IVisible
            (
                "# DC ce tick : ",
                Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini"),
                Color.White,
                new Vector3(Position.X + 10, Position.Y + 90, 0),
                partie
            );
            WidgetNbDetectionsCollisionsCeTick.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug;

            WidgetOmbre = new IVisible
            (
                Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"),
                Position,
                partie
            );
            WidgetOmbre.TailleVecteur = Taille;
            WidgetOmbre.Couleur = new Color(Color.Black, 230);
            WidgetOmbre.PrioriteAffichage = Preferences.PrioriteGUIPanneauDebug + 0.01f;
        }


        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            WidgetFPS.Texte = "FPS: " + (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString("F2");
            WidgetRunningSlowly.Texte = "Running Slowly?: " + gameTime.IsRunningSlowly;
            WidgetNbEnnemis.Texte = "# Ennemis: " + NbEnnemis;
            WidgetNbProjectiles.Texte = "# Projectiles: " + NbProjectiles;
            WidgetNbDetectionsCollisionsCeTick.Texte = "# DC ce tick: " + NbDetectionsCollisionsCeTick;

            Partie.ajouterScenable(WidgetFPS);
            Partie.ajouterScenable(WidgetNbEnnemis);
            Partie.ajouterScenable(WidgetNbProjectiles);
            Partie.ajouterScenable(WidgetRunningSlowly);
            Partie.ajouterScenable(WidgetNbDetectionsCollisionsCeTick);
            Partie.ajouterScenable(WidgetOmbre);
        }
    }
}
