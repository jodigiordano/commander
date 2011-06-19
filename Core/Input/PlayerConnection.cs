namespace EphemereGames.Core.Input
{
    using System;
    using Microsoft.Xna.Framework.GamerServices;
    

    internal delegate void PlayerHandler(Player p);


    class PlayerConnection
    {
        public event PlayerHandler PlayerConnected;
        public event PlayerHandler PlayerDisconnected;


        public PlayerConnection()
        {
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(DoPlayerConnected);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(DoPlayerDisconnected);
        }


        public void Connect(Player player)
        {
#if WINDOWS
            NotifyPlayerConnected(player);
            return;
#endif

            if (Guide.IsVisible)
                return;

            if (SignedInGamer.SignedInGamers[player.Index] == null)
                Guide.ShowSignIn(1, false);
        }


        public void Disconnect(Player player)
        {
#if WINDOWS
            NotifyPlayerDisconnected(player);
            return;
#endif

            if (Guide.IsVisible)
                return;

            Guide.ShowSignIn(1, false);
        }


        private void DoPlayerConnected(object sender, SignedInEventArgs e)
        {
            Player player = Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex);
            player.Profile = e.Gamer;

            NotifyPlayerConnected(Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex));
        }


        private void DoPlayerDisconnected(object sender, SignedOutEventArgs e)
        {
            NotifyPlayerDisconnected(Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex));
        }


        private void NotifyPlayerConnected(Player player)
        {
            if (PlayerConnected != null)
                PlayerConnected(player);
        }


        private void NotifyPlayerDisconnected(Player player)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(player);
        }
    }
}
