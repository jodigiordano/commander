namespace EphemereGames.Commander
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content;


    public class SaveGame : Data
    {
        [ContentSerializer(Optional = false)]
        public SerializableDictionaryProxy<int, int> Progress { get; set; }

        [ContentSerializer(Optional = false)]
        public int VolumeMusic                    { get; set; }

        [ContentSerializer(Optional = false)]
        public int VolumeSfx              { get; set; }

        [ContentSerializer(Optional = false)]
        public SerializableDictionaryProxy<int, HighScores> HighScores { get; set; }

        [ContentSerializer(Optional = false)]
        public string ProductKey { get; set; }

        [ContentSerializer(Optional = false)]
        public SerializableDictionaryProxy<int, int> Tutorials { get; set; }


        public SaveGame()
        {
            Name = "savePlayer";
            Folder = "Commander";
            File = "Save.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            SaveGame d = donnee as SaveGame;

            this.Progress = d.Progress;
            this.VolumeMusic = d.VolumeMusic;
            this.VolumeSfx = d.VolumeSfx;
            this.ProductKey = d.ProductKey;
            this.HighScores = d.HighScores;
            this.Tutorials = d.Tutorials;
            this.HighScores.Initialize();
            this.Progress.Initialize();
            this.Tutorials.Initialize();
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


        protected override void DoSaveStarted()
        {
            base.DoSaveStarted();

            this.HighScores.InitializeToSave();
            this.Progress.InitializeToSave();
            this.Tutorials.InitializeToSave();
        }


        protected override void DoLoadEnded()
        {
            base.DoLoadEnded();

            EphemereGames.Core.Audio.Audio.VolumeMusique = this.VolumeMusic / 10f;
            EphemereGames.Core.Audio.Audio.VolumeEffetsSonores = this.VolumeSfx / 10f;
        }


        private void premierChargement()
        {
            this.VolumeMusic = 5;
            this.VolumeSfx = 7;
            this.ProductKey = "";

            this.HighScores = new SerializableDictionaryProxy<int, HighScores>();
            this.Progress = new SerializableDictionaryProxy<int, int>();
            this.Tutorials = new SerializableDictionaryProxy<int, int>();

            EphemereGames.Core.Audio.Audio.VolumeMusique = this.VolumeMusic / 10f;
            EphemereGames.Core.Audio.Audio.VolumeEffetsSonores = this.VolumeSfx / 10f;

            EphemereGames.Core.Persistence.Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
