namespace EphemereGames.Core.SimplePersistence
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using SpriteSheetRuntime;
#if !WINDOWS_PHONE
    using Microsoft.Xna.Framework.Media;
#endif


    class Package
    {
        public bool Loaded                                  { get; private set; }
        public string Name                                  { get; set; }
        public bool Temporary                               { get; set; }
        public ContentManager AssetsPool                    { get; set; }
        public List<AssetDescriptor> Assets                 { get; set; }
        
        private Dictionary<string, IAsset> LoadedAssets         { get; set; }
        private Dictionary<string, object> ManagedLoadedAssets  { get; set; }


        public Package()
        {
            LoadedAssets = new Dictionary<string, IAsset>();
            ManagedLoadedAssets = new Dictionary<string, object>();
            Loaded = false;
        }


        public object Get(string name)
        {
            bool exists;

            IAsset asset;
            object o;

            exists = LoadedAssets.TryGetValue(name, out asset);

            if (exists)
                return asset;

            exists = ManagedLoadedAssets.TryGetValue(name, out o);

            if (exists)
                return o;

            return null;
        }


        public void Load()
        {
            foreach (var asset in Assets)
            {
                if (asset.Type == "Image")
                    ManagedLoadedAssets.Add(asset.Name, AssetsPool.Load<Texture2D>(asset.Path));
#if !WINDOWS_PHONE
                else if (asset.Type == "Video")
                    ManagedLoadedAssets.Add(asset.Name, AssetsPool.Load<Video>(asset.Path));
#endif
                else if (asset.Type == "VisualEffect")
                    ManagedLoadedAssets.Add(asset.Name, AssetsPool.Load<Effect>(asset.Path));
                else if (asset.Type == "Font")
                    ManagedLoadedAssets.Add(asset.Name, AssetsPool.Load<SpriteFont>(asset.Path));
                else if (asset.Type == "SpriteSheet")
                    ManagedLoadedAssets.Add(asset.Name, AssetsPool.Load<SpriteSheet>(asset.Path));
                else
                {
                    var obj = Persistence.AssetsController.GetAssetType(asset.Type).Load(
                        asset.Name,
                        asset.Path,
                        asset.Parameters,
                        AssetsPool);

                    LoadedAssets.Add(asset.Name, obj);
                }
            }

            Loaded = true;
        }


        public void Unload()
        {
            foreach (var asset in LoadedAssets.Values)
                asset.Unload();

            AssetsPool.Unload();
            ManagedLoadedAssets.Clear();
            LoadedAssets.Clear();
            Loaded = false;
        }
    }
}
