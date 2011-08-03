namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    class Translator : IVisual
    {
        private string TextToTranslate;
        public Text ToTranslate;
        public Text Translated;
        private char[] PartNotTranslated;
        private char[] PartTranslated;
        private char[] AlienVersion;
        private double[] TimeTranslateALetter;
        private double TimeEachTranslation;
        private double[] TimeTranslationElapsed;
        private bool ShowTranslation;
        private Scene Scene;
        private Vector3 Position;
        private double TotalTimeTranslation;
        private double TotalTimeElapsed;

        public bool CenterText;


        public Translator(Scene scene, Vector3 position, string alienFontName, Color alienColor, string knownFont, Color knownColor, string text, float size, bool showTranslation, int timeTranslation, int timeBetweenTwoTranslation, double visualPriority, bool visible)
        {
            Scene = scene;
            Position = position;
            ShowTranslation = showTranslation;
            TotalTimeTranslation = timeTranslation;
            TotalTimeElapsed = 0;
            TimeEachTranslation = timeBetweenTwoTranslation;
            CenterText = false;
            TextToTranslate = text;

            ToTranslate = new Text(text, alienFontName, alienColor, position)
            {
                SizeX = size,
                VisualPriority = visualPriority,
                Alpha = visible ? alienColor.A : (byte) 0
            };


            Translated = new Text("", knownFont, knownColor, position)
            {
                SizeX = size,
                VisualPriority = visualPriority,
                Alpha = visible ? knownColor.A : (byte) 0
            };



            PartNotTranslated = new char[text.Length];
            PartTranslated = new char[text.Length];
            TimeTranslateALetter = new double[text.Length];
            TimeTranslationElapsed = new double[text.Length];
            AlienVersion = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    PartTranslated[i] = PartNotTranslated[i] = text[i];
                    TimeTranslateALetter[i] = 0;
                    TimeTranslationElapsed[i] = 0;
                }

                else
                {
                    PartTranslated[i] = PartNotTranslated[i] = AlienVersion[i] = (char)Main.Random.Next(48, 100);
                    TimeTranslateALetter[i] = Main.Random.Next(0, timeTranslation);
                    TimeTranslationElapsed[i] = Main.Random.Next(0, timeBetweenTwoTranslation);
                }
            }

        }


        public bool Finished
        {
            get { return TotalTimeTranslation < TotalTimeElapsed; }
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public byte Alpha
        {
            get
            {
                return ToTranslate.Alpha;
            }
            set
            {
                ToTranslate.Alpha = value;
                Translated.Alpha = value;
            }
        }


        public void Update()
        {
            TotalTimeElapsed += Preferences.TargetElapsedTimeMs;

            for (int i = 0; i < TextToTranslate.Length; i++)
                TimeTranslationElapsed[i] -= Preferences.TargetElapsedTimeMs;
        }


        public void Draw()
        {
            for (int i = 0; i < TextToTranslate.Length; i++)
            {
                if (TimeTranslateALetter[i] < TotalTimeElapsed)
                {
                    PartTranslated[i] = (char)TextToTranslate[i];
                    PartNotTranslated[i] = ' ';
                }
                
                else if (ShowTranslation && TimeTranslationElapsed[i] <= 0)
                {
                    PartTranslated[i] = ' ';
                    PartNotTranslated[i] = AlienVersion[i] = (char)Main.Random.Next(48, 100);
                    TimeTranslationElapsed[i] = TimeEachTranslation;
                }

                else
                {
                    PartTranslated[i] = ' ';
                    PartNotTranslated[i] = AlienVersion[i];
                }
            }

            Translated.Data = new string(PartTranslated);
            ToTranslate.Data = new string(PartNotTranslated);

            if (CenterText)
            {
                Translated.Origin = Translated.Center;
                ToTranslate.Origin = ToTranslate.Center;
            }

            else
            {
                Translated.Origin = Vector2.Zero;
                ToTranslate.Origin = Vector2.Zero;
            }

            Scene.Add(Translated);
            Scene.Add(ToTranslate);
        }
    }
}
