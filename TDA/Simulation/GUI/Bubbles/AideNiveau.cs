namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;

    class AideNiveau : GameComponent
    {
        private int Niveau;
        public List<string> TypesObjets;
        public List<string> Quotes;
        public Dictionary<string, IObjetPhysique> QuotesObjets;

        public bool PeutLancerNouvelleQuote
        {
            get
            {
                return (TempsProchaineQuote < 0 && !Termine);
            }
        }

        public KeyValuePair<IObjetPhysique, string> ProchaineQuote
        {
            get
            {
                return new KeyValuePair<IObjetPhysique, string>(QuotesObjets[TypesObjets[Main.SaveGame.Tutorials[Niveau] % Quotes.Count]], Quotes[Main.SaveGame.Tutorials[Niveau] % Quotes.Count]);
            }
        }

        public bool Termine
        {
            get
            {
                return (Main.SaveGame.Tutorials[Niveau] >= Quotes.Count * 2);
            }
        }

        private Main Main;
        private const double TEMPS_ENTRE_QUOTES = 10000;
        private double TempsProchaineQuote;

        public AideNiveau(Main main)
            : base(main)
        {
            Main = main;
            Niveau = -1;

            TypesObjets = new List<string>();
            Quotes = new List<string>();
            QuotesObjets = new Dictionary<string, IObjetPhysique>();

            TempsProchaineQuote = TEMPS_ENTRE_QUOTES / 2;
        }

        public AideNiveau(
            Main main,
            int niveau,
            List<KeyValuePair<string, string>> quotesDefs)
            : base(main)
        {
            Main = main;
            Niveau = niveau;

            if (!Main.SaveGame.Tutorials.ContainsKey(Niveau))
            {
                Main.SaveGame.Tutorials.Add(Niveau, 0);
                EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
            }

            TypesObjets = new List<string>();
            Quotes = new List<string>();
            QuotesObjets = new Dictionary<string, IObjetPhysique>();

            foreach (var kvp in quotesDefs)
            {
                TypesObjets.Add(kvp.Key);
                Quotes.Add(kvp.Value);
            }

            TempsProchaineQuote = TEMPS_ENTRE_QUOTES;
        }

        public override void Update(GameTime gameTime)
        {
            if (TempsProchaineQuote < 0)
                TempsProchaineQuote = TEMPS_ENTRE_QUOTES;

            TempsProchaineQuote -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public void incrementerQuoteLancee()
        {
            Main.SaveGame.Tutorials[Niveau]++;
            //EphemereGames.Core.Persistance.Facade.sauvegarderDonnee("savePlayer"); //sur la Xbox, ralentissement
        }
    }
}
