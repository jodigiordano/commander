//=====================================================================
//
// Sauvegarde et chargements de données
//
//=====================================================================

namespace Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Core.Utilities;
    
    class GestionnaireDonnees
    {
        private static GestionnaireDonnees instance = new GestionnaireDonnees();

        private Dictionary<string, AbstractDonnee> Donnees      { get; set; }

        private GestionnaireDonnees()
        {
            this.Donnees = new Dictionary<string, AbstractDonnee>();
        }


        public static GestionnaireDonnees Instance
        {
            get { return instance; }
        }

        
        public void ajouterDonnee(AbstractDonnee donnee)
        {
            this.Donnees.Add(donnee.Nom, donnee);
        }


        //=====================================================================
        // Sauvegarde/Récupération dans un fichier
        //=====================================================================

        /// <summary>
        /// Sauvegarder une donnee
        /// </summary>
        /// <param name="nomDonnee">Nom de la donnee</param>
        public void sauvegarder(string nomDonnee)
        {
            Preferences.ThreadDonnees.AddTask(new ThreadTask(Donnees[nomDonnee].sauvegarder));
        }

        /// <summary>
        /// Charger une donnee
        /// </summary>
        /// <param name="nomDonnee">Nom de la donnee</param>
        public void charger(string nomDonnee)
        {
            Preferences.ThreadDonnees.AddTask(new ThreadTask(Donnees[nomDonnee].charger));
        }

        /// <summary>
        /// Update le(s) save device (qui gère(nt) les interactions avec le Guide)
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Preferences.ThreadDonnees.AddTask(new ThreadTask(delegate
            {
                // Update du SaveDevice du profil s'il existe
                foreach(var donnee in Donnees)
                    donnee.Value.Update(gameTime);
            }));
        }

        public void initialiserDonneeJoueur(string nomDonnee, PlayerIndex joueur)
        {
            ((AbstractDonneeJoueur)Donnees[nomDonnee]).Initialize(joueur);
        }

        public bool donneeEstChargee(string nomDonnee)
        {
            return (Donnees.ContainsKey(nomDonnee) && Donnees[nomDonnee].Charge);
        }
    }
}
