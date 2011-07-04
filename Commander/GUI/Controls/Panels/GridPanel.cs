namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GridPanel : Panel
    {
        public int NbColumns;
        private float DistanceBetweenTwoChoices;
        private bool RecalculatePositions;


        public GridPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            NbColumns = 3;
            DistanceBetweenTwoChoices = 30;
        }


        public override void AddWidget(string name, PanelWidget widget)
        {
            base.AddWidget(name, widget);

            RecalculatePositions = true;
        }


        public override void RemoveWidget(string name)
        {
            base.RemoveWidget(name);

            RecalculatePositions = true;
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();

            if (RecalculatePositions)
            {
                Vector3 upperLeft = base.GetUpperLeftUsableSpace();
                float initialX = upperLeft.X;
                float distanceColumn = Dimension.Y / NbColumns;

                int columnCounter = 0;

                foreach (var w in Widgets.Values)
                {
                    if (columnCounter != 0)
                        upperLeft.X += distanceColumn;

                    w.Position = upperLeft;

                    columnCounter++;

                    if (columnCounter >= NbColumns)
                        columnCounter = 0;

                    if (columnCounter == 0)
                    {
                        upperLeft.X = initialX;
                        upperLeft.Y += w.Dimension.Y + DistanceBetweenTwoChoices;
                    }
                }

                RecalculatePositions = false;
            }
        }
    }
}
