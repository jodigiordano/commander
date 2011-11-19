namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GridPanel : Panel
    {
        public int NbColumns;
        public float DistanceBetweenTwoChoices;


        public GridPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            NbColumns = 3;
            DistanceBetweenTwoChoices = 30;
        }


        protected override void ComputePositions()
        {
            base.ComputePositions();

            Vector3 upperLeft = base.GetUpperLeftUsableSpace();
            float initialX = upperLeft.X;
            float distanceColumn = Dimension.X / NbColumns;

            int columnCounter = 0;

            foreach (var w in Widgets)
            {
                if (columnCounter != 0)
                    upperLeft.X += distanceColumn;

                w.Value.Position = upperLeft;

                columnCounter++;

                if (columnCounter >= NbColumns)
                    columnCounter = 0;

                if (columnCounter == 0)
                {
                    upperLeft.X = initialX;
                    upperLeft.Y += w.Value.Dimension.Y + DistanceBetweenTwoChoices;
                }
            }
        }
    }
}
