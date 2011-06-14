namespace EphemereGames.Core.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content;
    using ParallelTasks;


    class AssetsController
    {
        private Dictionary<string, Package> permanentPackages = new Dictionary<string, Package>();
        private Dictionary<string, Package> temporaryPackages = new Dictionary<string, Package>();
        private Dictionary<string, IAsset> TypesAssets = new Dictionary<string, IAsset>();


        public AssetsController()
        {
            XDocument packagesElements = XDocument.Load(Preferences.ContentFolderPath + "\\" + Preferences.PackagesFolderPath);

            var packagesAll =
              (from package in packagesElements.Descendants("Package")
               select new Package
               {
                   AssetsPool = new ContentManager(Preferences.GameServiceContainer, Preferences.ContentFolderPath + "/"),
                   Name = package.Attribute("nom").Value,
                   Temporary = Boolean.Parse(package.Attribute("temporaire").Value),

                   Assets = (from asset in package.Descendants("asset")
                   select new AssetDescriptor
                   {
                       Name = asset.Attribute("nom").Value,
                       Type = asset.Attribute("type").Value,
                       Path = asset.Attribute("chemin").Value,
                       Parameters = asset.Elements("param").ToDictionary(x => x.Attribute("nom").Value, x => x.Attribute("valeur").Value)
                   }).ToList()
               }).ToList();

            foreach (var package in packagesAll)
                if (package.Temporary)
                    temporaryPackages.Add(package.Name, package);
                else
                    permanentPackages.Add(package.Name, package);
        }


        public void LoadPackage(string packageName)
        {
            Package package;

            if (temporaryPackages.ContainsKey(packageName))
                package = temporaryPackages[packageName];
            else
                package = permanentPackages[packageName];

            if (package.Loaded)
                return;

            Parallel.StartBackground(package.Load);
        }


        public void UnloadPackage(string packageName)
        {
            if (temporaryPackages.ContainsKey(packageName))
                Parallel.StartBackground(temporaryPackages[packageName].Unload);
            else
                Parallel.StartBackground(permanentPackages[packageName].Unload);
        }


        public bool IsPackageLoaded(string packageName)
        {
            if (temporaryPackages.ContainsKey(packageName))
                return temporaryPackages[packageName].Loaded;

            return permanentPackages[packageName].Loaded;
        }


        public T GetAsset<T>(string name)
        {
            foreach (var kvp in permanentPackages)
            {
                object asset = kvp.Value.Get(name);

                if (asset != null)
                    return (T) asset;
            }

            foreach (var kvp in temporaryPackages)
            {
                object asset = kvp.Value.Get(name);

                if (asset != null)
                    return (T) asset;
            }

            return default(T);
        }


        public T GetAssetCopy<T>(string name)
        {
            T objet = GetAsset<T>(name);

            objet = (T) ((IAsset) objet).Clone();

            return objet;
        }


        public void AddAssetType(IAsset assetType)
        {
            TypesAssets.Add(assetType.AssetType, assetType);
        }


        public IAsset GetAssetType(string assetType)
        {
            return TypesAssets[assetType];
        }
    }
}