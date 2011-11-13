namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    abstract class AssetsPanel : SlideshowPanel
    {
        protected int ColumnsCount;
        protected int AssetsPerPanel;

        public Simulator Simulator { get; set; }


        public AssetsPanel(string name, Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle(name);
            Slider.SpaceForLabel = 100;
            Slider.SetLabel("Set");

            Alpha = 0;

            ColumnsCount = 4;
            AssetsPerPanel = 16;
        }


        public override void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            var assets = GetAssets();

            for (int i = 0; i < Math.Ceiling(assets.Count / (float)AssetsPerPanel); i++)
                AddWidget("panel" + i, CreateSubPanel(assets, i * AssetsPerPanel, Math.Min(i * AssetsPerPanel + (AssetsPerPanel - 1), assets.Count - 1)));

            Sync();
        }


        public new PanelWidget GetWidgetByName(string name)
        {
            foreach (var p in Widgets)
                foreach (var w in ((GridPanel) p.Value).Widgets)
                    if (w.Key == name)
                        return w.Value;

            return null;
        }


        public virtual void Sync() { }


        private GridPanel CreateSubPanel(List<string> assets, int begin, int end)
        {
            GridPanel panel = new GridPanel(Simulator.Scene, Vector3.Zero, Size, VisualPriority, Color.White)
            {
                NbColumns = ColumnsCount,
                OnlyShowWidgets = true
            };

            for (int i = begin; i <= end; i++)
            {
                var name = assets[i];
                var widget = GetWidget(name);
                widget.Sticky = true;
                widget.ClickHandler = DoClick;
                panel.AddWidget(name, widget);
            }

            return panel;
        }
        

        protected abstract List<string> GetAssets();
        protected abstract PanelWidget GetWidget(string assetName);
        protected abstract void DoClick(PanelWidget widget, Commander.Player player);
    }
}
