namespace EphemereGames.Core.Persistance
{
    using System;
    using System.Collections.Generic;
    
    class DescriptionContenu
    {
        public string Nom { get; set; }
        public string Type { get; set; }
        public string Chemin { get; set; }
        public Dictionary<string, string> Parametres { get; set; }
    }
}
