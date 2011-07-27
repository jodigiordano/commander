namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    class Translator
    {
        private string TexteATraduire;
        public Text ToTranslate;
        public Text Translated;
        private char[] PartieNonTraduiteTexte;
        private char[] PartieTraduiteTexte;
        private char[] VersionAlien;
        private double[] TempsLettreTraduction;
        private double TempsChaqueRecherche;
        private double[] ProgressionUneRecherche;
        private bool ShowTranslation;
        private Scene Scene;
        private Vector3 Position;
        private double TempsTraduction;
        private double Elapsed;

        public bool CenterText;

        public bool Termine
        {
            get { return TempsTraduction < Elapsed; }
        }


        public Translator(Scene scene, Vector3 position, string alienFontName, Color alienColor, string knownFont, Color knownColor, string text, float size, bool showTranslation, int timeTranslation, int timeBetweenTwoTranslation, double visualPriority, bool visible)
        {
            Scene = scene;
            Position = position;
            ShowTranslation = showTranslation;
            TempsTraduction = timeTranslation;
            Elapsed = 0;
            TempsChaqueRecherche = timeBetweenTwoTranslation;
            CenterText = false;
            TexteATraduire = text;

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



            PartieNonTraduiteTexte = new char[text.Length];
            PartieTraduiteTexte = new char[text.Length];
            TempsLettreTraduction = new double[text.Length];
            ProgressionUneRecherche = new double[text.Length];
            VersionAlien = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    PartieTraduiteTexte[i] = PartieNonTraduiteTexte[i] = text[i];
                    TempsLettreTraduction[i] = 0;
                    ProgressionUneRecherche[i] = 0;
                }

                else
                {
                    PartieTraduiteTexte[i] = PartieNonTraduiteTexte[i] = VersionAlien[i] = (char)Main.Random.Next(48, 100);
                    TempsLettreTraduction[i] = Main.Random.Next(0, timeTranslation);
                    ProgressionUneRecherche[i] = Main.Random.Next(0, timeBetweenTwoTranslation);
                }
            }

        }


        public void Update()
        {
            Elapsed += Preferences.TargetElapsedTimeMs;

            for (int i = 0; i < TexteATraduire.Length; i++)
                ProgressionUneRecherche[i] -= Preferences.TargetElapsedTimeMs;
        }


        public void Draw()
        {
            for (int i = 0; i < TexteATraduire.Length; i++)
            {
                if (TempsLettreTraduction[i] < Elapsed)
                {
                    PartieTraduiteTexte[i] = (char)TexteATraduire[i];
                    PartieNonTraduiteTexte[i] = ' ';
                }
                
                else if (ShowTranslation && ProgressionUneRecherche[i] <= 0)
                {
                    PartieTraduiteTexte[i] = ' ';
                    PartieNonTraduiteTexte[i] = VersionAlien[i] = (char)Main.Random.Next(48, 100);
                    ProgressionUneRecherche[i] = TempsChaqueRecherche;
                }

                else
                {
                    PartieTraduiteTexte[i] = ' ';
                    PartieNonTraduiteTexte[i] = VersionAlien[i];
                }
            }

            Translated.Data = new string(PartieTraduiteTexte);
            ToTranslate.Data = new string(PartieNonTraduiteTexte);

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
