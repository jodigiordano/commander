namespace EphemereGames.Commander
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistance;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content;


    [Serializable]
    public class GenerateurData : Data
    {
        [ContentSerializer(Optional = false)]
        public List<DescripteurScenario> Scenarios { get; set; }


        public GenerateurData()
        {
            Name = "generateurData";
            Folder = "Commander";
            File = "Generator.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            GenerateurData d = donnee as GenerateurData;

            this.Scenarios = d.Scenarios;
        }


        public override void DoFileNotFound()
        {
            base.DoFileNotFound();
            premierChargement();
        }


        protected override void DoLoadFailed()
        {
            base.DoLoadFailed();
            premierChargement();
        }


        private void premierChargement()
        {
            this.Scenarios = new List<DescripteurScenario>();

            EphemereGames.Core.Persistance.Facade.SaveData(this.Name);
            Loaded = true;
        }
    }
}
