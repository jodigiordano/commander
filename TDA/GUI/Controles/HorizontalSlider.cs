namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    
    class HorizontalSlider : DrawableGameComponent
    {
        private Scene Scene;
        private Main Main;
        private Cursor Curseur;
        private IVisible Gauche;
        private IVisible Droite;
        private IVisible ValeurRep;
        private Vector3 Position;
        private Cercle GaucheCercle;
        private Cercle DroiteCercle;

        public int Valeur, Min, Max, Increment;

        public HorizontalSlider(Main main, Scene scene, Cursor curseur, Vector3 position, int min, int max, int valeur, int increment, float priorite)
            : base(main)
        {
            Main = main;
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Valeur = valeur;
            Min = min;
            Max = max;
            Increment = increment;

            Gauche = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Gauche"), Position - new Vector3(50, 0, 0), Scene);
            Gauche.PrioriteAffichage = priorite;
            Gauche.Origine = Gauche.Centre;

            Droite = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Droite"), Position + new Vector3(50, 0, 0), Scene);
            Droite.PrioriteAffichage = priorite;
            Droite.Origine = Droite.Centre;

            ValeurRep = new IVisible(Valeur.ToString(), Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Position, Scene);
            ValeurRep.Taille = 2;
            ValeurRep.PrioriteAffichage = priorite;
            ValeurRep.Origine = ValeurRep.Centre;

            GaucheCercle = new Cercle(Gauche.Position, 16);
            DroiteCercle = new Cercle(Droite.Position, 16);
        }

        public override void Update(GameTime gameTime)
        {
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, Scene.Nom) &&
                Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, GaucheCercle) &&
                Valeur > Min)
            {
                Valeur = Math.Max(Min, Valeur - Increment);
            }

            else if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, Scene.Nom) &&
                Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, DroiteCercle) &&
                Valeur < Max)
            {
                Valeur = Math.Min(Max, Valeur + Increment);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ValeurRep.Texte = Valeur.ToString();

            Scene.ajouterScenable(Gauche);
            Scene.ajouterScenable(Droite);
            Scene.ajouterScenable(ValeurRep);
        }
    }
}
