namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;


    class GUIPausePlayer
    {
        public SpaceshipCursor Cursor;

        private Simulator Simulator;


        public GUIPausePlayer(Simulator simulator, Color color, string representation, InputType inputType)
        {
            Simulator = simulator;

            Cursor = new SpaceshipCursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.PlayerPanelCursor, color, representation, false);
            Cursor.FadeOut();
        }


        public void Draw()
        {
            if (Cursor.Alpha == 0)
                return;

            Cursor.Draw();
        }
    }
}
