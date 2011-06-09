namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;

    class Translator
    {
        private String TexteATraduire;
        public IVisible PartieNonTraduite;
        public IVisible PartieTraduite;
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
        private double TempsTraductionEcoule;

        public bool Centre;

        public bool Termine
        {
            get { return TempsTraduction < TempsTraductionEcoule; }
        }

        public Translator(
            Main main,
            Scene scene,
            Vector3 position,
            SpriteFont policeLangueEtrangere,
            Color couleurLangueEtrangere,
            SpriteFont policeLangueConnue,
            Color couleurLangueConnue,
            String texteATraduire,
            float taille,
            bool showRecherche,
            int tempsTraduction,
            int tempsChaqueRecherche,
            double visualPriority)
        {
            this.Scene = scene;
            this.Position = position;
            this.ShowRecherche = showRecherche;
            this.TempsTraduction = tempsTraduction;
            this.TempsTraductionEcoule = 0;
            this.TempsChaqueRecherche = tempsChaqueRecherche;
            this.Centre = false;
            this.TexteATraduire = texteATraduire;

            PartieNonTraduite = new IVisible(texteATraduire, policeLangueEtrangere, couleurLangueEtrangere, position);
            PartieNonTraduite.Taille = taille;
            PartieNonTraduite.VisualPriority = visualPriority;
            PartieTraduite = new IVisible("", policeLangueConnue, couleurLangueConnue, position);
            PartieTraduite.Taille = taille;
            PartieTraduite.VisualPriority = visualPriority;

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
            TempsTraductionEcoule += 16.66;

            for (int i = 0; i < TexteATraduire.Length; i++)
                ProgressionUneRecherche[i] -= 16.66;
        }
        }


        public void Draw()
        {
            for (int i = 0; i < TexteATraduire.Length; i++)
            {
                if (TempsLettreTraduction[i] < TempsTraductionEcoule)
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

            PartieTraduite.Texte = new string(PartieTraduiteTexte);
            PartieNonTraduite.Texte = new string(PartieNonTraduiteTexte);

            if (Centre)
            {
                PartieTraduite.Origine = PartieTraduite.Centre;
                PartieNonTraduite.origine = PartieNonTraduite.Centre;
            }

            else
            {
                PartieTraduite.Origine = Vector2.Zero;
                PartieNonTraduite.origine = Vector2.Zero;
            }

            Scene.ajouterScenable(PartieTraduite);
            Scene.ajouterScenable(PartieNonTraduite);
        }
    }
}
