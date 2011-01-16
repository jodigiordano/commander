namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class HorizontalSlider
    {
        public int Valeur, Min, Max, Increment;

        private Scene Scene;
        private Cursor Curseur;
        private IVisible Gauche;
        private IVisible Droite;
        private IVisible ValeurRep;
        private Vector3 Position;
        private Cercle GaucheCercle;
        private Cercle DroiteCercle;
        

        public HorizontalSlider(Scene scene, Cursor curseur, Vector3 position, int min, int max, int valeur, int increment, float priorite)
        {
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Valeur = valeur;
            Min = min;
            Max = max;
            Increment = increment;

            Gauche = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Gauche"), Position - new Vector3(50, 0, 0));
            Gauche.VisualPriority = priorite;
            Gauche.Origine = Gauche.Centre;

            Droite = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Droite"), Position + new Vector3(50, 0, 0));
            Droite.VisualPriority = priorite;
            Droite.Origine = Droite.Centre;

            ValeurRep = new IVisible(Valeur.ToString(), EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position);
            ValeurRep.Taille = 2;
            ValeurRep.VisualPriority = priorite;
            ValeurRep.Origine = ValeurRep.Centre;

            GaucheCercle = new Cercle(Gauche.Position, 16);
            DroiteCercle = new Cercle(Droite.Position, 16);
        }


        public void doClick()
        {
            if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, GaucheCercle) && Valeur > Min)
                Valeur = Math.Max(Min, Valeur - Increment);
            else if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, DroiteCercle) && Valeur < Max)
                Valeur = Math.Min(Max, Valeur + Increment);
        }


        public void Draw()
        {
            ValeurRep.Texte = Valeur.ToString();

            Scene.ajouterScenable(Gauche);
            Scene.ajouterScenable(Droite);
            Scene.ajouterScenable(ValeurRep);
        }
    }
}
