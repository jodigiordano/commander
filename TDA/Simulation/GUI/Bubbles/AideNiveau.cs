namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;

    class AideNiveau : GameComponent
    {
        private int Niveau;
        private int Sauvegarde;
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
                return new KeyValuePair<IObjetPhysique, string>(QuotesObjets[TypesObjets[Main.SaveGame.Progression[Sauvegarde] % Quotes.Count]], Quotes[Main.SaveGame.Progression[Sauvegarde] % Quotes.Count]);
            }
        }

        public bool Termine
        {
            get
            {
                return (Main.SaveGame.Progression[Sauvegarde] >= Quotes.Count * 2);
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
            Sauvegarde = -1;

            TypesObjets = new List<string>();
            Quotes = new List<string>();
            QuotesObjets = new Dictionary<string, IObjetPhysique>();

            TempsProchaineQuote = TEMPS_ENTRE_QUOTES / 2;
        }

        public AideNiveau(
            Main main,
            int niveau,
            int sauvegarde,
            List<KeyValuePair<string, string>> quotesDefs)
            : base(main)
        {
            Main = main;
            Niveau = niveau;
            Sauvegarde = sauvegarde;

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
            Main.SaveGame.Progression[Sauvegarde]++;
            //Core.Persistance.Facade.sauvegarderDonnee("savePlayer"); //sur la Xbox, ralentissement
        }
    }
}
