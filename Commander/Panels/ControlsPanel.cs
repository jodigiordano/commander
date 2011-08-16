namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ControlsPanel : VerticalPanel
    {
        public ControlsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Controls");

            Alpha = 0;
        }
    }
}
