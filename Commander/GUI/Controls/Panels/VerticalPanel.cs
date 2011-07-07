namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VerticalPanel : Panel
    {
        public float DistanceBetweenTwoChoices;

        protected bool RecalculatePositions;


        public VerticalPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            DistanceBetweenTwoChoices = 30;
            RecalculatePositions = true;
        }


        public override void AddWidget(string name, PanelWidget widget)
        {
            base.AddWidget(name, widget);

            RecalculatePositions = true;
        }


        public override Vector3 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;

                RecalculatePositions = true;
            }
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            if (RecalculatePositions)
            {
                Vector3 upperLeft = base.GetUpperLeftUsableSpace();

                foreach (var w in Widgets.Values)
                {
                    w.Position = upperLeft;

                    upperLeft.Y += w.Dimension.Y + DistanceBetweenTwoChoices;
                }

                RecalculatePositions = false;
            }

            base.Draw();


        }
    }
}
