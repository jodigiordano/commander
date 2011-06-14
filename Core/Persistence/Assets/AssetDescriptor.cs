namespace EphemereGames.Core.Persistence
{
    using System.Collections.Generic;
    
    class AssetDescriptor
    {
        public string Name;
        public string Type;
        public string Path;
        public Dictionary<string, string> Parameters;
    }
}
