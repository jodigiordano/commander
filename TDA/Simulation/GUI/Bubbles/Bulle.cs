namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Utilities;

    class Bulle : DrawableGameComponent
    {
        protected Simulation Simulation;

        private List<IVisible> Coins;
        private List<IVisible> Contours;
        protected IVisible Filtre;
        private IVisible Bla;

        public Rectangle Dimension;
        public int PositionBla;

        public Bulle(Simulation simulation, Rectangle dimension, float prioriteAffichage)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Dimension = dimension;

            Bla = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleBlabla"), Vector3.Zero);
            Bla.VisualPriority = prioriteAffichage;

            Filtre = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
            Filtre.Couleur = new Color(0, 0, 0, 200);
            Filtre.VisualPriority = prioriteAffichage + 0.02f;


            Coins = new List<IVisible>();
            Contours = new List<IVisible>();

            for (int i = 0; i < 4; i++)
            {
                IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleCoin"), Vector3.Zero);
                iv.VisualPriority = prioriteAffichage + 0.01f;

                Coins.Add(iv);

                iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
                iv.VisualPriority = prioriteAffichage + 0.01f;

                Contours.Add(iv);

            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            Dimension.Height = Math.Max(Dimension.Height, 30);

            Filtre.TailleVecteur = new Vector2(Dimension.Width + 8, Dimension.Height + 8);
            Filtre.Position = new Vector3(Dimension.Left - 4, Dimension.Top - 4, 0);

            Coins[0].Position = new Vector3(Dimension.Left - 8, Dimension.Top - 8, 0); //Haut droite
            Coins[0].Rotation = 0;

            Coins[1].Position = new Vector3(Dimension.Left - 4, Dimension.Bottom + 4, 0); //Bas droite
            Coins[1].Origine = Coins[1].Centre;
            Coins[1].Rotation = -MathHelper.PiOver2;

            Coins[2].Position = new Vector3(Dimension.Right + 8, Dimension.Top - 8, 0); //Haut gauche
            Coins[1].Origine = Coins[1].Centre;
            Coins[2].Rotation = MathHelper.PiOver2;

            Coins[3].Position = new Vector3(Dimension.Right + 8, Dimension.Bottom + 8, 0); //Bas gauche
            Coins[1].Origine = Coins[1].Centre;
            Coins[3].Rotation = MathHelper.Pi;


            Contours[0].TailleVecteur = new Vector2(Dimension.Width, 4); //Haut
            Contours[0].Position = new Vector3(Dimension.Left, Dimension.Top - 8, 0);

            Contours[1].TailleVecteur = new Vector2(Dimension.Width, 4); //Bas
            Contours[1].Position = new Vector3(Dimension.Left, Dimension.Bottom + 4, 0);

            Contours[2].TailleVecteur = new Vector2(4, Dimension.Height); //Gauche
            Contours[2].Position = new Vector3(Dimension.Left - 8, Dimension.Top, 0);

            Contours[3].TailleVecteur = new Vector2(4, Dimension.Height); //Droite
            Contours[3].Position = new Vector3(Dimension.Right + 4, Dimension.Top, 0);

            switch (PositionBla)
            {
                case 0: // haut gauche
                    Bla.Position = new Vector3(Dimension.Left - 4, Dimension.Top + 12, 0);
                    Bla.Origine = new Vector2(Bla.Texture.Width, 0);
                    Bla.Rotation = 0;
                    Bla.Effets = SpriteEffects.None;
                    break;

                case 1: // haut droite
                    Bla.Position = new Vector3(Dimension.Right + 4, Dimension.Top + 12, 0);
                    Bla.Origine = Vector2.Zero;
                    Bla.Rotation = 0;
                    Bla.Effets = SpriteEffects.FlipHorizontally;
                    break;
                case 2: // bas droite
                    Bla.Position = new Vector3(Dimension.Right + 16, Dimension.Bottom - 32, 0);
                    Bla.Origine = Bla.Centre;
                    Bla.Rotation = MathHelper.Pi;
                    Bla.Effets = SpriteEffects.None;
                    break;
                case 3: // bas gauche
                    Bla.Position = new Vector3(Dimension.Left + 16, Dimension.Bottom + 16, 0);
                    Bla.Origine = Bla.Centre;
                    Bla.Rotation = -MathHelper.PiOver2;
                    Bla.Effets = SpriteEffects.None;
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                Simulation.Scene.ajouterScenable(Contours[i]);
                Simulation.Scene.ajouterScenable(Coins[i]);
            }

            Simulation.Scene.ajouterScenable(Filtre);
            Simulation.Scene.ajouterScenable(Bla);
        }


        public virtual void doShow(double temps)
        {
            foreach (var coin in Coins)
            {
                coin.Couleur.A = 0;
                Simulation.Scene.Effets.Add(coin, PredefinedEffects.FadeInFrom0(255, 0, temps));
            }

            foreach (var contour in Contours)
            {
                contour.Couleur.A = 0;
                Simulation.Scene.Effets.Add(contour, PredefinedEffects.FadeInFrom0(255, 0, temps));
            }

            Filtre.Couleur.A = 0;
            Bla.Couleur.A = 0;

            Simulation.Scene.Effets.Add(Filtre, PredefinedEffects.FadeInFrom0(128, 0, temps));
            Simulation.Scene.Effets.Add(Bla, PredefinedEffects.FadeInFrom0(255, 0, temps));
        }


        public virtual void doHide(double temps)
        {
            foreach (var coin in Coins)
                Simulation.Scene.Effets.Add(coin, PredefinedEffects.FadeOutTo0(255, 0, temps));

            foreach (var contour in Contours)
                Simulation.Scene.Effets.Add(contour, PredefinedEffects.FadeOutTo0(255, 0, temps));

            Simulation.Scene.Effets.Add(Filtre, PredefinedEffects.FadeOutTo0(128, 0, temps));
            Simulation.Scene.Effets.Add(Bla, PredefinedEffects.FadeOutTo0(255, 0, temps));
        }
    }
}
