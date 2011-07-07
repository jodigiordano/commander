﻿namespace EphemereGames.Core.Persistence
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using EasyStorage;


    public abstract class Data
    {
        [XmlIgnore]
        public string Name      { get; set; }

        [XmlIgnore]
        public string Folder    { get; set; }

        [XmlIgnore]
        public string File      { get; set; }

        [XmlIgnore]
        public bool Loaded      { get; set; }

        [XmlIgnore]
#if WINDOWS_PHONE
        public IsolatedStorageSaveDevice SaveDevice { get; set; }
#else
        public SharedSaveDevice SaveDevice { get; set; }
#endif

        public Data()
        {
            Name = "MyData";
            Folder = "MyGame";
            File = "MyData.xml";
            Loaded = false;
        }


        public void Save()
        {
            DoSaveStarted();

            while (!SaveDevice.IsReady) { }

            bool sauvegardeReussie = true;

            try
            {
                SaveDevice.Save(Folder, File, new FileAction(Serialize));
            }

            catch (InvalidOperationException)
            {
                DoSaveFailed();
                sauvegardeReussie = false;
            }

            if (sauvegardeReussie)
                DoSaveEnded();
        }


        public virtual void Load()
        {
            DoLoadStarted();

            while (!SaveDevice.IsReady) { }

            if (!SaveDevice.FileExists(Folder, File))
                DoFileNotFound();
            else
                SaveDevice.Load(Folder, File, new FileAction(Deserialize));
        }


        private void Serialize(Stream stream)
        {
//TODO!
#if !WINDOWS_PHONE
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stream, this);
#endif
        }


        private void Deserialize(Stream stream)
        {
//TODO!
#if !WINDOWS_PHONE
            object data = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());

                data = serializer.Deserialize(stream);

                if (data != null)
                    this.DoInitialize(data);
            }

            catch {}

            if (data == null)
            {
                Loaded = false;
                DoLoadFailed();
            }

            else
            {
                Loaded = true;
                DoLoadEnded();
            }
#endif
        }


        protected abstract void DoInitialize(object data);
        protected virtual void DoSaveStarted() { }
        protected virtual void DoSaveEnded() { }
        protected virtual void DoLoadStarted() { }
        protected virtual void DoLoadEnded() { }
        protected virtual void DoLoadFailed() { }
        protected virtual void DoSaveFailed() { }
        public virtual void DoFileNotFound() { }
    }
}
