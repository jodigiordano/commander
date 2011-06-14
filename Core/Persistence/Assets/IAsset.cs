namespace EphemereGames.Core.Persistence
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;


    public interface IAsset
    {
        string AssetType { get; }

        object Load(string name, string path, Dictionary<string, string> parameters, ContentManager manager);

        object Clone();
    }
}
