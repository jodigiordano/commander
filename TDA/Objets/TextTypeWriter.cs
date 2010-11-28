namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Core.Utilities;

    class TextTypeWriter : DrawableGameComponent
    {
        private String Phrase;
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
        private int NbCaracteresTraitesMot;
        private List<String> Mots;
        private double Compteur;
        private bool JouerSons;
        private List<String> Sons;

        public IVisible Texte;

        public TextTypeWriter(Main main, String phrase, Color couleur, Vector3 positionDepart, SpriteFont police, float taille, Vector2 canevas, double cadence, bool arretFinPhrase, double tempsArret, bool jouerSons, List<String> sons, Scene scene)
            : base(main)
        {
            this.Main = main;
            this.Phrase = phrase;
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

            IVisible iv = new IVisible("a", police, Color.White, Vector3.Zero, scene);
            iv.Taille = this.Taille;
            LongueurCaracterePixel = iv.Rectangle.Width;

            NbCaracteresLigne = (int) (canevas.X / LongueurCaracterePixel);

            Mots = new List<string>(Phrase.Split(' '));

            Phrase = "";

            NbCaracteresTraitesMot = 0;

            for (int i = 0; i < Mots.Count; i++)
            {
                if (Mots[i].Length > NbCaracteresLigne - NbCaracteresTraitesLigne)
                {
                    Phrase += "\n\n";
                    NbCaracteresTraitesLigne = 0;
                }

                Phrase += Mots[i] + " ";

                NbCaracteresTraitesLigne += Mots[i].Length + 1;
            }


            Compteur = 0;

            Texte = new IVisible("", police, couleur, PositionDepart, scene);
            Texte.Taille = this.Taille;
        }


        public bool Termine
        {
            get { return NbCaracteresTraites >= Phrase.Length; }
        }


        public override void Update(GameTime gameTime)
        {
            Compteur -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Compteur <= 0 && NbCaracteresTraites < Phrase.Length)
            {
                Compteur = (ArretFinPhrase && Phrase[NbCaracteresTraites] == '.') ? this.TempsArret : Cadence;

                Texte.Texte += Phrase[NbCaracteresTraites];

                NbCaracteresTraites++;

                if (JouerSons)
                    Core.Audio.Facade.jouerEffetSonore("Partie", Sons[Main.Random.Next(0, Sons.Count)]);
            }
        }
    }
}
