﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class LevelEndedAnnunciation
    {
        private CelestialBody CelestialBody;
        private Level Level;

        private Simulator Simulator;
        private Path Path;

        private GameEndedBubble Bubble;


        private static List<string> WonQuotes = new List<string>()
        {
            "Thank you Commander!\n\n\n\n\n\n" +
            "You saved us but the\n\n" +
            "aliens are already\n\n" +
            "attacking another colony!",

            "That was awesome, Commander!\n\n\n\n\n\n" +
            "Today you saved many lives,\n\n" +
            "but the war is not over!\n\n" +
            "Hurry up! Go save them all!"
        };


        private static List<string> LostQuotes = new List<string>()
        {
            "Mouhahaha! ...\n\n\n\n\n\n" +
            "Mouhahahaha! ...\n\n" +
            "Mouhahahaaaaa! You lost.",

            "Nice try for a human! \n\n\n\n\n\n" +
            "Next time try to press some\n\n" +
            "buttons on the controller.\n\n"
        };


        public LevelEndedAnnunciation(Simulator simulator, Path path, Level level)
        {
            Simulator = simulator;
            Path = path;
            Level = level;
        }


        public void DoGameStateChanged(GameState state)
        {
            if (state != GameState.Won && state != GameState.Lost)
                return;

            Bubble = new GameEndedBubble(
                Simulator.Scene,
                VisualPriorities.Default.GameEndedAnimation,
                state == GameState.Won ? WonQuotes[Main.Random.Next(0, WonQuotes.Count)] : LostQuotes[Main.Random.Next(0, LostQuotes.Count)],
                state == GameState.Won ? Colors.Default.HumansBright : Colors.Default.AlienBright,
                Level.CommonStash,
                Level.GetStarsCount(Level.CommonStash.TotalScore)
            );

            CelestialBody = state == GameState.Won ? Path.LastCelestialBody : Path.FirstCelestialBody;

            Bubble.Alpha = 0;
            Simulator.Scene.VisualEffects.Add(Bubble, Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
        }


        public void Update()
        {
            if (Bubble == null)
                return;

            Bubble.Position = CelestialBody.Position;
            Bubble.Update();
        }


        public void Draw()
        {
            if (Bubble == null)
                return;

            Bubble.Draw();
        }
    }
}
