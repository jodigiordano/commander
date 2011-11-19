namespace EphemereGames.Core.SimplePersistence
{
    using System.IO;
    using System.Xml.Serialization;


    public abstract class SimpleData
    {
        [XmlIgnore]
        public string Directory { get; set; }

        [XmlIgnore]
        public string File      { get; set; }

        [XmlIgnore]
        public bool Loaded      { get; set; }


        internal string StringSource;
        internal byte[] ByteSource;


        private XmlSerializer Serializer;


        public SimpleData()
        {
            Directory = "Data";
            File = "Data.xml";
            Loaded = false;

            Serializer = new XmlSerializer(GetType());
        }


        #region Save

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


        internal void SaveFromString()
        {
            ByteSource = new System.Text.UTF8Encoding().GetBytes(StringSource);
            SaveFromStream();
        }


        internal void SaveFromStream()
        {
            DoSaveStarted();

            bool success = true;

            object output = null;

            try
            {
                var ms = new MemoryStream(ByteSource);

                using (StreamReader reader = new StreamReader(ms, true))
                    output = Serializer.Deserialize(reader);

                if (output != null)
                    DoInitialize(output);

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

            ByteSource = null;
            StringSource = null;
        }

        #endregion


        #region Load

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


        internal void LoadFromString()
        {
            ByteSource = new System.Text.UnicodeEncoding().GetBytes(StringSource);
            LoadFromStream();
        }


        internal void LoadFromStream()
        {
            DoLoadStarted();

            object output = null;

            try
            {
                var ms = new MemoryStream(ByteSource);

                using (StreamReader reader = new StreamReader(ms, true))
                    output = Serializer.Deserialize(reader);

                if (output != null)
                    DoInitialize(output);
            }

            catch
            {
                output = null; //initialization failed
            }

            if (output == null)
            {
                Loaded = false;
                DoLoadFailed();
            }

            else
            {
                Loaded = true;
                DoLoadEnded();
            }

            ByteSource = null;
            StringSource = null;
        }

        #endregion


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
