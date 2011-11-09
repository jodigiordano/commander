namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class PlayersCollisions
    {
        public List<KeyValuePair<SimPlayer, SimPlayer>> Output;

        private Simulator Simulator;

        private List<SimPlayer> PlayersList;


        public PlayersCollisions(Simulator simulator)
        {
            Simulator = simulator;

            Output = new List<KeyValuePair<SimPlayer, SimPlayer>>();
            PlayersList = new List<SimPlayer>();
        }


        public void Sync()
        {
            Output.Clear();

            PlayersList.Clear();

            foreach (var p in Simulator.Data.Players.Values)
                PlayersList.Add(p);


            for (int i = 0; i < PlayersList.Count; i++)
                for (int j = 0; j < PlayersList.Count; j++)
                {
                    if (j <= i)
                        continue;

                    var player1 = PlayersList[i];
                    var player2 = PlayersList[j];

                    if (Physics.CircleCicleCollision(player1.Circle, player2.Circle))
                        Output.Add(new KeyValuePair<SimPlayer, SimPlayer>(player1, player2));
                }
        }
    }
}
