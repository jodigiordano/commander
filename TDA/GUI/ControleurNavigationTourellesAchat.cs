namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;

    class ControleurNavigationTourellesAchat
    {
        // Donneees externes
        public CorpsCeleste CorpsCelesteSelectionne;
        public Emplacement EmplacementSelectionne;

        // Out
        public Dictionary<Tourelle, bool> Tourelles { get; private set; }
        public Tourelle Selectionne                 { get { return (selectionne == null) ? null : selectionne.Value; } }
        public CorpsCeleste ProjectionEnCours;

        private LinkedListNode<Tourelle> selectionne;
        private LinkedList<Tourelle> TourellesDisponibles;
        private LinkedList<Tourelle> TourellesNonDisponibles;
        
        private JoueurCommun Joueur;
        private Chemin CheminProjection;
         

        public ControleurNavigationTourellesAchat(JoueurCommun joueur, Chemin cheminProjection)
        {
            Joueur = joueur;
            CheminProjection = cheminProjection;

            CorpsCelesteSelectionne = null;
            EmplacementSelectionne = null;

            selectionne = null;
            Tourelles = new Dictionary<Tourelle, bool>();
            TourellesDisponibles = new LinkedList<Tourelle>();
            TourellesNonDisponibles = new LinkedList<Tourelle>();

            ProjectionEnCours = null;
        }


        public void Suivant()
        {
            if (selectionne == null)
                return;

            selectionne = (selectionne.Next == null) ? TourellesDisponibles.First : selectionne.Next;
        }


        public void Precedent()
        {
            if (selectionne == null)
                return;

            selectionne = (selectionne.Previous == null) ? TourellesDisponibles.Last : selectionne.Previous;
        }


        public void Update(GameTime gameTime)
        {
            int nbDisponiblesAvant = TourellesDisponibles.Count;
            bool selectionneAvant = selectionne != null;

            Tourelles.Clear();
            TourellesDisponibles.Clear();
            TourellesNonDisponibles.Clear();

            if (CorpsCelesteSelectionne == null || EmplacementSelectionne == null || EmplacementSelectionne.EstOccupe)
            {
                selectionne = null;
                determinerProjection();
                return;
            }

            for (int i = 0; i < CorpsCelesteSelectionne.TourellesPermises.Count; i++)
            {
                Tourelle tourelle = CorpsCelesteSelectionne.TourellesPermises[i];

                if (tourelle.PrixAchat <= Joueur.ReserveUnites)
                    TourellesDisponibles.AddLast(tourelle);
                else
                    TourellesNonDisponibles.AddLast(tourelle);

                Tourelles.Add(tourelle, false);
            }

            foreach (var tourelle in TourellesDisponibles)
                Tourelles[tourelle] = true;


            if (TourellesDisponibles.Count > 0 && nbDisponiblesAvant == 0)
                selectionne = TourellesDisponibles.First;
            else if (TourellesDisponibles.Count == 0)
                selectionne = null;
            else if (selectionneAvant)
                selectionne = TourellesDisponibles.Find(selectionne.Value);

            determinerProjection();
        }


        private void determinerProjection()
        {
            if (ProjectionEnCours == null &&
                selectionne != null &&
                selectionne.Value.Type == TypeTourelle.Gravitationnelle &&
                !CheminProjection.contientCorpsCeleste(CorpsCelesteSelectionne))
            {
                CorpsCelesteSelectionne.ContientTourelleGravitationnelleByPass = true;
                CheminProjection.ajouterCorpsCeleste(CorpsCelesteSelectionne);
                ProjectionEnCours = CorpsCelesteSelectionne;
            }

            else if (ProjectionEnCours != null &&
                    (selectionne == null || selectionne.Value.Type != TypeTourelle.Gravitationnelle))
            {
                ProjectionEnCours.ContientTourelleGravitationnelleByPass = false;
                CheminProjection.enleverCorpsCeleste(ProjectionEnCours);
                ProjectionEnCours = null;
            }
        }


        public void doTourelleAchetee(Tourelle tourelle)
        {
            ProjectionEnCours = null;
        }
    }
}
