namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;

    class ControleurSystemePlanetaire : DrawableGameComponent
    {
        public List<CorpsCeleste> CorpsCelestes;
        public event PhysicalObjectHandler ObjetDetruit;
        public Chemin Chemin;
        public Chemin CheminProjection;

        private Scene Scene;


        public override void Initialize()
        {
            Chemin.CorpsCelestes = CorpsCelestes;
            Chemin.Initialize();

            CheminProjection.CorpsCelestes = CorpsCelestes;
            CheminProjection.Initialize();
        }


        public ControleurSystemePlanetaire(Simulation simulation)
            : base(simulation.Main)
        {
            this.Scene = simulation.Scene;
            CorpsCelestes = new List<CorpsCeleste>();
            Chemin = new Chemin(simulation, new Color(255, 255, 255, 100), TypeBlend.Alpha);
            CheminProjection = new Chemin(simulation, new Color(255, 255, 255, 255), TypeBlend.Alpha);
        }


        protected virtual void notifyObjetDetruit(IObjetPhysique objet)
        {
            if (ObjetDetruit != null)
                ObjetDetruit(objet);
        }


        public override void Update(GameTime gameTime)
        {
            // Pour les corps celestes qui meurent par eux-memes
            for (int i = CorpsCelestes.Count - 1; i > -1; i--)
                if (!CorpsCelestes[i].EstVivant)
                {
                    notifyObjetDetruit(CorpsCelestes[i]);

                    if (Chemin.contientCorpsCeleste(CorpsCelestes[i]))
                    {
                        Chemin.enleverCorpsCeleste(CorpsCelestes[i]);
                        CheminProjection.enleverCorpsCeleste(CorpsCelestes[i]);
                    }

                    CorpsCelestes.RemoveAt(i);
                }

            for (int i = 0; i < CorpsCelestes.Count; i++)
                CorpsCelestes[i].Update(gameTime);

            Chemin.Update(gameTime);
            CheminProjection.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < CorpsCelestes.Count; i++)
                CorpsCelestes[i].Draw(null);
        }


        public bool ajouterTourelle(Tourelle tourelle, CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            emplacement.Tourelle = tourelle;

            if (tourelle.Type == TypeTourelle.Gravitationnelle &&
                corpsCeleste.Priorite != -1 &&
                !Chemin.contientCorpsCeleste(corpsCeleste))
            {
                Chemin.ajouterCorpsCeleste(corpsCeleste);
                CheminProjection.ajouterCorpsCeleste(corpsCeleste);
            }

            return true;
        }


        public bool enleverTourelle(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            Tourelle tourelle = emplacement.Tourelle;

            int nbTourellesGrav = 0;

            foreach (var emplacement2 in corpsCeleste.Emplacements)
                if (emplacement2.EstOccupe && emplacement2.Tourelle.Type == TypeTourelle.Gravitationnelle)
                    nbTourellesGrav++;

            if (tourelle.Type == TypeTourelle.Gravitationnelle && nbTourellesGrav == 1)
            {
                Chemin.enleverCorpsCeleste(corpsCeleste);
                CheminProjection.enleverCorpsCeleste(corpsCeleste);
            }

            emplacement.Tourelle = null;


            return true;
        }


        public void doDetruireCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            corpsCeleste.doMeurt();

            Chemin.enleverCorpsCeleste(corpsCeleste);
            CheminProjection.enleverCorpsCeleste(corpsCeleste);

            CorpsCelestes.Remove(corpsCeleste);

            notifyObjetDetruit(corpsCeleste);
        }


        public void doObjetCree(IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi == null)
                return;
        }
    }
}
