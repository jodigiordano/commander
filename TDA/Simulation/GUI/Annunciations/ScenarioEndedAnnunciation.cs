namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;

    class ScenarioEndedAnnunciation
    {
        private IVisible Filter;
        private VaisseauAlien AlienShip;
        private List<KeyValuePair<IVisible, CorpsCeleste>> Missiles;
        private List<ParticuleEffectWrapper> MissilesVisual;
        private List<ParticuleEffectWrapper> Implosions;
        private List<CorpsCeleste> CelestialBodies;
        private Scenario Scenario;

        private GameState GameState;
        private Double TimeLostState = 0;
        private int ImplosionsIndex = 0;

        private Translator TranslatorGameWon;
        private Translator TranslatorScoreExplanations;
        private Translator TranslatorTotalScore;
        private Simulation Simulation;
        private bool HighscoreBeaten;
        private IVisible NewHighscore;
        private IVisible[] Stars;


        //private static string[] QuotesLost = new string[]
        //{
        //    "Even my grandmother is a better commander than\n\nyou! You're fired, commander!\n\nPress Start.",
        //    "You know you have to Press buttons on the\n\ncontroller, right? Press Start.\n\nOh, and you're fired, commander!",
        //    "So close and so far at the same time !\n\nNow take a step closer to the door because\n\nyou're fired, commander! Press Start.",
        //    "You just killed 8 billions people and my cat\n\nwas on this planet! You're so fired !\n\nPress Start... I'll miss you, baby Bear.",
        //    "Are you sure you want to retry ?\n\nI mean, I saw you play and it's kind of\n\nhopeless. I'm firing you and it's a favour,\n\ntrust me. Press Start.",
        //    "Alright, this one is hard. If you want, I\n\nhave some baby toys that you could play with.\n\nYou have plenty of time now because\n\nyou're fired, commander! Press Start.",
        //    "...\n\nYou're fired, commander !!!!!\n\nPress Start.",
        //    "They told me to not underestimated you. I\n\ndidn't know they were talking about your lack\n\nof skills. Can you at least Press Start ?",
        //};


        public ScenarioEndedAnnunciation(Simulation simulation, List<CorpsCeleste> celestialBodies, Scenario scenario)
        {
            Simulation = simulation;
            CelestialBodies = celestialBodies;
            Scenario = scenario;

            Missiles = new List<KeyValuePair<IVisible, CorpsCeleste>>();
            MissilesVisual = new List<ParticuleEffectWrapper>(CelestialBodies.Count);
            Implosions = new List<ParticuleEffectWrapper>(CelestialBodies.Count);

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                ParticuleEffectWrapper particule = this.Simulation.Scene.Particules.recuperer("missileAlien");
                particule.VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.10f;

                MissilesVisual.Add(particule);

                particule = this.Simulation.Scene.Particules.recuperer("implosionAlien");
                particule.VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.09f;

                Implosions.Add(particule);
            }

            AlienShip = new VaisseauAlien(simulation.Scene, Preferences.PrioriteGUIVictoireDefaite + 0.08f);

            Filter = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
            Filter.VisualPriority = 0.02f;
            Filter.TailleVecteur = new Vector2(1800, 200);
            Filter.Couleur = new Color(0, 0, 0, 200);

            Stars = new IVisible[3];
            Stars[0] = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Star"), new Vector3(-300, -10, 0));
            Stars[0].Taille = 0.5f;
            Stars[0].Origine = Stars[0].Centre;
            Stars[0].VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f;
            Stars[1] = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Star"), new Vector3(-200, -10, 0));
            Stars[1].Taille = 0.5f;
            Stars[1].Origine = Stars[0].Centre;
            Stars[1].VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f;
            Stars[2] = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Star"), new Vector3(-100, -10, 0));
            Stars[2].Taille = 0.5f;
            Stars[2].Origine = Stars[0].Centre;
            Stars[2].VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f;

            HighscoreBeaten = false;
            NewHighscore = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), new Color(234, 196, 28, 0), new Vector3(0, -30, 0));
            NewHighscore.Taille = 3;
            NewHighscore.VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f;
        }


        public void doGameStateChanged(GameState etat)
        {
            GameState = etat;

            if (GameState != GameState.Won && GameState != GameState.Lost)
                return;


            TranslatorScoreExplanations = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -130, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                Color.White,
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(234, 196, 28, 0),
                Scenario.CommonStash.Score + " (base) + " +
                Scenario.CommonStash.Cash + " (cash) +\n\n" +
                (Scenario.CommonStash.Lives * 50) + " (lives) + " +
                Scenario.CommonStash.TimeLeft + " (time) =",
                3,
                true,
                2000,
                200,
                Preferences.PrioriteGUIVictoireDefaite + 0.01f
            );


            TranslatorTotalScore = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -40, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                Color.White,
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(234, 196, 28, 0),
                Scenario.CommonStash.TotalScore.ToString(),
                5,
                true,
                2000,
                200,
                Preferences.PrioriteGUIVictoireDefaite + 0.01f
            );


            HighScores h;
            Simulation.Main.SaveGame.HighScores.TryGetValue(Scenario.Id, out h);
            HighscoreBeaten = h == null || h.Scores.Count <= 0 || Scenario.CommonStash.TotalScore > h.Scores[0].Value;
            int diff = (h == null || h.Scores.Count <= 0) ? Scenario.CommonStash.TotalScore : Scenario.CommonStash.TotalScore - h.Scores[0].Value;

            NewHighscore.Texte = ((HighscoreBeaten) ? "new highscore!" : "not your best!") + " (" + (diff > 0 ? "+" : "") + diff + ")";


            if (GameState == GameState.Won)
            {
                TranslatorGameWon = new Translator
                (
                    Simulation.Main,
                    Simulation.Scene,
                    new Vector3(-600, -130, 0),
                    EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                    Color.White,
                    EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                    new Color(234, 196, 28, 0),
                    "Thank you Commander!\n\nBut our enemy destroyed the colony anyway!",
                    3,
                    true,
                    2000,
                    200,
                    Preferences.PrioriteGUIVictoireDefaite + 0.01f
                );


                EffetDeplacementTrajet e = new EffetDeplacementTrajet();
                e.Delay = 2000;
                e.Length = 15000;
                e.Progress = AbstractEffect.ProgressType.Linear;
                e.Trajet = new Trajet2D(new Vector2[]
                {
                    new Vector2(-1200, 100),
                    new Vector2(-500, 100),
                    new Vector2(0, 100),
                    new Vector2(900, 100)
                }, new double[]
                {
                    0,
                    4000,
                    12000,
                    15000
                });

                this.Simulation.Scene.Effets.Add(AlienShip.Representation, e);

                this.Simulation.Scene.Effets.Add(TranslatorGameWon.PartieTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorGameWon.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorGameWon.PartieTraduite, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 5000, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorGameWon.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 5000, 1000));

                this.Simulation.Scene.Effets.Add(TranslatorScoreExplanations.PartieTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorScoreExplanations.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorTotalScore.PartieTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorTotalScore.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 5000, 1000));

                int nbStars = Scenario.NbStars(Scenario.CommonStash.TotalScore);

                for (int i = 0; i < 3; i++)
                    this.Simulation.Scene.Effets.Add(Stars[i], Core.Visuel.PredefinedEffects.FadeInFrom0((i < nbStars) ? 255 : 50, 5000, 1000));

                 Simulation.Scene.Effets.Add(NewHighscore, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 5000, 1000));
            }

            else if (GameState == GameState.Lost)
            {
                this.Simulation.Scene.Effets.Add(TranslatorScoreExplanations.PartieTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorScoreExplanations.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorTotalScore.PartieTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.Effets.Add(TranslatorTotalScore.PartieNonTraduite, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));

                int nbStars = Scenario.NbStars(Scenario.CommonStash.TotalScore);

                for (int i = 0; i < 3; i++)
                    this.Simulation.Scene.Effets.Add(Stars[i], Core.Visuel.PredefinedEffects.FadeInFrom0((i < nbStars) ? 255 : 50, 0, 1000));

                Simulation.Scene.Effets.Add(NewHighscore, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));
            }

            this.Simulation.Scene.Effets.Add(Filter, Core.Visuel.PredefinedEffects.FadeInFrom0(200, 0, 1000));

            EffetDeplacementTrajet effet = new EffetDeplacementTrajet();
            effet.Delay = 0;
            effet.Length = 10000;
            effet.Progress = AbstractEffect.ProgressType.Linear;
            effet.Trajet = new Trajet2D(new Vector2[]
                {
                    new Vector2(-1920, -155),
                    new Vector2(-1000, -155),
                    new Vector2(-740, -155)
                }, new double[]
                {
                    0,
                    600,
                    1200
                });

            this.Simulation.Scene.Effets.Add(Filter, effet);
        }


        public void Update(GameTime gameTime)
        {
            if (GameState == GameState.Won)
            {
                TimeLostState += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (TimeLostState > 8000)
                {
                    for (int i = 0; i < CelestialBodies.Count; i++)
                    {
                        Missiles.Add(new KeyValuePair<IVisible, CorpsCeleste>(new IVisible(), CelestialBodies[i]));

                        Missiles[i].Key.Position = AlienShip.Representation.Position;

                        EffetSuivre deplacement = new EffetSuivre();
                        deplacement.Delay = i * 500;
                        deplacement.Length = 10000;
                        deplacement.ObjetSuivi = CelestialBodies[i];
                        deplacement.Vitesse = 20f;

                        this.Simulation.Scene.Effets.Add(Missiles[i].Key, deplacement);
                    }

                    TimeLostState = Double.NegativeInfinity;
                }

                if (TimeLostState == Double.NegativeInfinity)
                {
                    for (int i = Missiles.Count - 1; i > -1; i--)
                    {
                        if (Vector3.DistanceSquared(Missiles[i].Key.Position, Missiles[i].Value.Position) <= 400)
                        {
                            Implosions[ImplosionsIndex].Emettre(ref Missiles[i].Key.position);
                            ImplosionsIndex++;

                            Missiles[i].Value.doMeurt();

                            Missiles.RemoveAt(i);
                        }
                        else
                            MissilesVisual[i].Emettre(ref Missiles[i].Key.position);
                    }
                }
                        

                AlienShip.Update(gameTime);
            }


            if (GameState == GameState.Won)
                TranslatorGameWon.Update(gameTime);

            if (GameState == GameState.Won || GameState == GameState.Lost)
            {
                TranslatorScoreExplanations.Update(gameTime);
                TranslatorTotalScore.Update(gameTime);
            }
        }


        public void Draw()
        {
            if (GameState == GameState.Won)
            {
                AlienShip.Draw(null);
                TranslatorGameWon.Draw(null);
            }


            if (GameState == GameState.Won || GameState == GameState.Lost)
            {
                TranslatorScoreExplanations.Draw(null);
                TranslatorTotalScore.Draw(null);

                Simulation.Scene.ajouterScenable(NewHighscore);
                Simulation.Scene.ajouterScenable(Stars[0]);
                Simulation.Scene.ajouterScenable(Stars[1]);
                Simulation.Scene.ajouterScenable(Stars[2]);
                Simulation.Scene.ajouterScenable(Filter);
            }
        }
    }
}
