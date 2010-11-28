//=====================================================================
//
// Gestionnaire des effets
//
//=====================================================================

namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public class GestionnaireEffets
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        protected List<AbstractEffet> Effets { get; set; }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public GestionnaireEffets()
        {
            Effets = new List<AbstractEffet>();
        }


        //=====================================================================
        // Services
        //=====================================================================

        /// <summary>
        /// Nombre d'effets actifs
        /// </summary>
        public int NbEffetsActifs
        {
            get
            {
                return Effets.Count;
            }
        }


        /// <summary>
        /// Mise à jour
        /// </summary>
        /// <param name="gameTime">temps</param>
        public virtual void Update(GameTime gameTime)
        {
            for (int i = Effets.Count - 1; i > -1; i--)
            {
                AbstractEffet effet = Effets[i];

                if (!effet.Termine)
                    Effets[i].Update(gameTime);
                else
                    Effets.RemoveAt(i);
            }
        }


        /// <summary>
        /// Insertion d'un effet sur un IVisible
        /// </summary>
        /// <param name="iVisible">L'objet concerné</param>
        /// <param name="effet">Un effet</param>
        public void ajouter(object objet, AbstractEffet effet)
        {
            effet.objet = objet;

            Effets.Add(effet);
        }


        /// <summary>
        /// Insertion de plusieurs effets sur un IVisible
        /// </summary>
        /// <param name="iVisible">L'objet concerné</param>
        /// <param name="listeEffets">Liste des effets</param>
        public void ajouter(object objet, List<AbstractEffet> listeEffets)
        {
            for (int i = 0; i < listeEffets.Count; i++)
                ajouter(objet, listeEffets[i]);
        }


        /// <summary>
        /// Stopper les effets
        /// Appliquer leurs valeurs finales
        /// Réinitialiser les effets
        /// </summary>
        public void stop()
        {
            for (int i = 0; i < Effets.Count; i++)
            {
                Effets[i].Termine = true;
                Effets[i].Update(null);
                Effets[i].init();                
            }
        }


        /// <summary>
        /// Vider la liste des effets
        /// </summary>
        public void vider()
        {
            Effets.Clear();
        }

        /// <summary>
        /// Détermine si un groupe d'effets sont terminés
        /// </summary>
        /// <param name="effets">Liste des effets</param>
        /// <returns>Retourne vrai si tous les effets sont termines</returns>
        public bool EffetsTermines(List<AbstractEffet> effets)
        {
            foreach (var effet in effets)
                if (!effet.Termine)
                    return false;

            return true;
        }
    }
}
