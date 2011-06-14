namespace EphemereGames.Core.Persistence
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using SpriteSheetRuntime;
    

    class Package
    {
        public bool Loaded                                  { get; private set; }
        public string Name                                  { get; set; }
        public bool Temporary                               { get; set; }
        public ContentManager AssetsPool                    { get; set; }
        public List<AssetDescriptor> Assets                 { get; set; }
        
        private Dictionary<string, object> LoadedAssets     { get; set; }


        public Package()
        {
            LoadedAssets = new Dictionary<string, object>();
            Loaded = false;
        }


        public object Get(string name)
        {
            if (LoadedAssets.ContainsKey(name))
                return LoadedAssets[name];

            return null;
        }


        public void Load()
        {
            foreach (var asset in Assets)
            {
                if (asset.Type == "Texture2D")
                    LoadedAssets.Add(asset.Name, AssetsPool.Load<Texture2D>(asset.Path));
                else if (asset.Type == "Video")
                    LoadedAssets.Add(asset.Name, AssetsPool.Load<Video>(asset.Path));
                else if (asset.Type == "EffetFX")
                    LoadedAssets.Add(asset.Name, AssetsPool.Load<Effect>(asset.Path));
                else if (asset.Type == "Police")
                    LoadedAssets.Add(asset.Name, AssetsPool.Load<SpriteFont>(asset.Path));
                else if (asset.Type == "SpriteSheet")
                    LoadedAssets.Add(asset.Name, AssetsPool.Load<SpriteSheet>(asset.Path));
                else
                {
                    var obj = Persistence.AssetsController.GetAssetType(asset.Type).Load(
                        asset.Name,
                        asset.Path,
                        asset.Parameters,
                        this.AssetsPool);

                    LoadedAssets.Add(asset.Name, obj);
                }
            }

            Loaded = true;
        }


        public void Unload()
        {
            AssetsPool.Unload();
            LoadedAssets.Clear();
            Loaded = false;
        }
    }
}
