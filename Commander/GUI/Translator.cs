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
        private bool ShowRecherche;
        private Scene Scene;
        private Vector3 Position;
        private double TempsTraduction;
        private double Elapsed;

        public bool Centre;

        public bool Termine
        {
            get { return TempsTraduction < Elapsed; }
        }


        public Translator(Scene scene, Vector3 position, string policeLangueEtrangere, Color couleurLangueEtrangere, string policeLangueConnue, Color couleurLangueConnue, string texteATraduire, float taille, bool showRecherche, int tempsTraduction, int tempsChaqueRecherche, double visualPriority)
        {
            this.Scene = scene;
            this.Position = position;
            this.ShowRecherche = showRecherche;
            this.TempsTraduction = tempsTraduction;
            this.Elapsed = 0;
            this.TempsChaqueRecherche = tempsChaqueRecherche;
            this.Centre = false;
            this.TexteATraduire = texteATraduire;

            ToTranslate = new Text(texteATraduire, policeLangueEtrangere, couleurLangueEtrangere, position);
            ToTranslate.SizeX = taille;
            ToTranslate.VisualPriority = visualPriority;
            Translated = new Text("", policeLangueConnue, couleurLangueConnue, position);
            Translated.SizeX = taille;
            Translated.VisualPriority = visualPriority;

            PartieNonTraduiteTexte = new char[texteATraduire.Length];
            PartieTraduiteTexte = new char[texteATraduire.Length];
            TempsLettreTraduction = new double[texteATraduire.Length];
            ProgressionUneRecherche = new double[texteATraduire.Length];
            VersionAlien = new char[texteATraduire.Length];

            for (int i = 0; i < texteATraduire.Length; i++)
            {
                if (texteATraduire[i] == '\n')
                {
                    PartieTraduiteTexte[i] = PartieNonTraduiteTexte[i] = texteATraduire[i];
                    TempsLettreTraduction[i] = 0;
                    ProgressionUneRecherche[i] = 0;
                }

                else
                {
                    PartieTraduiteTexte[i] = PartieNonTraduiteTexte[i] = VersionAlien[i] = (char)Main.Random.Next(48, 100);
                    TempsLettreTraduction[i] = Main.Random.Next(0, tempsTraduction);
                    ProgressionUneRecherche[i] = Main.Random.Next(0, tempsChaqueRecherche);
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
                
                else if (ShowRecherche && ProgressionUneRecherche[i] <= 0)
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

            if (Centre)
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
