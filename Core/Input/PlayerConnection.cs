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
            // try to connect
            player.State = PlayerState.Connecting;

            // on windows, no need to open the Guide
            if (Inputs.Target != Utilities.Setting.Xbox360)
            {
                player.State = PlayerState.Connected;
                NotifyPlayerConnected(player);
                return;
            }

            // if the player is already connected, just notify it
            if (player.Profile != null)
            {
                player.State = PlayerState.Connected;
                NotifyPlayerConnected(player);
                return;
            }

            // if the guide is already opened, don't do more
            if (Guide.IsVisible)
                return;

            // open the guide
            Guide.ShowSignIn((Inputs.Target == Utilities.Setting.Xbox360) ? 4 : 1, false);
        }


        public void Disconnect(Player player)
        {
            // try to disconnect
            player.State = PlayerState.Disconnecting;

            // on windows, no need to open the guide to disconnect
            if (Inputs.Target != Utilities.Setting.Xbox360)
            {
                player.State = PlayerState.Disconnected;
                NotifyPlayerDisconnected(player);
                return;
            }

            // if the guide is already opened, don't do more
            if (Guide.IsVisible)
                return;

            // open the guide
            Guide.ShowSignIn((Inputs.Target == Utilities.Setting.Xbox360) ? 4 : 1, false);
        }


        private void DoPlayerConnected(object sender, SignedInEventArgs e)
        {
            Player player = Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex);
            player.Profile = e.Gamer;

            if (player.State == PlayerState.Connecting)
            {
                player.State = PlayerState.Connected;

                NotifyPlayerConnected(Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex));
            }
        }


        private void DoPlayerDisconnected(object sender, SignedOutEventArgs e)
        {
            Player player = Inputs.PlayersController.GetPlayer(e.Gamer.PlayerIndex);
            player.State = PlayerState.Disconnected;

            NotifyPlayerDisconnected(player);
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
