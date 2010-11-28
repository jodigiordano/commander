//=====================================================================
//
// Charge et d�charge les assets de la m�moire
//
// LE "CONSTRUCTEUR" Initialize(GameServiceContainer) DOIT �TRE APPEL�
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
        // R�cup�rer un asset charg�
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

            throw new Exception("Asset introuvable ! Pas suppos�");
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

        // packages qui ne sont jamais d�charg�s
        private Dictionary<String, Package> packagesPermanents = new Dictionary<string, Package>();
        
        // packages qui sont d�charg�s (habituellement un package � l fois de charg�)
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
            System.Diagnostics.Debug.Assert(noNiveau > 0, "Suppos� charger un VRAI niveau !");

            // ne pas charger un niveau deux fois
            // (consid�re qu'un niveau doit �tre charg� compl�tement en m�moire, c�d intro/dialogue/niveau etc.)
            foreach (var kvp in packagesTemporaires)
            {
                if (kvp.Value.Niveau == noNiveau)
                {
                    if (kvp.Value.Charge)
                        return;
                }
            }

            // d�charger tous les packages temporaires d�j� charg�s
            foreach (var kvp in packagesTemporaires)
                if (kvp.Value.Niveau > 0 && kvp.Value.Charge)
                    Preferences.ThreadContenu.AddTask(new ThreadTask(kvp.Value.decharger));

            // charger tous les packages temporaires se rapportant au niveau demand�
            foreach (var kvp in packagesTemporaires)
                if (kvp.Value.Niveau == noNiveau)
                    Preferences.ThreadContenu.AddTask(new ThreadTask(kvp.Value.charger));
        }

        public bool estCharge(int noNiveau)
        {
            // (consid�re qu'un niveau doit �tre charg� compl�tement en m�moire, c�d intro/dialogue/jeu etc.)
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
        /// Ajouter un type d'asset qui peut �tre persist�
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