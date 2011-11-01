namespace EphemereGames.Core.SimplePersistence
{
    using System.IO;
    using System.Xml.Serialization;


    public abstract class SimpleData
    {
        [XmlIgnore]
        public string Name      { get; set; }

        [XmlIgnore]
        public string Directory { get; set; }

        [XmlIgnore]
        public string File      { get; set; }

        [XmlIgnore]
        public bool Loaded      { get; set; }


        private XmlSerializer Serializer;


        public SimpleData()
        {
            Name = "Data";
            Directory = "Data";
            File = "Data.xml";
            Loaded = false;

            Serializer = new XmlSerializer(GetType());
        }


        internal void SaveData()
        {
            DoSaveStarted();

            bool success = true;

            try
            {
                using (StreamWriter writer = new StreamWriter(Directory + @"\" + File))
                    Serializer.Serialize(writer.BaseStream, this);
            }

            catch
            {
                success = false;
            }

            if (success)
                DoSaveEnded();
            else
                DoSaveFailed();
        }


        internal virtual void LoadData()
        {
            DoLoadStarted();

            object data = null;

            try
            {
                using (StreamReader reader = new StreamReader(Directory + @"\" + File))
                    data = Serializer.Deserialize(reader);

                if (data != null)
                    DoInitialize(data);
            }

            catch
            {
                data = null; //initialization failed
            }

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
