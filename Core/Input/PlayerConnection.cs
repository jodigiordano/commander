namespace Core.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using System;
    

    public delegate void ConnectHandler(PlayerIndex index, SignedInGamer gamer);

    public class PlayerConnection
    {
        public event ConnectHandler PlayerConnected;
        public event ConnectHandler PlayerDisconnected;


        public PlayerConnection()
        {
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(playerConnected);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(playerDisconnected);
        }


        public void Connect(PlayerIndex player)
        {
#if WINDOWS
            notifyPlayerConnected(player, null);
            return;
#endif

            if (Guide.IsVisible)
                return;

            if (SignedInGamer.SignedInGamers[player] == null)
                Guide.ShowSignIn(1, false);
        }


        private void playerConnected(object sender, SignedInEventArgs e)
        {
            notifyPlayerConnected(e.Gamer.PlayerIndex, e.Gamer);
        }


        private void playerDisconnected(object sender, SignedOutEventArgs e)
        {
            notifyPlayerDisconnected(e.Gamer.PlayerIndex, e.Gamer);
        }


        private void notifyPlayerConnected(PlayerIndex index, SignedInGamer gamer)
        {
            if (PlayerConnected != null)
                PlayerConnected(index, gamer);
        }


        private void notifyPlayerDisconnected(PlayerIndex index, SignedInGamer gamer)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(index, gamer);
        }
    }
}
