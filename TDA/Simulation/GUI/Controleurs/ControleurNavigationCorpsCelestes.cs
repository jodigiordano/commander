namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;

    class ControleurNavigationCorpsCelestes : GameComponent
    {
        private Scene Scene;
        private int corpsCelesteSelectionne;
        private int emplacementSelectionne;
        private List<CorpsCeleste> CorpsCelestes;
        private Curseur Curseur;

        public bool CorpsCelesteSelectionneChange;
        public bool EmplacementSelectionneChange;
        public Vector3 AnciennePositionCorpsCelesteSelectionne;


        public ControleurNavigationCorpsCelestes(Simulation simulation, List<CorpsCeleste> corpsCelestes, Curseur curseur)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;
            CorpsCelestes = corpsCelestes;
            Curseur = curseur;

            corpsCelesteSelectionne = -1;
            emplacementSelectionne = -1;
            AnciennePositionCorpsCelesteSelectionne = new Vector3(float.NaN);
        }


        public CorpsCeleste CorpsCelesteSelectionne
        {
            get { return (corpsCelesteSelectionne == -1) ? null : CorpsCelestes[corpsCelesteSelectionne]; }
        }


        public Emplacement EmplacementSelectionne
        {
            get { return (emplacementSelectionne == -1) ? null : CorpsCelestes[corpsCelesteSelectionne].Emplacements[emplacementSelectionne]; }
        }

        


        public override void Update(GameTime gameTime)
        {
            doSelectionEmplacement();
            doSelectionCorpsCeleste();
            doGlueMode();
        }

        private void doSelectionEmplacement()
        {
            int ancien = emplacementSelectionne;

            emplacementSelectionne = -1;

            if (!Curseur.Actif)
                return;

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                if (!CorpsCelestes[i].Selectionnable)
                    continue;

                for (int j = 0; j < CorpsCelestes[i].Emplacements.Count; j++)
                    if (Core.Physique.Facade.collisionCercleRectangle(Curseur.Cercle, CorpsCelestes[i].Emplacements[j].Rectangle))
                    {
                        CorpsCelesteSelectionneChange = corpsCelesteSelectionne != i;

                        corpsCelesteSelectionne = i;
                        emplacementSelectionne = j;
                        break;
                    }
            }

            EmplacementSelectionneChange = emplacementSelectionne != ancien;
        }

        private void doSelectionCorpsCeleste()
        {
            if (emplacementSelectionne != -1)
                return;

            int ancien = corpsCelesteSelectionne;

            corpsCelesteSelectionne = -1;

            if (!Curseur.Actif)
                return;

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                if (!CorpsCelestes[i].Selectionnable)
                    continue;

                if (Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, CorpsCelestes[i].Cercle))
                {
                    corpsCelesteSelectionne = i;
                    break;
                }
            }
            CorpsCelesteSelectionneChange = corpsCelesteSelectionne != ancien;
        }

        private void doGlueMode()
        {
            if (corpsCelesteSelectionne != -1 && CorpsCelesteSelectionneChange)
                AnciennePositionCorpsCelesteSelectionne = CorpsCelestes[corpsCelesteSelectionne].Position;

            if (corpsCelesteSelectionne != -1)
            {
                Curseur.Position += (CorpsCelestes[corpsCelesteSelectionne].Position - AnciennePositionCorpsCelesteSelectionne);

                AnciennePositionCorpsCelesteSelectionne = CorpsCelestes[corpsCelesteSelectionne].Position;
            }
        }

        public void doCorpsCelesteDetruit()
        {
            //Selectionne = (CorpsCelestes.Count - 1 == 0) ? -1 : 0;
            corpsCelesteSelectionne = -1;
        }
    }
}
