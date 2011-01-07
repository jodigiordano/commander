namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;

    class TextTypeWriter : DrawableGameComponent
    {
        private String Raw;
        private double Cadence;
        private bool ArretFinPhrase;
        private Vector2 Canevas;
        private SpriteFont Police;
        private Scene Scene;
        private Main Main;
        private Vector3 PositionDepart;
        private float Taille;
        private double TempsArret;

        private int LongueurCaracterePixel;
        private int NbCaracteresLigne;
        private int NbCaracteresTraites;
        private int NbCaracteresTraitesLigne;
        private double Compteur;
        private bool JouerSons;
        private List<String> Sons;

        public IVisible Texte;

        public TextTypeWriter(Main main, String raw, Color couleur, Vector3 positionDepart, SpriteFont police, float taille, Vector2 canevas, double cadence, bool arretFinPhrase, double tempsArret, bool jouerSons, List<String> sons, Scene scene)
            : base(main)
        {
            this.Main = main;
            this.Police = police;
            this.Canevas = canevas;
            this.Cadence = cadence;
            this.ArretFinPhrase = arretFinPhrase;
            this.Scene = scene;
            this.PositionDepart = positionDepart;
            this.TempsArret = tempsArret;
            this.Taille = taille;
            this.JouerSons = jouerSons;
            this.Sons = sons;

            IVisible iv = new IVisible("a", police, Color.White, Vector3.Zero);
            iv.Taille = this.Taille;
            LongueurCaracterePixel = iv.Rectangle.Width;

            NbCaracteresLigne = (int) (canevas.X / LongueurCaracterePixel);

            Raw = "";

            string[] sentences = raw.Split(new string[] { "\n\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sentence in sentences)
            {
                string[] words = sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                NbCaracteresTraitesLigne = 0;

                foreach (var word in words)
                {
                    if (word.Length > NbCaracteresLigne - NbCaracteresTraitesLigne)
                    {
                        Raw += "\n\n";
                        NbCaracteresTraitesLigne = 0;
                    }

                    Raw += word + " ";
                    NbCaracteresTraitesLigne += word.Length + 1;
                }

                Raw += "\n\n\n";
            }


            Compteur = 0;

            Texte = new IVisible("", police, couleur, PositionDepart);
            Texte.Taille = this.Taille;
        }


        public bool Termine
        {
            get { return NbCaracteresTraites >= Raw.Length; }
        }


        public override void Update(GameTime gameTime)
        {
            Compteur -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Compteur <= 0 && NbCaracteresTraites < Raw.Length)
            {
                Compteur = (ArretFinPhrase && Raw[NbCaracteresTraites] == '.') ? this.TempsArret : Cadence;

                Texte.Texte += Raw[NbCaracteresTraites];

                NbCaracteresTraites++;

                if (JouerSons)
                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Sons[Main.Random.Next(0, Sons.Count)]);
            }
        }
    }
}
