namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SlideshowPanel : Panel
    {
        protected NumericHorizontalSlider Slider;
        private List<Panel> Panels;


        public SlideshowPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            Slider = new NumericHorizontalSlider("", 0, 0, 0, 1, 0, 200)
            {
                Scene = scene
            };

            Slider.Initialize();
            Slider.VisualPriority = visualPriority;

            Panels = new List<Panel>();
        }


        public override void AddWidget(string name, PanelWidget widget)
        {
            Panels.Add((Panel) widget);

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


        protected override bool Hover(Circle circle)
        {
            if (ShowCloseButton && CloseButton.DoHover(circle))
            {
                LastHoverWidget = CloseButton;
                return true;
            }


            if (Slider.DoHover(circle))
            {
                LastHoverWidget = Slider;

                Sticky = true;
                return true;
            }


            if (Panels[Slider.Value].DoHover(circle))
            {
                //By default, a panel do not care about hover
                LastHoverWidget = Panels[Slider.Value].LastHoverWidget;

                //Sticky = true;
                return true;
            }

            Sticky = false;

            return false;
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
            Slider.Position = base.GetUpperLeftUsableSpace() + new Vector3(Dimension.X / 2 - Slider.Dimension.X / 2, 0, 0);

            Vector3 upperLeft = base.GetUpperLeftUsableSpace() + new Vector3(0, Slider.Dimension.Y + 30, 0);

            foreach (var w in Widgets)
                w.Value.Position = upperLeft;
        }
    }
}
