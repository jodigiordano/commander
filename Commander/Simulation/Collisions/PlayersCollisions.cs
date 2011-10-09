namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class PlayersCollisions
    {
        public List<KeyValuePair<SimPlayer, SimPlayer>> Output;

        public List<SimPlayer> Players;


        public PlayersCollisions()
        {
            Output = new List<KeyValuePair<SimPlayer, SimPlayer>>();
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Players.Count; i++)
                for (int j = 0; j < Players.Count; j++)
                {
                    if (j <= i)
                        continue;

                    var player1 = Players[i];
                    var player2 = Players[j];

                    if (Physics.CircleCicleCollision(player1.Circle, player2.Circle))
                        Output.Add(new KeyValuePair<SimPlayer, SimPlayer>(player1, player2));
                }
        }
    }
}
