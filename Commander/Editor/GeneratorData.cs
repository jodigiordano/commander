namespace EphemereGames.Commander
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content;


    public class GeneratorData : Data
    {
        [ContentSerializer(Optional = false)]
        public List<ScenarioDescriptor> Scenarios { get; set; }


        public GeneratorData()
        {
            Name = "generateurData";
            Folder = "Commander";
            File = "Generator.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            GeneratorData d = donnee as GeneratorData;

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
            this.Scenarios = new List<ScenarioDescriptor>();

            EphemereGames.Core.Persistence.Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
