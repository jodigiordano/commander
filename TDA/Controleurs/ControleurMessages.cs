namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using Core.Visuel;
    using Core.Utilities;

    class ControleurMessages : DrawableGameComponent
    {
        public List<Tourelle> Tourelles;
        public CorpsCeleste CorpsCelesteAProteger;
        public Sablier Sablier;
        public Curseur Curseur;
        public List<CorpsCeleste> CorpsCelestes;
        public Chemin Chemin;
        public Bulle BulleGUI;
        public AideNiveau AideNiveau;

        private Simulation Simulation;
        private Dictionary<IObjetPhysique, BulleTexte> ObjetsParlants;
        private double TempsDerniereQuoteTourelleLancee;
        private List<KeyValuePair<IObjetPhysique, BulleTexte>> toDelete;

        private static List<String> QuotesEnnui = new List<string>()
        {
            "I'm bored.",
            "I'm hungry.",
            "Give me some asteroids!",
            "I want to go home.",
            "Let's dance!",
            "To be, or not to be:\n\nthat is the question"
        };

        private static List<String> QuotesHistoire = new List<string>()
        {
            "The aliens started attacking\n\nour colonies in 2050.",
            "Some say they're green.\n\nI'm sure they stink.",
            "It's year 2100.\n\nThis war must end." ,
            "The humans created us\n\nto protect them.\n\nWho will protect us?",
            "My serial number\n\nis RP-65535. Hmmm.",
            "We try to protect the\n\ncolonies but this alien\n\nmothership is so powerful.",
            "When we see the mothership,\n\nwe know it's the end.",
            "Fifty years later,\n\nwe still don't know\n\nwhat they want.",
            "We should give them Earth\n\nin exchange of peace.",
            "You are the only hope\n\nof humanity, Commander.",
            "When the war is over,\n\nwhat about me ?",
            "The aliens are\n\nthrowing asteroids at us.\n\nThey are mean.",
            "I think green was the\n\nfirst. Then came yellow.",
            "World 1 has 9 colonies.\n\nThey must not discover World 2.",
            "There is a warp gate from\n\nWorld 1 to World 2. The\n\nlegend says it will be\n\nactivated one day.",
            "What if the aliens win?\n\nI don't want to speak alien.",
            "Pink is so cute.\n\nI'm sad that I can't love her.",
            "Power to the mercenaries!\n\nWe do all the job!",
            "The Mercenaries Alliance\n\nwill one day rule this universe.",
            "I wish I had wings\n\nlike The Resistance.",
            "The aliens are cowards.\n\nThey throw rocks at us like kids.",
            "Humans say the war could\n\nlast 20 years. I give them 5.",
            "When they can't kill\n\nus with asteroids,\n\nthey call the mothership.",
            "Every colony is falling,\n\none after another.",
            "The human civilization is\n\nliving it's last decade.",
            "They have what ? 30 ?\n\n50 colonies ? Something like that.",
            "Another 2384292734043 kills\n\nand they will send me home.",
            "Humans should treat us\n\nbetter. We're not machines.",
            "I have seen one.\n\nIt was scary.\n\nI don't want to think about it.",
            "They say World 2 is gearing\n\nup with new technology.",
            "The mothership is a legend.\n\nI'll believe it when I'll see it.",
            "Do the aliens have\n\nan homeworld or they\n\nlive on their ship?",
            "The aliens are\n\nscattered on the\n\nasteroids' belt.",
            "How do they find\n\nevery human colony\n\nby the way ?",
            "Intel says aliens\n\nhave spies among humans.\n\nIt's insane.",
            "Intel says one colony\n\nis giving them others\n\ncolonies' coordinates.",
            "We'll be on the cover\n\nof Time Magazine this year.\n\nI'm so proud.",
            "Intel says there is a\n\nship bigger than the mothership.",
            "Intel says they\n\nhave a plan. Yeah right."
        };

        private static List<String> QuotesMissile = new List<string>()
        {
            "I figured out everything,\n\nexcept how to live.",
            "You know, every man dies.\n\nNot every man really lives.",
            "My life is a long\n\nlesson in humility.",
            "I accept no one's definition\n\nof my life; I define myself.",
            "I now know that only a few\n\nthings are really important.", 
            "I'm living deeply; thereof\n\nI have no fear of death.",
            "The purpose of my life\n\nis a life of purpose.",
            "This life is worth living\n\nbecause it is what I make it.",
            "What I play is life.",
            "Happiness is overrated.",
            "I'm happy now that I stopped\n\nlooking for the meaning of life.",
            "A mistake is simply\n\nanother way of doing things.",
            "Adapt or perish, now as ever,\n\nis nature's inexorable imperative.",
            "Autumn is a second spring\n\nwhen every leaf is a flower.",
        };


        private static List<String> QuotesLaserMultiple = new List<string>()
        {
            "A lot of people are afraid of heights.\n\nNot me, I'm afraid of widths.",
            "A nickel ain't\n\nworth a dime anymore.",
            "Anyone who says he can see\n\nthrough women is missing a lot.",
            "California is a fine place to live\n\nif you happen to be an orange.",
            "Curiosity killed the cat,\n\nbut for a while I was a suspect.",
            "Everywhere is within walking\n\ndistance if you have the time.",
            "Fashions have done\n\nmore harm than revolutions.",
            "Food is an important part\n\nof a balanced diet.",
            "Go to Heaven for the climate,\n\nHell for the company.",
            "God did not intend religion\n\nto be an exercise club.",
            "He would make a lovely corpse.",
            "I am free of all prejudices.\n\nI hate every one equally.",
            "I bought some batteries,\n\nbut they weren't included.",
            "I cook with wine, sometimes\n\nI even add it to the food.",
            "I intend to live forever.\n\nSo far, so good.",
            "I like children - fried.",
            "I like marriage. The idea.",
            "I love Mickey Mouse more than\n\nany woman I have ever known.",
            "I never said most of\n\nthe things I said.",
            "I rant, therefore I am.",
            "I refuse to join any club\n\nthat would have me as a member.",
            "I think serial monogamy says it all.",
            "I used to be Snow White,\n\nbut I drifted.",
            "I used to jog but the ice cubes\n\nkept falling out of my glass.",
            "I would never die for my\n\nbeliefs because I might be wrong.",
            "I'm an idealist. I don't know\n\nwhere I'm going, but I'm on my way.",
            "If at first you don't succeed,\n\nfailure may be your style.",
            "If God wanted us to bend over\n\nhe'd put diamonds on the floor.",
            "If love is the answer, could you\n\nplease rephrase the question?",
            "If two wrongs don't\n\nmake a right, try three.",
            "Life is hard. After all,\n\nit kills you.",
            "My computer beat me at checkers,\n\nbut I sure beat it at kickboxing.",
            "My fake plants died because\n\nI did not pretend to water them.",
            "Never fight an inanimate object.",
            "Never have more children\n\nthan you have car windows.",
            "Never wear anything that panics the cat.",
            "O Lord, help me to be pure,\n\nbut not yet.",
            "Parents are the last people\n\non earth who ought to have children.",
            "Procrastination is the art\n\nof keeping up with yesterday.",
            "Roses are red, violets are blue,\n\nI'm schizophrenic, and so am I.",
            "The superfluous,\n\na very necessary thing.",
            "There is no sadder sight\n\nthan a young pessimist.",
            "Weather forecast\n\nfor tonight: dark.",
            "You're only as good\n\nas your last haircut.",
        };


        private static List<String> QuotesLaserSimple = new List<string>()
        {
            "I do all things with love.",
            "Friendship often ends in love;\n\nbut love in friendship - never.",
            "If I judge people, I have\n\nno time to love them.",
            "If I want to be loved,\n\nI must be lovable.",
            "Love can sometimes be magic.\n\nBut magic can sometimes...\n\njust be an illusion.",
            "What the world really needs\n\nis more love and less paper work.",
            "When love is not madness,\n\nit is not love.",
            "I cannot be lonely because I\n\nlike the person I'm alone with.",
            "The way to love\n\nanything is to realize\n\nthat it may be lost.", 
            "I loved with a love that\n\nwas more than love.",
            "Loving is not just looking at\n\neach other, it's looking in\n\nthe same direction.",
            "Nobody has ever measured,\n\nnot even poets, how\n\nmuch the heart can hold.", 
            "The best proof of love is trust.",
            "The best thing to hold onto\n\nin life is each other.",
            "Love is metaphysical gravity.",
            "Love is the triumph of\n\nimagination over intelligence.",
            "Love is what you've\n\nbeen through with somebody.",
        };


        private static List<String> QuotesGravitationnelle = new List<string>()
        {
            "Computers are useless. They\n\ncan only give you answers.",
            "I think computer viruses\n\nshould count as life.",
            "Never trust a computer you\n\ncan't throw out a window.",
            "With computers I\n\nmake mistakes faster.",
            "Men have become the\n\ntools of their tools.",
            "Microsoft isn't evil, they\n\njust make really crappy\n\noperating systems.",
            "The fog of information\n\ncan drive out knowledge.",
            "Technology is the knack of\n\nso arranging the world that\n\nwe don't have to experience it.",
            "The real problem is\n\nnot whether machines think\n\nbut whether men do.",
            "We are the children\n\nof a technological age.",
            "I affect the world\n\nby what I browse."
        };


        private static List<String> QuotesBase = new List<string>()
        {
            "I won't be old until regrets\n\ntake the place of my dreams.",
            "Dreams are illustrations...\n\nfrom the book my soul is\n\nwriting about me.",
            "It takes a lot of courage to\n\nshow my dreams to someone else.",
            "Reality is wrong.\n\nDreams are for real.",
            "The best way to make my\n\ndreams come true is to wake up.",
            "Fiction reveals truths\n\nthat reality obscures.",
            "I like nonsense, it\n\nwakes up the brain cells.",
            "If everyone is thinking alike,\n\nthen somebody isn't thinking.",
            "Imagination rules the world.",
            "It's not what I look at\n\nthat matters, it's what I see.",
            "I live out of my imagination,\n\nnot my history.",
            "There are no rules of\n\narchitecture for a castle\n\nin the clouds.",
            "To imagine is everything,\n\nto know is nothing at all.",
            "I can't wait for inspiration.\n\nI have to go after it with a club.",
            "All things are difficult\n\nbefore they are easy.",
            "I'm regular and orderly in\n\nmy life, so that I may be\n\nviolent and original in my work.",
            "Every noble work is\n\nat first impossible.",
            "If I find a job I like, I'll\n\nadd five days to every week.",
            "I have not failed. I've\n\njust found 10,000 ways that\n\nwon't work.",
            "It is my work in life that\n\nis the ultimate seduction.",
            "It's a shame that the\n\nonly thing a man can do for\n\neight hours a day is work.",
            "Laziness may appear attractive,\n\nbut work gives satisfaction.",
            "Men for the sake of getting\n\na living forget to live.",
            "The harder I work,\n\nthe luckier I get.",
            "The more I want to\n\nget something done, the\n\nless I call it work.",
            "To find joy in work is to\n\ndiscover the fountain of youth.",
            "I work to become,\n\nnot to acquire.",
            "Work is love made visible.",
            "Lost time is never found again.",
            "The time I kill is killing me.",
            "The trouble with our times is that\n\nthe future is not what it used to be.",
            "Time is the longest distance\n\nbetween two places.",
            "Time is the wisest\n\ncounsellor of all.",
            "The best way to predict\n\nthe future is to invent it.",
        };


        private static List<String> QuotesSlowMotion = new List<string>()
        {
            "Anger and intolerance\n\nare the enemies of\n\ncorrect understanding.",
            "Anger is a short madness.",
            "Anger is a wind which blows\n\nout the lamp of the mind.",
            "I get mad, then I get over it.",
            "He who angers me conquers me.",
            "I keep cool; anger\n\nis not an argument.",
            "I never go to bed mad.\n\nI stay up and fight.",
            "No man can think clearly\n\nwhen his fists are clenched.",
            "If I speak when I'm angry,\n\nI will make the best speech\n\nI will ever regret.",
            "Whatever is begun in\n\nanger ends in shame.",
            "When anger rises, I\n\nthink of the consequences.",
            "When I'm angry, I count to four;\n\nwhen I'm very angry, I swear.",
            "Forgiveness is the key\n\nto action and freedom.",
            "Forgiveness means\n\nletting go of the past.",
            "It is often easier to ask for\n\nforgiveness than to ask for permission.",
            "I forgive to the\n\ndegree that I love.",
            "I am a citizen of the world.",
            "Nationalism is an infantile disease.\n\nIt is the measles of mankind.",
            "Patriotism is a kind of\n\nreligion; it is the egg from\n\nwhich wars are hatched.",
            "Patriotism is the\n\nvirtue of the vicious.",
            "Nobody can bring me\n\npeace but myself.",
            "Peace and justice\n\nare two sides of the\n\nsame coin.",
            "Peace begins with a smile.",
            "People always make war\n\nwhen they say they love peace.",
            "Those who are at war\n\nwith others are not at peace\n\nwith themselves.",
            "I don't have\n\nto have fought in a war\n\nto love peace.",
        };

        public ControleurMessages(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.ObjetsParlants = new Dictionary<IObjetPhysique, BulleTexte>();

            TempsDerniereQuoteTourelleLancee = 0;

            toDelete = new List<KeyValuePair<IObjetPhysique, BulleTexte>>();
        }


        public override void Initialize()
        {
            initAideNiveau();
        }


        public override void Update(GameTime gameTime)
        {
            TempsDerniereQuoteTourelleLancee += gameTime.ElapsedGameTime.TotalMilliseconds;

            doQuoteTourelle(gameTime);

            if (AideNiveau != null && Simulation.Etat == EtatPartie.EnCours)
            {
                AideNiveau.Update(gameTime);

                if (AideNiveau.PeutLancerNouvelleQuote)
                {
                    KeyValuePair<IObjetPhysique, string> prochaineQuote = AideNiveau.ProchaineQuote;

                    afficherMessage(prochaineQuote.Key, prochaineQuote.Value, 7000, Preferences.PrioriteGUIPanneauCorpsCeleste - 0.0001f);
                    AideNiveau.incrementerQuoteLancee();
                }
            }

            toDelete.Clear();

            foreach (var kvp in ObjetsParlants)
            {
                kvp.Value.Position = kvp.Key.Position;
                kvp.Value.Update(gameTime);

                if (kvp.Value.Termine)
                    toDelete.Add(kvp);
            }

            foreach (var kvp in toDelete)
                ObjetsParlants.Remove(kvp.Key);

            if (AideNiveau != null && AideNiveau.Termine)
                verifierCollisions();
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (var bulle in ObjetsParlants.Values)
                bulle.Draw(gameTime);
        }

        public void afficherMessage(IObjetPhysique objet, String message, double temps, float prioriteAffichage)
        {
            if (objet == null || ObjetsParlants.ContainsKey(objet))
                return;

            IVisible texteInfos = new IVisible(message, Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
            texteInfos.Taille = 2;

            prioriteAffichage = (prioriteAffichage == -1) ? Preferences.PrioriteSimulationTourelle - 0.02f : prioriteAffichage;

            BulleTexte bulle = new BulleTexte(Simulation, texteInfos, objet.Position, 0, prioriteAffichage);

            bulle.Texte.Texte = message;
            bulle.TempsAffichage = temps;

            toDelete.Clear();

            foreach (var kvp in ObjetsParlants)
                if (kvp.Value.Dimension.Intersects(bulle.Dimension))
                    toDelete.Add(kvp);

            foreach (var kvp in toDelete)
                ObjetsParlants.Remove(kvp.Key);

            bulle.doShow(250);
            bulle.TempsFadeOut = 250;
            ObjetsParlants.Add(objet, bulle);
        }

        public void arreterMessage(IObjetPhysique objet)
        {
            BulleTexte bulle;

            if (objet != null && ObjetsParlants.TryGetValue(objet, out bulle) && !double.IsNaN(bulle.TempsFadeOut))
            {
                ObjetsParlants[objet].TempsAffichage = ObjetsParlants[objet].TempsFadeOut;
            }
        }


        private void doQuoteTourelle(GameTime gameTime)
        {
            if (!Simulation.ModeDemo)
                return;

            if (Tourelles.Count <= 0)
                return;

            if (AideNiveau != null && !AideNiveau.Termine)
                return;

            if (TempsDerniereQuoteTourelleLancee > 5000)
            {
                TempsDerniereQuoteTourelleLancee = 0;

                if (Main.Random.Next(0, 5) == 0)
                {

                    Tourelle tourelle = Tourelles[Main.Random.Next(0, Tourelles.Count)];

                    if (!tourelle.Visible || !tourelle.Spectateur || tourelle.Type == TypeTourelle.GravitationnelleAlien || ObjetsParlants.ContainsKey(tourelle))
                        return;

                    IVisible texte = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
                    texte.Taille = 2;
                    texte.Couleur = tourelle.Couleur;

                    if (Main.Random.Next(0, 10) == 1)
                    {
                        texte.Texte = QuotesHistoire[Main.Random.Next(0, QuotesHistoire.Count)];
                    }

                    else
                    {
                        switch (tourelle.Type)
                        {
                            case TypeTourelle.Base: texte.Texte = QuotesBase[Main.Random.Next(0, QuotesBase.Count)]; break;
                            case TypeTourelle.Gravitationnelle: texte.Texte = QuotesGravitationnelle[Main.Random.Next(0, QuotesGravitationnelle.Count)]; break;
                            case TypeTourelle.LaserMultiple: texte.Texte = QuotesLaserMultiple[Main.Random.Next(0, QuotesLaserMultiple.Count)]; break;
                            case TypeTourelle.LaserSimple: texte.Texte = QuotesLaserSimple[Main.Random.Next(0, QuotesLaserSimple.Count)]; break;
                            case TypeTourelle.Missile: texte.Texte = QuotesMissile[Main.Random.Next(0, QuotesMissile.Count)]; break;
                            case TypeTourelle.SlowMotion: texte.Texte = QuotesSlowMotion[Main.Random.Next(0, QuotesSlowMotion.Count)]; break;
                        }
                    }

                    BulleTexte bulle = new BulleTexte(Simulation, texte, tourelle.Position, 0, Preferences.PrioriteSimulationTourelle - 0.02f);
                    bulle.TempsAffichage = texte.Texte.Length * 100;

                    foreach (var kvp in ObjetsParlants)
                        if (kvp.Value.Dimension.Intersects(bulle.Dimension))
                            return;

                    bulle.doShow(250);
                    bulle.TempsFadeOut = 250;
                    ObjetsParlants.Add(tourelle, bulle);
                }
            }
        }

        private void verifierCollisions()
        {
            if (BulleGUI != null && BulleGUI.Visible)
            {
                toDelete.Clear();

                foreach (var kvp in ObjetsParlants)
                    if (BulleGUI.Dimension.Intersects(kvp.Value.Dimension))
                        toDelete.Add(kvp);

                foreach (var kvp in toDelete)
                {
                    ObjetsParlants.Remove(kvp.Key);
                }
            }

            toDelete.Clear();

            foreach (var kvp in ObjetsParlants)
                foreach (var kvp2 in ObjetsParlants)
                    if (kvp.Key != kvp2.Key && !toDelete.Contains(kvp) && kvp.Value.Dimension.Intersects(kvp2.Value.Dimension))
                        toDelete.Add(kvp2);

            foreach (var kvp in toDelete)
            {
                ObjetsParlants.Remove(kvp.Key);
            }
        }

        private void initAideNiveau()
        {
            if (AideNiveau == null)
                return;

            foreach (var objet in AideNiveau.TypesObjets)
            {
                if (AideNiveau.QuotesObjets.ContainsKey(objet))
                    continue;

                switch (objet)
                {
                    case "PAP":
                        AideNiveau.QuotesObjets.Add(objet, CorpsCelesteAProteger);
                        break;
                    case "Sablier":
                        AideNiveau.QuotesObjets.Add(objet, Sablier);
                        break;
                    case "Curseur":
                        AideNiveau.QuotesObjets.Add(objet, Curseur);
                        break;
                    case "Emplacement":
                        for (int i = 1; i < CorpsCelestes.Count; i++)
                            if (CorpsCelestes[i].Emplacements != null && CorpsCelestes[i].Emplacements.Count > 1)
                            {
                                AideNiveau.QuotesObjets.Add(objet, CorpsCelestes[i].Emplacements[1]);
                                break;
                            }
                        break;
                    case "Grav":
                        AideNiveau.QuotesObjets.Add(objet, Chemin.prochainRelais(Chemin.PremierRelais).Emplacements[0]);
                        break;
                    case "Planete":
                        AideNiveau.QuotesObjets.Add(objet, CorpsCelestes[CorpsCelestes.Count - 2]);
                        break;
                    case "Ceinture":
                        AideNiveau.QuotesObjets.Add(objet, Chemin.PremierRelais);
                        break;
                }
            }
        }
    }
}
