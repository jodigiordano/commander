namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HorizontalSlider
    {
        public int Valeur, Min, Max, Increment;

        private Scene Scene;
        private Image Gauche;
        private Image Droite;
        private Text ValeurRep;
        private Vector3 Position;
        private Circle GaucheCercle;
        private Circle DroiteCercle;


        public HorizontalSlider(Scene scene, Vector3 position, int min, int max, int valeur, int increment, float priorite)
        {
            Scene = scene;
            Position = position;

            Valeur = valeur;
            Min = min;
            Max = max;
            Increment = increment;

            Gauche = new Image("Gauche", Position - new Vector3(50, 0, 0));
            Gauche.VisualPriority = priorite;

            Droite = new Image("Droite", Position + new Vector3(50, 0, 0));
            Droite.VisualPriority = priorite;

            ValeurRep = new Text(Valeur.ToString(), @"Pixelite", Color.White, Position);
            ValeurRep.SizeX = 2;
            ValeurRep.VisualPriority = priorite;
            ValeurRep.Origin = ValeurRep.Center;

            GaucheCercle = new Circle(Gauche.Position, 16);
            DroiteCercle = new Circle(Droite.Position, 16);
        }


        public void DoClick(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, GaucheCercle) && Valeur > Min)
                Valeur = Math.Max(Min, Valeur - Increment);
            else if (Physics.CircleCicleCollision(circle, DroiteCercle) && Valeur < Max)
                Valeur = Math.Min(Max, Valeur + Increment);
        }


        public void Draw()
        {
            ValeurRep.Data = Valeur.ToString();

            Scene.Add(Gauche);
            Scene.Add(Droite);
            Scene.Add(ValeurRep);
        }
    }
}
