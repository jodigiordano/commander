namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VerticalPanel : Panel
    {
        public float DistanceBetweenTwoChoices;
        public bool CenterWidgets;


        public VerticalPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            DistanceBetweenTwoChoices = 30;
            CenterWidgets = false;
        }


        protected override void ComputePositions()
        {
            base.ComputePositions();

            Vector3 upperLeft = base.GetUpperLeftUsableSpace();

            foreach (var w in Widgets)
            {
                w.Value.Position = upperLeft;

                if (CenterWidgets)
                    w.Value.Position += new Vector3(Dimension.X / 2 - w.Value.Dimension.X / 2, 0, 0);

                upperLeft.Y += w.Value.Dimension.Y + DistanceBetweenTwoChoices;
            }
        }
    }
}
