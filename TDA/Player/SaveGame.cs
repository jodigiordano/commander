namespace TDA
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Core.Persistance;
    using Core.Utilities;
    using Microsoft.Xna.Framework.Content;

    [Serializable]
    public class SaveGame : AbstractDonneeJoueur
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
            Nom = "savePlayer";
            NomDossier = "Commander";
            NomFichier = "Save.xml";
        }


        protected override void doInitialiser(object donnee)
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


        public override void doFichierInexistant()
        {
            base.doFichierInexistant();
            premierChargement();
        }


        protected override void doChargementErreur()
        {
            base.doChargementErreur();
            premierChargement();
        }


        protected override void doSauvegardeDebute()
        {
            base.doSauvegardeDebute();

            this.HighScores.InitializeToSave();
            this.Progress.InitializeToSave();
            this.Tutorials.InitializeToSave();
        }


        protected override void doChargementTermine()
        {
            base.doChargementTermine();

            Core.Audio.Facade.VolumeMusique = this.VolumeMusic / 10f;
            Core.Audio.Facade.VolumeEffetsSonores = this.VolumeSfx / 10f;
        }


        private void premierChargement()
        {
            this.VolumeMusic = 5;
            this.VolumeSfx = 7;
            this.ProductKey = "";

            this.HighScores = new SerializableDictionaryProxy<int, HighScores>();
            this.Progress = new SerializableDictionaryProxy<int, int>();
            this.Tutorials = new SerializableDictionaryProxy<int, int>();

            Core.Audio.Facade.VolumeMusique = this.VolumeMusic / 10f;
            Core.Audio.Facade.VolumeEffetsSonores = this.VolumeSfx / 10f;

            Core.Persistance.Facade.sauvegarderDonnee(this.Nom);
            Charge = true;
        }
    }
}
