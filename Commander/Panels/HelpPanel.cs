namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpPanel : VerticalPanel
    {
        public HelpPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("How to play");

            Alpha = 0;
        }
    }
}
