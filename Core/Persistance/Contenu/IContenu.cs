namespace Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;

    public interface IContenu : ICloneable
    {
        String TypeAsset { get; }
        object charger(String nom, String chemin, Dictionary<String, String> parametres, ContentManager contenu);
        //void decharger(String nom);
    }
}
