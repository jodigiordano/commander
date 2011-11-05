namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HorizontalPanel : Panel
    {
        public float DistanceBetweenTwoChoices;


        public HorizontalPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            DistanceBetweenTwoChoices = 30;
            RecomputePositions = true;
        }


        protected override void ComputePositions()
        {
            base.ComputePositions();

            Vector3 upperLeft = base.GetUpperLeftUsableSpace();

            foreach (var w in Widgets)
            {
                w.Value.Position = upperLeft;

                upperLeft.X += w.Value.Dimension.X + DistanceBetweenTwoChoices;
            }
        }
    }
}
