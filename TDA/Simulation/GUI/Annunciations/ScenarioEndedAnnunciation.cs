namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using Core.Utilities;

    class ScenarioEndedAnnunciation
    {
        private IVisible Filter;
        private Objets.VaisseauAlien AlienShip;
        private List<KeyValuePair<IVisible, CorpsCeleste>> Missiles;
        private List<ParticuleEffectWrapper> MissilesVisual;
        private List<ParticuleEffectWrapper> Implosions;
        private List<CorpsCeleste> CelestialBodies;

        private EtatPartie GameState;
        private Double TimeLostState = 0;
        private int ImplosionsIndex = 0;

        private Translator TranslatorScenarioEnded;
        private Simulation Simulation;

        private static string[] QuotesLost = new string[]
        {
            "Even my grandmother is a better commander than\n\nyou! You're fired, commander!\n\nPress Start.",
            "You know you have to Press buttons on the\n\ncontroller, right? Press Start.\n\nOh, and you're fired, commander!",
            "So close and so far at the same time !\n\nNow take a step closer to the door because\n\nyou're fired, commander! Press Start.",
            "You just killed 8 billions people and my cat\n\nwas on this planet! You're so fired !\n\nPress Start... I'll miss you, baby Bear.",
            "Are you sure you want to retry ?\n\nI mean, I saw you play and it's kind of\n\nhopeless. I'm firing you and it's a favour,\n\ntrust me. Press Start.",
            "Alright, this one is hard. If you want, I\n\nhave some baby toys that you could play with.\n\nYou have plenty of time now because\n\nyou're fired, commander! Press Start.",
            "...\n\nYou're fired, commander !!!!!\n\nPress Start.",
            "They told me to not underestimated you. I\n\ndidn't know they were talking about your lack\n\nof skills. Can you at least Press Start ?",
        };

        public ScenarioEndedAnnunciation(Simulation simulation, List<CorpsCeleste> celestialBodies)
        {
            this.Simulation = simulation;

            CelestialBodies = celestialBodies;

            Missiles = new List<KeyValuePair<IVisible, CorpsCeleste>>();
            MissilesVisual = new List<ParticuleEffectWrapper>(CelestialBodies.Count);
            Implosions = new List<ParticuleEffectWrapper>(CelestialBodies.Count);

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                ParticuleEffectWrapper particule = this.Simulation.Scene.Particules.recuperer("missileAlien");
                particule.PrioriteAffichage = Preferences.PrioriteGUIVictoireDefaite + 0.10f;

                MissilesVisual.Add(particule);

                particule = this.Simulation.Scene.Particules.recuperer("implosionAlien");
                particule.PrioriteAffichage = Preferences.PrioriteGUIVictoireDefaite + 0.09f;

                Implosions.Add(particule);
            }

            AlienShip = new Objets.VaisseauAlien(simulation.Scene, Preferences.PrioriteGUIVictoireDefaite + 0.08f);

            Filter = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero);
            Filter.PrioriteAffichage = 0.02f;
            Filter.TailleVecteur = new Vector2(1800, 200);
            Filter.Couleur = new Color(Color.Black, 200);
        }

        public void doGameStateChanged(EtatPartie etat)
        {
            GameState = etat;

            if (GameState != EtatPartie.Gagnee && GameState != EtatPartie.Perdue)
                return;

            if (GameState == EtatPartie.Gagnee)
            {
                TranslatorScenarioEnded = new Translator
                (
                    Simulation.Main,
                    Simulation.Scene,
                    new Vector3(-600, -130, 0),
                    Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                    Color.White,
                    Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                    new Color(234, 196, 28, 0),
                    "Thank you Commander!\n\nBut our enemy destroyed " + Simulation.DescriptionScenario.Lieu + " anyway!\n\nPress Start.",
                    3,
                    true,
                    2000,
                    200
                );


                EffetDeplacementTrajet e = new EffetDeplacementTrajet();
                e.Delai = 2000;
                e.Duree = 15000;
                e.Progression = AbstractEffet.TypeProgression.Lineaire;
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

                this.Simulation.Scene.Effets.ajouter(AlienShip.Representation, e);
            }

            else if (GameState == EtatPartie.Perdue)
            {
                TranslatorScenarioEnded = new Translator
                (
                    Simulation.Main,
                    Simulation.Scene,
                    new Vector3(-600, -130, 0),
                    Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                    Color.White,
                    Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                    new Color(234, 196, 28, 0),
                    QuotesLost[Main.Random.Next(0, QuotesLost.Length)],
                    3,
                    true,
                    2000,
                    200
                );
            }

            TranslatorScenarioEnded.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIVictoireDefaite + 0.01f;
            TranslatorScenarioEnded.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIVictoireDefaite + 0.01f;

            this.Simulation.Scene.Effets.ajouter(TranslatorScenarioEnded.PartieTraduite, EffetsPredefinis.fadeInFrom0(255, 0, 1000));
            this.Simulation.Scene.Effets.ajouter(TranslatorScenarioEnded.PartieNonTraduite, EffetsPredefinis.fadeInFrom0(255, 0, 1000));
            this.Simulation.Scene.Effets.ajouter(Filter, EffetsPredefinis.fadeInFrom0(200, 0, 1000));

            EffetDeplacementTrajet effet = new EffetDeplacementTrajet();
            effet.Delai = 0;
            effet.Duree = 10000;
            effet.Progression = AbstractEffet.TypeProgression.Lineaire;
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

            this.Simulation.Scene.Effets.ajouter(Filter, effet);
        }


        public void Update(GameTime gameTime)
        {
            if (GameState == EtatPartie.Gagnee)
            {
                TimeLostState += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (TimeLostState > 8000)
                {
                    for (int i = 0; i < CelestialBodies.Count; i++)
                    {
                        Missiles.Add(new KeyValuePair<IVisible, CorpsCeleste>(new IVisible(), CelestialBodies[i]));

                        Missiles[i].Key.Position = AlienShip.Representation.Position;

                        EffetSuivre deplacement = new EffetSuivre();
                        deplacement.Delai = i * 500;
                        deplacement.Duree = 10000;
                        deplacement.ObjetSuivi = CelestialBodies[i];
                        deplacement.Vitesse = 20f;

                        this.Simulation.Scene.Effets.ajouter(Missiles[i].Key, deplacement);
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

            if (GameState == EtatPartie.Gagnee || GameState == EtatPartie.Perdue)
                TranslatorScenarioEnded.Update(gameTime);
        }


        public void Draw()
        {
            if (GameState == EtatPartie.Gagnee)
                AlienShip.Draw(null);

            if (GameState == EtatPartie.Gagnee || GameState == EtatPartie.Perdue)
            {
                TranslatorScenarioEnded.Draw(null);
                Simulation.Scene.ajouterScenable(Filter);
            }
        }
    }
}
