namespace Core.Persistance
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public abstract class AbstractDonnee
    {
        [XmlIgnore]
        public string Nom                       { get; set; }

        [XmlIgnore]
        public string NomDossier                { get; set; }

        [XmlIgnore]
        public string NomFichier                { get; set; }

        [XmlIgnore]
        public bool Charge                      { get; set; }
        
#if XBOX
        protected SaveDevice PeripheriqueStockage { get; set; }
#else
        protected PCSaveDevice PeripheriqueStockage { get; set; }
#endif

        public AbstractDonnee()
        {
            Nom = "donnees";
            NomDossier = "MyGame";
            NomFichier = "data.xml";
            Charge = false;
        }

        public void Initialize()
        {
            if (PeripheriqueStockage != null)
            {
#if XBOX
                PeripheriqueStockage.Dispose();
#endif
                PeripheriqueStockage = null;
                Charge = false;
            }
        }


        public virtual void sauvegarder()
        {
            doSauvegardeDebute();

            bool sauvegardeReussie = true;

            try
            {
                PeripheriqueStockage.Save(NomFichier, serialiser);
            }

            catch (InvalidOperationException)
            {
                doSauvegardeErreur();
                sauvegardeReussie = false;
            }

            if (sauvegardeReussie)
                doSauvegardeTermine();
        }


        public virtual void charger()
        {
#if XBOX
            PeripheriqueStockage.PromptForDevice();
#else
            saveDevice_DeviceSelected(null, null);
#endif
        }


        private void serialiser(StreamWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(writer.BaseStream, this);
        }

        private void deserialiser(StreamReader reader)
        {
            object donnee = null;

            // désérialisation XML simple (on try car le fichier de sauvegarde peut être corrompu)
            try
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());

                donnee = serializer.Deserialize(reader.BaseStream);

                if (donnee != null)
                    this.doInitialiser(donnee);
            }

            catch (Exception e) {}

            if (donnee == null)
            {
                Charge = false;
                doChargementErreur();
            }

            else
            {
                Charge = true;
                doChargementTermine();
            }
        }

        protected abstract void doInitialiser(object donnee);

        protected virtual void doSauvegardeDebute() { }
        protected virtual void doSauvegardeTermine() { }
        protected virtual void doChargementDebute() { }
        protected virtual void doChargementTermine() { }
        protected virtual void doChargementErreur() { }
        protected virtual void doSauvegardeErreur() { }
        public virtual void doFichierInexistant() { }
        public virtual void doBoiteDialogueReselectionPeripheriqueFermee() { }

        protected void saveDevice_DeviceReselectPromptClosed(object sender, SaveDevicePromptEventArgs e)
        {
            if (!e.ShowDeviceSelector)
                doBoiteDialogueReselectionPeripheriqueFermee();
        }

        protected void saveDevice_DeviceSelected(object sender, EventArgs e)
        {
            // si les Donnees avaient déjà été chargées, c'est que le storage device a été retiré
            // on sauvegarde sur le nouveau SD sélectionné
            if (Charge)
                GestionnaireDonnees.Instance.sauvegarder(Nom);
            else
            {
                Charge = false;

                doChargementDebute();

                if (!PeripheriqueStockage.FileExists(NomFichier))
                    doFichierInexistant();
                else
                    PeripheriqueStockage.Load(NomFichier, deserialiser);
            }
        }

        public void Update(GameTime gameTime)
        {
#if XBOX
            if (PeripheriqueStockage != null)
                PeripheriqueStockage.Update(gameTime);
#endif
        }
    }
}
