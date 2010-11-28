namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class VueAvancee : DrawableGameComponent
    {
        // Donnees externes
        public Emplacement EmplacementSelectionne;

        private Scene Scene;
        private PointsDeVieEnnemis ViesEnnemis;
        private List<CorpsCeleste> CorpsCelestes;
        private List<IVisible> Cercles;

        private int NbCercles;

        public VueAvancee(Simulation simulation, List<Ennemi> ennemis, List<CorpsCeleste> corpsCelestes)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            CorpsCelestes = corpsCelestes;
            ViesEnnemis = new PointsDeVieEnnemis(simulation, ennemis);

            Cercles = new List<IVisible>();
            NbCercles = 50;

            for (int i = 0; i < NbCercles; i++)
            {
                IVisible iv = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("CercleBlanc"), Vector3.Zero, Scene);
                iv.Couleur = new Color(Color.Black, 200);
                iv.Origine = iv.Centre;
                iv.PrioriteAffichage = Preferences.PrioriteGUIEtoiles - 0.001f;

                Cercles.Add(iv);
            }
        }

        public override void Update(GameTime gameTime)
        {
            ViesEnnemis.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            ViesEnnemis.Draw(null);

            int compteurCercles = 0;

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                CorpsCeleste corps = CorpsCelestes[i];

                for (int j = 0; j < corps.Emplacements.Count; j++)
                {
                    Emplacement emplacement = corps.Emplacements[j];

                    if (emplacement.EstOccupe)
                    {
                        Cercles[compteurCercles].Position = emplacement.Tourelle.Position;
                        Cercles[compteurCercles].Couleur = new Color(emplacement.Tourelle.Couleur, 100);
                        Cercles[compteurCercles].Taille = (emplacement.Tourelle.ZoneActivation.Rayon / 100) * 2;
                        Scene.ajouterScenable(Cercles[compteurCercles]);

                        compteurCercles++;

                        if (compteurCercles >= 50)
                            return;
                    }
                }
            }
        }
    }
}
