namespace EphemereGames.Core.Persistence
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;


    public interface IAsset
    {
        string AssetType { get; }

        IAsset Load(string name, string path, Dictionary<string, string> parameters, ContentManager manager);
        void Unload();

        object Clone();
    }
}
