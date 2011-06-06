namespace EphemereGames.Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;


    public interface IContenu : ICloneable
    {
        String AssetType { get; }

        object Load(String name, String path, Dictionary<String, String> parameters, ContentManager manager);
    }
}
