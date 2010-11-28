//=====================================================================
//
// Charge et décharge les assets de la mémoire
//
// LE "CONSTRUCTEUR" Initialize(GameServiceContainer) DOIT ÊTRE APPELÉ
//             AVANT TOUTE UTILISATION DE CETTE CLASSE
//
//=====================================================================

namespace Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Linq;
    using System.Xml.Linq;
    using Core.Utilities;


    class GestionnaireContenu
    {
        //=====================================================================
        // Récupérer un asset chargé
        //=====================================================================

        public T recuperer<T>(String nom)
        {
            foreach (var kvp in packagesPermanents)
            {
                object asset = kvp.Value.recuperer(nom);

                if (asset != null)
                    return (T) asset;
            }

            foreach (var kvp in packagesTemporaires)
            {
                object asset = kvp.Value.recuperer(nom);

                if (asset != null)
                    return (T)asset;
            }

            throw new Exception("Asset introuvable ! Pas supposé");
        }

        public T recupererParCopie<T>(String nom)
        {
            T objet = recuperer<T>(nom);

            objet = (T)((IContenu)objet).Clone();

            return objet;
        }


        //=====================================================================
        // Attributs
        //=====================================================================

        private static GestionnaireContenu instance;

        // packages qui ne sont jamais déchargés
        private Dictionary<String, Package> packagesPermanents = new Dictionary<string, Package>();
        
        // packages qui sont déchargés (habituellement un package à l fois de chargé)
        private Dictionary<String, Package> packagesTemporaires = new Dictionary<string, Package>();

        private ContentManager contenuBase;

        private Dictionary<String, IContenu> TypesAssets = new Dictionary<string, IContenu>();


        public String RepertoireContenu
        {
            get { return contenuBase.RootDirectory; }
        }


        //=====================================================================
        // Initialisation
        //=====================================================================

        public GestionnaireContenu()
        {
            contenuBase = new ContentManager(Preferences.GameServiceContainer, Preferences.DossierContenu + "/");

            XDocument packagesElements = XDocument.Load(Preferences.DossierContenu + "\\" + Preferences.CheminRelatifPackages);

            var packagesAll =
              (from package in packagesElements.Descendants("Package")
               select new Package
               {
                   Contenu = new ContentManager(Preferences.GameServiceContainer, Preferences.DossierContenu + "/"),
                   Nom = package.Attribute("nom").Value,
                   Temporaire = Boolean.Parse(package.Attribute("temporaire").Value),
                   Niveau = Int32.Parse(package.Attribute("niveau").Value),

                   Assets = (from asset in package.Descendants("asset")
                   select new DescriptionContenu
                   {
                       Nom = asset.Attribute("nom").Value,
                       Type = asset.Attribute("type").Value,
                       Chemin = asset.Attribute("chemin").Value,
                       Parametres = asset.Elements("param").ToDictionary(x => x.Attribute("nom").Value, x => x.Attribute("valeur").Value)
                   }).ToList()
               }).ToList();

            foreach (var package in packagesAll)
                if (package.Temporaire)
                    packagesTemporaires.Add(package.Nom, package);
                else
                    packagesPermanents.Add(package.Nom, package);
        }


        //===========================================================================
        // Services
        //===========================================================================

        public static GestionnaireContenu Instance
        {
            get
            {
                if (instance == null)
                    instance = new GestionnaireContenu();

                return instance;
            }
        }

        public void charger(int noNiveau)
        {
            System.Diagnostics.Debug.Assert(noNiveau > 0, "Supposé charger un VRAI niveau !");

            // ne pas charger un niveau deux fois
            // (considère qu'un niveau doit être chargé complètement en mémoire, càd intro/dialogue/niveau etc.)
            foreach (var kvp in packagesTemporaires)
            {
                if (kvp.Value.Niveau == noNiveau)
                {
                    if (kvp.Value.Charge)
                        return;
                }
            }

            // décharger tous les packages temporaires déjà chargés
            foreach (var kvp in packagesTemporaires)
                if (kvp.Value.Niveau > 0 && kvp.Value.Charge)
                    Preferences.ThreadContenu.AddTask(new ThreadTask(kvp.Value.decharger));

            // charger tous les packages temporaires se rapportant au niveau demandé
            foreach (var kvp in packagesTemporaires)
                if (kvp.Value.Niveau == noNiveau)
                    Preferences.ThreadContenu.AddTask(new ThreadTask(kvp.Value.charger));
        }

        public bool estCharge(int noNiveau)
        {
            // (considère qu'un niveau doit être chargé complètement en mémoire, càd intro/dialogue/jeu etc.)
            foreach (var kvp in packagesTemporaires)
            {
                if (kvp.Value.Niveau == noNiveau && !kvp.Value.Charge)
                    return false;
            }

            return true;
        }

        public void charger(String nomPackage)
        {
            Package package;

            if (packagesTemporaires.ContainsKey(nomPackage))
                package = packagesTemporaires[nomPackage];
            else
                package = packagesPermanents[nomPackage];

            if (package.Charge)
                return;

            Preferences.ThreadContenu.AddTask(new ThreadTask(package.charger));
        }

        public void decharger(String nomPackage)
        {
            if (packagesTemporaires.ContainsKey(nomPackage))
                Preferences.ThreadContenu.AddTask(new ThreadTask(packagesTemporaires[nomPackage].decharger));
            else
                Preferences.ThreadContenu.AddTask(new ThreadTask(packagesPermanents[nomPackage].decharger));
        }

        public bool estCharge(String nomPackage)
        {
            if (packagesTemporaires.ContainsKey(nomPackage))
                return packagesTemporaires[nomPackage].Charge;

            return packagesPermanents[nomPackage].Charge;
        }

        /// <summary>
        /// Ajouter un type d'asset qui peut être persisté
        /// </summary>
        /// <param name="typeAsset">Type de l'asset</param>
        public void ajouterTypeAsset(IContenu typeAsset)
        {
            TypesAssets.Add(typeAsset.TypeAsset, typeAsset);
        }

        public IContenu getTypeAsset(String nomTypeAsset)
        {
            return TypesAssets[nomTypeAsset];
        }
    }
}