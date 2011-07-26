namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SlideshowPanel : Panel
    {
        protected NumericHorizontalSlider Slider;
        private List<PanelWidget> Panels;


        public SlideshowPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            Slider = new NumericHorizontalSlider("", 0, 0, 0, 1, 200)
            {
                Scene = scene,
                VisualPriority = visualPriority
            };

            Panels = new List<PanelWidget>();
        }


        public override void AddWidget(string name, PanelWidget widget)
        {
            Panels.Add(widget);

            Slider.Max = Panels.Count - 1;

            base.AddWidget(name, widget);
        }


        public override void ClearWidgets()
        {
            Panels.Clear();

            Slider.Max = 0;

            base.ClearWidgets();
        }


        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();
            Slider.Draw();
        }


        public override void Fade(int from, int to, double length)
        {
            Slider.Fade(from, to, length);

            base.Fade(from, to, length);
        }


        protected override bool Click(Circle circle)
        {
            if (Slider.DoClick(circle))
                return true;

            return base.Click(circle);
        }


        protected override void DrawWidgets()
        {
            Panels[Slider.Value].Draw();
        }


        protected override bool ClickWidgets(Circle circle)
        {
            return Panels[Slider.Value].DoClick(circle);
        }


        protected override void ComputePositions()
        {
            Slider.Position = base.GetUpperLeftUsableSpace();

            Vector3 upperLeft = base.GetUpperLeftUsableSpace() + new Vector3(0, Slider.Dimension.Y, 0);

            foreach (var w in Widgets)
                w.Value.Position = upperLeft;
        }
    }
}
