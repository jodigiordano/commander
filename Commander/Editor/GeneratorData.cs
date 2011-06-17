namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework.Content;


    public class GeneratorData : Data
    {
        [ContentSerializer(Optional = false)]
        public List<LevelDescriptor> Levels { get; set; }


        public GeneratorData()
        {
            Name = "generateurData";
            Folder = "Commander";
            File = "Generator.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            GeneratorData d = donnee as GeneratorData;

            this.Levels = d.Levels;
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
            this.Levels = new List<LevelDescriptor>();

            Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
