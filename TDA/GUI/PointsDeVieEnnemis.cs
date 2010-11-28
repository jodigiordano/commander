namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class PointsDeVieEnnemis : DrawableGameComponent
    {
        private List<List<IVisible>> Vies;
        private List<Ennemi> Ennemis;
        private Scene Scene;

        public PointsDeVieEnnemis(Simulation simulation, List<Ennemi> ennemis)
            : base(simulation.Main)
        {
            Ennemis = ennemis;
            Scene = simulation.Scene;

            Vies = new List<List<IVisible>>();

            for (int i = 0; i < 100; i++)
            {
                List<IVisible> representations = new List<IVisible>();

                for (int j = 1; j < 7; j++)
                {
                    IVisible vie = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("ViesEnnemis" + j), Vector3.Zero, Scene);
                    vie.PrioriteAffichage = Preferences.PrioriteGUIVUeAvanceePointsVieEnnemis;
                    vie.Origine = vie.Centre;

                    representations.Add(vie);
                }

                Vies.Add(representations);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            int indiceVies = 0;

            for (int i = 0; i < Ennemis.Count; i++)
            {
                Ennemi ennemi = Ennemis[i];

                float ratioVies = ennemi.PointsVie / ennemi.PointsVieDepart;

                int indice = (int)Math.Round((1 - ratioVies) * 5);

                Vies[indiceVies][indice].Position = ennemi.Position - new Vector3(0, ennemi.RepresentationVivant.Texture.Height * ennemi.RepresentationVivant.TailleVecteur.Y, 0);

                Scene.ajouterScenable(Vies[indiceVies][indice]);
                indiceVies++;

                if (indiceVies >= 100)
                    return;
            }
        }
    }
}
