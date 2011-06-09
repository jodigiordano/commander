namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Bulle
    {
        protected Simulation Simulation;

        private List<IVisible> Corners;
        private List<IVisible> Edges;
        protected IVisible Filter;
        private IVisible Bla;

        public Rectangle Dimension;
        public int BlaPosition;


        public Bulle(Simulation simulation, Rectangle dimension, double prioriteAffichage)
        {
            this.Simulation = simulation;
            this.Dimension = dimension;

            Bla = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleBlabla"), Vector3.Zero);
            Bla.VisualPriority = prioriteAffichage;

            Filter = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
            Filter.Couleur = new Color(0, 0, 0, 200);
            Filter.VisualPriority = prioriteAffichage + 0.00002f;

            Corners = new List<IVisible>();
            Edges = new List<IVisible>();

            for (int i = 0; i < 4; i++)
            {
                IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleCoin"), Vector3.Zero);
                iv.VisualPriority = prioriteAffichage + 0.00001f;

                Corners.Add(iv);

                iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
                iv.VisualPriority = prioriteAffichage + 0.00001f;

                Edges.Add(iv);
            }
        }


        public virtual void Show()
        {
            for (int i = 0; i < 4; i++)
            {
                Simulation.Scene.Add(Edges[i]);
                Simulation.Scene.Add(Corners[i]);
            }

            Simulation.Scene.Add(Filter);
            Simulation.Scene.Add(Bla);
        }


        public virtual void Hide()
        {
            for (int i = 0; i < 4; i++)
            {
                Simulation.Scene.Remove(Edges[i]);
                Simulation.Scene.Remove(Corners[i]);
            }

            Simulation.Scene.Remove(Filter);
            Simulation.Scene.Remove(Bla);
        }


        public virtual void Draw()
        {
            Dimension.Height = Math.Max(Dimension.Height, 30);

            Filter.TailleVecteur = new Vector2(Dimension.Width + 8, Dimension.Height + 8);
            Filter.Position = new Vector3(Dimension.Left - 4, Dimension.Top - 4, 0);

            Corners[0].Position = new Vector3(Dimension.Left - 8, Dimension.Top - 8, 0); //Haut droite
            Corners[0].Rotation = 0;

            Corners[1].Position = new Vector3(Dimension.Left - 4, Dimension.Bottom + 4, 0); //Bas droite
            Corners[1].Origine = Corners[1].Centre;
            Corners[1].Rotation = -MathHelper.PiOver2;

            Corners[2].Position = new Vector3(Dimension.Right + 8, Dimension.Top - 8, 0); //Haut gauche
            Corners[1].Origine = Corners[1].Centre;
            Corners[2].Rotation = MathHelper.PiOver2;

            Corners[3].Position = new Vector3(Dimension.Right + 8, Dimension.Bottom + 8, 0); //Bas gauche
            Corners[1].Origine = Corners[1].Centre;
            Corners[3].Rotation = MathHelper.Pi;


            Edges[0].TailleVecteur = new Vector2(Dimension.Width, 4); //Haut
            Edges[0].Position = new Vector3(Dimension.Left, Dimension.Top - 8, 0);

            Edges[1].TailleVecteur = new Vector2(Dimension.Width, 4); //Bas
            Edges[1].Position = new Vector3(Dimension.Left, Dimension.Bottom + 4, 0);

            Edges[2].TailleVecteur = new Vector2(4, Dimension.Height); //Gauche
            Edges[2].Position = new Vector3(Dimension.Left - 8, Dimension.Top, 0);

            Edges[3].TailleVecteur = new Vector2(4, Dimension.Height); //Droite
            Edges[3].Position = new Vector3(Dimension.Right + 4, Dimension.Top, 0);

            switch (BlaPosition)
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
        }


        public virtual void FadeIn(double time)
        {
            foreach (var coin in Corners)
            {
                coin.Couleur.A = 0;
                Simulation.Scene.Effects.Add(coin, PredefinedEffects.FadeInFrom0(255, 0, time));
            }

            foreach (var contour in Edges)
            {
                contour.Couleur.A = 0;
                Simulation.Scene.Effects.Add(contour, PredefinedEffects.FadeInFrom0(255, 0, time));
            }

            Filter.Couleur.A = 0;
            Bla.Couleur.A = 0;

            Simulation.Scene.Effects.Add(Filter, PredefinedEffects.FadeInFrom0(128, 0, time));
            Simulation.Scene.Effects.Add(Bla, PredefinedEffects.FadeInFrom0(255, 0, time));
        }


        public virtual void FadeOut(double time)
        {
            foreach (var coin in Corners)
                Simulation.Scene.Effects.Add(coin, PredefinedEffects.FadeOutTo0(255, 0, time));

            foreach (var contour in Edges)
                Simulation.Scene.Effects.Add(contour, PredefinedEffects.FadeOutTo0(255, 0, time));

            Simulation.Scene.Effects.Add(Filter, PredefinedEffects.FadeOutTo0(128, 0, time));
            Simulation.Scene.Effects.Add(Bla, PredefinedEffects.FadeOutTo0(255, 0, time));
        }
    }
}
