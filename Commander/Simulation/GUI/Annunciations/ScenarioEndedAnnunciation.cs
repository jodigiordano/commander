﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class ScenarioEndedAnnunciation
    {
        private Image Filter;
        private AlienSpaceship AlienShip;
        private List<KeyValuePair<Image, CelestialBody>> Missiles;
        private List<Particle> MissilesVisual;
        private List<Particle> Implosions;
        private List<CelestialBody> CelestialBodies;
        private Scenario Scenario;

        private GameState GameState;
        private Double TimeLostState = 0;
        private int ImplosionsIndex = 0;

        private Translator TranslatorGameWon;
        private Translator TranslatorScoreExplanations;
        private Translator TranslatorTotalScore;
        private Simulation Simulation;
        private bool HighscoreBeaten;
        private Text NewHighscore;
        private Image[] Stars;


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


        public ScenarioEndedAnnunciation(Simulation simulation, List<CelestialBody> celestialBodies, Scenario scenario)
        {
            Simulation = simulation;
            CelestialBodies = celestialBodies;
            Scenario = scenario;

            Missiles = new List<KeyValuePair<Image, CelestialBody>>();
            MissilesVisual = new List<Particle>(CelestialBodies.Count);
            Implosions = new List<Particle>(CelestialBodies.Count);

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                Particle particule = this.Simulation.Scene.Particles.Get(@"missileAlien");
                particule.VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.10f;

                MissilesVisual.Add(particule);

                particule = this.Simulation.Scene.Particles.Get(@"implosionAlien");
                particule.VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.09f;

                Implosions.Add(particule);
            }

            AlienShip = new AlienSpaceship(simulation.Scene, Preferences.PrioriteGUIVictoireDefaite + 0.08f);

            Filter = new Image("PixelBlanc")
            {
                Size = new Vector2(1800, 200),
                Color = new Color(0, 0, 0, 200),
                VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.02f,
                Origin = Vector2.Zero
            };

            Stars = new Image[3];

            for (int i = 0; i < 3; i++)
                Stars[i] = new Image("Star", new Vector3(-300 + i * 100, -10, 0))
                {
                    SizeX = 0.5f,
                    VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f
                };

            HighscoreBeaten = false;

            NewHighscore = new Text("", "Pixelite", new Color(234, 196, 28, 0), new Vector3(0, -30, 0))
            {
                SizeX = 3,
                VisualPriority = Preferences.PrioriteGUIVictoireDefaite + 0.01f
            };
        }


        public void DoGameStateChanged(GameState etat)
        {
            GameState = etat;

            //if (GameState != GameState.Won && GameState != GameState.Lost)
            //{
            //    AlienShip.Hide();
            //    return;
            //}

            //AlienShip.Show();

            TranslatorScoreExplanations = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -130, 0),
                "Alien",
                Color.White,
                "Pixelite",
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
                "Alien",
                Color.White,
                "Pixelite",
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

            NewHighscore.Data = ((HighscoreBeaten) ? "new highscore!" : "not your best!") + " (" + (diff > 0 ? "+" : "") + diff + ")";


            if (GameState == GameState.Won)
            {
                TranslatorGameWon = new Translator
                (
                    Simulation.Main,
                    Simulation.Scene,
                    new Vector3(-600, -130, 0),
                    "Alien",
                    Color.White,
                    "Pixelite",
                    new Color(234, 196, 28, 0),
                    "Thank you Commander!\n\nBut our enemy destroyed the colony anyway!",
                    3,
                    true,
                    2000,
                    200,
                    Preferences.PrioriteGUIVictoireDefaite + 0.01f
                );


                MovePathEffect e = new MovePathEffect();
                e.Delay = 2000;
                e.Length = 15000;
                e.Progress = Effect<IPhysicalObject>.ProgressType.Linear;
                e.InnerPath = new Path2D(new List<Vector2>
                {
                    new Vector2(-1200, 100),
                    new Vector2(-500, 100),
                    new Vector2(0, 100),
                    new Vector2(900, 100)
                }, new List<double>
                {
                    0,
                    4000,
                    12000,
                    15000
                });

                this.Simulation.Scene.PhysicalEffects.Add(AlienShip.Representation, e);

                this.Simulation.Scene.VisualEffects.Add(TranslatorGameWon.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorGameWon.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorGameWon.PartieTraduite, Core.Visual.VisualEffects.FadeOutTo0(255, 5000, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorGameWon.PartieNonTraduite, Core.Visual.VisualEffects.FadeOutTo0(255, 5000, 1000));

                this.Simulation.Scene.VisualEffects.Add(TranslatorScoreExplanations.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorScoreExplanations.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorTotalScore.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorTotalScore.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));

                int nbStars = Scenario.NbStars(Scenario.CommonStash.TotalScore);

                for (int i = 0; i < 3; i++)
                    this.Simulation.Scene.VisualEffects.Add(Stars[i], Core.Visual.VisualEffects.FadeInFrom0((i < nbStars) ? 255 : 50, 5000, 1000));

                 Simulation.Scene.VisualEffects.Add(NewHighscore, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));
            }

            else if (GameState == GameState.Lost)
            {
                this.Simulation.Scene.VisualEffects.Add(TranslatorScoreExplanations.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorScoreExplanations.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorTotalScore.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
                this.Simulation.Scene.VisualEffects.Add(TranslatorTotalScore.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));

                int nbStars = Scenario.NbStars(Scenario.CommonStash.TotalScore);

                for (int i = 0; i < 3; i++)
                    this.Simulation.Scene.VisualEffects.Add(Stars[i], Core.Visual.VisualEffects.FadeInFrom0((i < nbStars) ? 255 : 50, 0, 1000));

                Simulation.Scene.VisualEffects.Add(NewHighscore, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
            }

            this.Simulation.Scene.VisualEffects.Add(Filter, Core.Visual.VisualEffects.FadeInFrom0(200, 0, 1000));

            MovePathEffect effet = new MovePathEffect();
            effet.Delay = 0;
            effet.Length = 10000;
            effet.Progress = Effect<IPhysicalObject>.ProgressType.Linear;
            effet.InnerPath = new Path2D(new List<Vector2>
                {
                    new Vector2(-1920, -155),
                    new Vector2(-1000, -155),
                    new Vector2(-740, -155)
                }, new List<double>
                {
                    0,
                    600,
                    1200
                });

            this.Simulation.Scene.PhysicalEffects.Add(Filter, effet);
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
                        Missiles.Add(new KeyValuePair<Image, CelestialBody>(new Image("PixelBlanc", AlienShip.Representation.Position), CelestialBodies[i]));

                        FollowEffect deplacement = new FollowEffect();
                        deplacement.Delay = i * 500;
                        deplacement.Length = 10000;
                        deplacement.ObjetSuivi = CelestialBodies[i];
                        deplacement.Vitesse = 10f;
                        deplacement.Progress = Effect<IPhysicalObject>.ProgressType.Linear;

                        this.Simulation.Scene.PhysicalEffects.Add(Missiles[i].Key, deplacement);
                    }

                    TimeLostState = Double.NegativeInfinity;
                }

                if (TimeLostState == Double.NegativeInfinity)
                {
                    for (int i = Missiles.Count - 1; i > -1; i--)
                    {
                        if (Vector3.DistanceSquared(Missiles[i].Key.Position, Missiles[i].Value.Position) <= 400)
                        {
                            Vector3 pos = Missiles[i].Key.Position;
                            Implosions[ImplosionsIndex].Trigger(ref pos);
                            ImplosionsIndex++;

                            Missiles[i].Value.DoDie();

                            Missiles.RemoveAt(i);
                        }
                        else
                        {
                            Vector3 pos = Missiles[i].Key.Position;
                            MissilesVisual[i].Trigger(ref pos);
                        }
                    }
                }
                        

                AlienShip.Update(gameTime);
            }


            if (GameState == GameState.Won)
                TranslatorGameWon.Update();

            if (GameState == GameState.Won || GameState == GameState.Lost)
            {
                TranslatorScoreExplanations.Update();
                TranslatorTotalScore.Update();
            }
        }


        //public void Show()
        //{
        //    Simulation.Scene.Add(NewHighscore);
        //    Simulation.Scene.Add(Stars[0]);
        //    Simulation.Scene.Add(Stars[1]);
        //    Simulation.Scene.Add(Stars[2]);
        //    Simulation.Scene.Add(Filter);

        //    AlienShip.Show();

        //    if (GameState == GameState.Won)
        //        TranslatorGameWon.Show();

        //    TranslatorScoreExplanations.Show();
        //    TranslatorTotalScore.Show();
        //}


        //public void Hide()
        //{
        //    Simulation.Scene.Remove(NewHighscore);
        //    Simulation.Scene.Remove(Stars[0]);
        //    Simulation.Scene.Remove(Stars[1]);
        //    Simulation.Scene.Remove(Stars[2]);
        //    Simulation.Scene.Remove(Filter);

        //    AlienShip.Hide();

        //    if (GameState == GameState.Won)
        //        TranslatorGameWon.Hide();

        //    TranslatorScoreExplanations.Hide();
        //    TranslatorTotalScore.Hide();
        //}


        public void Draw()
        {
            if (GameState == GameState.Won)
            {
                AlienShip.Draw();
                TranslatorGameWon.Draw();
            }

            if (GameState == GameState.Won || GameState == GameState.Lost)
            {
                TranslatorScoreExplanations.Draw();
                TranslatorTotalScore.Draw();

                Simulation.Scene.Add(NewHighscore);
                Simulation.Scene.Add(Stars[0]);
                Simulation.Scene.Add(Stars[1]);
                Simulation.Scene.Add(Stars[2]);
                Simulation.Scene.Add(Filter);
            }
        }
    }
}