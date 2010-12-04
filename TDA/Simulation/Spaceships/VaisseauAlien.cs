namespace TDA.Objets
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using Core.Physique;

    public class VaisseauAlien
    {
        private Scene Scene;
        public Sprite Tentacules;
        public IVisible Representation;
        //private List<IVisible> Tentacules;
        //private int noChaine;
        //private int nbChaines;

        public VaisseauAlien(Scene scene, float prioriteAffichage)
        {
            Scene = scene;

            Representation = new IVisible
            (
                Core.Persistance.Facade.recuperer<Texture2D>("VaisseauAlien"),
                Vector3.Zero,
                Scene
            );
            Representation.Taille = 8;
            Representation.Origine = Representation.Centre;
            Representation.PrioriteAffichage = prioriteAffichage;

            Tentacules = Core.Persistance.Facade.recupererParCopie<Sprite>("tentacules");
            Tentacules.Taille = 8;
            Tentacules.Origine = Tentacules.Centre;
            Tentacules.PrioriteAffichage = Representation.PrioriteAffichage + 0.01f;

            //noChaine = Core.Physique.Facade.creerChaine(new Vector2(Representation.Position.X, Representation.Position.Y), new Vector2(0, -400), 100);

            //Tentacules = new List<IVisible>();

            //nbChaines = Core.Physique.Facade.getNbMaillonsChaine(noChaine);

            //for (int i = 0; i < nbChaines; i++)
            //    Tentacules.Add(new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Tentacule"), new Vector3(Core.Physique.Facade.getPositionMaillon(noChaine, i), 0), Scene));
        }

        public void Update(GameTime gameTime)
        {
            Tentacules.Update(gameTime);

            //Core.Physique.Facade.deplacerMaillon(noChaine, 0, new Vector2(Representation.Position.X, Representation.Position.Y));
        }

        public void Draw(GameTime gameTime)
        {
            Tentacules.Position = Representation.Position;

            Scene.ajouterScenable(Representation);
            Scene.ajouterScenable(Tentacules);

            //for (int i = 0; i < nbChaines; i++)
            //{
            //    Tentacules[i].Position = new Vector3(Core.Physique.Facade.getPositionMaillon(noChaine, i), 0);
            //    Scene.ajouterScenable(Tentacules[i]);
            //}
        }
    }
}
