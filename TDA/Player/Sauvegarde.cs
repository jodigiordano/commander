namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Persistance;
    using Microsoft.Xna.Framework.Content;

    [Serializable]
    public class SaveGame : AbstractDonneeJoueur
    {
        [ContentSerializer(Optional = false)]
        public List<int> Progression               { get; set; }

        [ContentSerializer(Optional = false)]
        public int VolumeMusique                    { get; set; }

        [ContentSerializer(Optional = false)]
        public int VolumeEffetsSonores              { get; set; }

        [ContentSerializer(Optional = false)]
        public string ProductKey { get; set; }

        [ContentSerializer(Optional = false)]
        public List<DescripteurScenario> DescriptionsScenarios { get; set; }

        [ContentSerializer(Optional = false)]
        public List<DonneesGenerateur> DonneesGenerateur { get; set; }

        public SaveGame()
        {
            Nom = "savePlayer";
            NomDossier = "Commander";
            NomFichier = "Save.xml";
        }


        protected override void doInitialiser(object donnee)
        {
            SaveGame d = donnee as SaveGame;

            this.Progression = d.Progression;
            this.VolumeMusique = d.VolumeMusique;
            this.VolumeEffetsSonores = d.VolumeEffetsSonores;
            this.DescriptionsScenarios = d.DescriptionsScenarios;
            this.DonneesGenerateur = d.DonneesGenerateur;
            this.ProductKey = d.ProductKey;
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


        protected override void doChargementTermine()
        {
            base.doChargementTermine();

            Core.Audio.Facade.VolumeMusique = this.VolumeMusique / 10f;
            Core.Audio.Facade.VolumeEffetsSonores = this.VolumeEffetsSonores / 10f;
        }


        private void premierChargement()
        {
            this.VolumeMusique = 5;
            this.VolumeEffetsSonores = 7;
            this.ProductKey = "";

            this.Progression = new List<int>();

            for (int i = 0; i < 200; i++)
                this.Progression.Add(0);

            Core.Audio.Facade.VolumeMusique = this.VolumeMusique / 10f;
            Core.Audio.Facade.VolumeEffetsSonores = this.VolumeEffetsSonores / 10f;

            DescriptionsScenarios = new List<DescripteurScenario>();
            DonneesGenerateur = new List<DonneesGenerateur>();

            Core.Persistance.Facade.sauvegarderDonnee(this.Nom);
            Charge = true;
        }
    }
}
