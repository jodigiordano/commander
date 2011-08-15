namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CreditsPanel : VerticalPanel
    {
        private Label Jodi;
        private ImageLabel Tag;
        private Label Backgrounds;


        public CreditsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Credits");

            Alpha = 0;

            Jodi = new Label(new Text("Programming & all: Jodi Giordano", "Pixelite") { SizeX = 2 });

            AddWidget("Jodi", Jodi);
        }
    }
}
