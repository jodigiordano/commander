//=============================================================================
//
// Point d'entrée dans la librairie
//
//=============================================================================

namespace Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;

    
    public static class Facade
    {
        public static void Initialize(
            String dossierContenu,
            String cheminRelatifPackages,
            GameServiceContainer gsc,
            ManagedThread threadContenu,
            ManagedThread threadDonnees,
            StorageMessages storageMessages)
        {
            Preferences.DossierContenu = dossierContenu;
            Preferences.CheminRelatifPackages = cheminRelatifPackages;
            Preferences.GameServiceContainer = gsc;
            Preferences.ThreadContenu = threadContenu;
            Preferences.ThreadDonnees = threadDonnees;
            Preferences.StorageMessages = storageMessages;
        }

        public static T recuperer<T>(String nom)
        {
            return GestionnaireContenu.Instance.recuperer<T>(nom);
        }

        public static T recupererParCopie<T>(String nom)
        {
            return GestionnaireContenu.Instance.recupererParCopie<T>(nom);
        }

        public static void charger(String nomPackage)
        {
            GestionnaireContenu.Instance.charger(nomPackage);
        }

        public static void charger(int niveau)
        {
            GestionnaireContenu.Instance.charger(niveau);
        }

        public static void decharger(String nomPackage)
        {
            GestionnaireContenu.Instance.decharger(nomPackage);
        }


        public static bool estCharge(string nomAsset)
        {
            return GestionnaireContenu.Instance.estCharge(nomAsset);
        }

        public static bool estCharge(int noNiveau)
        {
            return GestionnaireContenu.Instance.estCharge(noNiveau);
        }

        //public static bool estCharge(String nomPackage)
        //{
        //    return GestionnaireContenu.Instance.estCharge(nomPackage);
        //}

        public static void ajouterTypeAsset(IContenu typeAsset)
        {
            GestionnaireContenu.Instance.ajouterTypeAsset(typeAsset);
        }

        public static void ajouterDonnee(AbstractDonnee donnee)
        {
            GestionnaireDonnees.Instance.ajouterDonnee(donnee);
        }

        public static void sauvegarderDonnee(String nomDonnee)
        {
            GestionnaireDonnees.Instance.sauvegarder(nomDonnee);
        }

        public static void chargerDonnee(String nomDonnee)
        {
            GestionnaireDonnees.Instance.charger(nomDonnee);
        }

        public static void initialiserDonneesJoueur(String nomDonnee, PlayerIndex joueur)
        {
            GestionnaireDonnees.Instance.initialiserDonneeJoueur(nomDonnee, joueur);
        }

        public static bool donneeEstCharge(string nomDonnee)
        {
            return GestionnaireDonnees.Instance.donneeEstChargee(nomDonnee);
        }

        public static void Update(GameTime gameTime)
        {
            GestionnaireDonnees.Instance.Update(gameTime);
        }

        public static String getRepertoireContenu()
        {
            return GestionnaireContenu.Instance.RepertoireContenu;
        }
    }
}
