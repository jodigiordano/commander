namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class GUIPausePlayer
    {
        public SpaceshipCursor Cursor;

        private Simulator Simulator;
        private SimPlayer InnerPlayer;


        public GUIPausePlayer(Simulator simulator, SimPlayer innerPlayer)
        {
            Simulator = simulator;

            InnerPlayer = innerPlayer;

            Cursor = new SpaceshipCursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.PlayerPanelCursor, InnerPlayer.Color, InnerPlayer.ImageName, false);
        }


        public bool Visible
        {
            get { return Cursor.Visible; }
        }


        public void Sync()
        {
            InnerPlayer.PausePlayer.Position = InnerPlayer.Position;
            InnerPlayer.PausePlayer.Direction = InnerPlayer.Direction;

            Cursor.Position = InnerPlayer.Position;
            Cursor.Direction = InnerPlayer.Direction;
        }


        public void Draw()
        {
            if (Cursor.Alpha == 0)
                return;

            Cursor.Draw();
        }
    }
}
