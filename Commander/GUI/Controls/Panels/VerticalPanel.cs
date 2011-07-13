namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VerticalPanel : Panel
    {
        public float DistanceBetweenTwoChoices;


        public VerticalPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            DistanceBetweenTwoChoices = 30;
        }


        protected override void ComputePositions()
        {
            Vector3 upperLeft = base.GetUpperLeftUsableSpace();

            foreach (var w in Widgets)
            {
                w.Value.Position = upperLeft;

                upperLeft.Y += w.Value.Dimension.Y + DistanceBetweenTwoChoices;
            }
        }
    }
}
