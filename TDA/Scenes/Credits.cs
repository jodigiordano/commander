namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Input;
    using RainingSundays.Core.Visuel;
    using RainingSundays.Core.Utilities;

    class Credits : SceneMenu
    {
        private Main Main;
        private IVisible Titre;
        private IVisible TitreRemerciements;
        private IVisible Filtre;
        private List<KeyValuePair<IVisible, IVisible>> TitrePersonne;
        private List<IVisible> Remerciements;
        private IVisible Retour;


        public Credits(Main main)
            : base(Vector2.Zero, "Menu")
        {
            Main = main;

            Nom = "Credits";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;
            Position += new Vector3(0, 0, 1);

            Titre = new IVisible("Credits", RainingSundays.Core.Persistance.Facade.recuperer<SpriteFont>("DataControlBold"), Color.White, new Vector3(0, -270, 0), this);
            Titre.Origine = Titre.Centre;

            Filtre = new IVisible(RainingSundays.Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero, this);
            Filtre.PrioriteAffichage = 0.85f;
            Filtre.TailleVecteur = new Vector2(600, 400);
            Filtre.Couleur = new Color(Color.Black, 200);
            Filtre.Origine = Filtre.Centre;

            TitrePersonne = new List<KeyValuePair<IVisible, IVisible>>();

            SpriteFont police = RainingSundays.Core.Persistance.Facade.recuperer<SpriteFont>("DataControlMini");

            TitrePersonne.Add(new KeyValuePair<IVisible, IVisible>(
                new IVisible("Game designer & programmer", police, Color.White, new Vector3(-270, -100, 0), this),
                new IVisible("Jodi Giordano", police, Color.White, new Vector3(100, -100, 0), this)));

            TitrePersonne.Add(new KeyValuePair<IVisible,IVisible>(
                new IVisible("Sound designer", police, Color.White, new Vector3(-270, -180, 0), this),
                new IVisible("Olivier Robert", police, Color.White, new Vector3(100, -180, 0), this)));

            TitrePersonne.Add(new KeyValuePair<IVisible, IVisible>(
                new IVisible("Visual designer", police, Color.White, new Vector3(-270, -140, 0), this),
                new IVisible("Bruno Durand", police, Color.White, new Vector3(100, -140, 0), this)));

            TitrePersonne.Add(new KeyValuePair<IVisible, IVisible>(
                new IVisible("Core engine designers", police, Color.White, new Vector3(-270, -60, 0), this),
                new IVisible("Jodi Giordano", police, Color.White, new Vector3(100, -60, 0), this)));

            TitrePersonne.Add(new KeyValuePair<IVisible, IVisible>(
                new IVisible("", police, Color.White, new Vector3(-270, -40, 0), this),
                new IVisible("Julien Theron", police, Color.White, new Vector3(100, -40, 0), this)));

            TitreRemerciements = new IVisible("Thanks to:", police, Color.White, new Vector3(-270, 40, 0), this);

            Remerciements = new List<IVisible>();
            Remerciements.Add(new IVisible("- Francois Pomerleau for his idea of a constantly moving path", police, Color.White, new Vector3(-270, 80, 0), this));
            Remerciements.Add(new IVisible("- The beautiful Farseer Physics Engine (farseerphysics.codeplex.com)", police, Color.White, new Vector3(-270, 100, 0), this));
            Remerciements.Add(new IVisible("- The crazy Mercury Particle Engine (mpe.codeplex.com)", police, Color.White, new Vector3(-270, 120, 0), this));
            Remerciements.Add(new IVisible("- The useful XNA Console (xnaconsole.codeplex.com)", police, Color.White, new Vector3(-270, 140, 0), this));

            Retour = new IVisible("Press B to return to the menu", police, Color.White, new Vector3(0, 200, 0), this);
            Retour.Origine = Retour.Centre;
            Retour.PrioriteAffichage = 0.1f;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (RainingSundays.Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Main.JoueursConnectes[0].Manette, "Credits"))
            {
                RainingSundays.Core.Visuel.Facade.effectuerTransition("CreditsVersMenu");
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable(Titre);
            ajouterScenable(Filtre);
            ajouterScenable(TitreRemerciements);
            ajouterScenable(Retour);

            foreach (var kvp in TitrePersonne)
            {
                ajouterScenable(kvp.Key);
                ajouterScenable(kvp.Value);
            }

            foreach (var remerciement in Remerciements)
                ajouterScenable(remerciement);
        }
    }
}
