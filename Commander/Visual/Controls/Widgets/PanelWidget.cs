namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    abstract class PanelWidget : IVisual
    {
        public PanelWidgetPlayerHandler ClickHandler;
        public PanelWidgetPlayerHandler HoverHandler;

        public abstract double VisualPriority   { get; set; }
        public abstract Vector3 Position        { get; set; }
        public abstract Vector3 Dimension       { get; set; }
        public abstract byte Alpha              { get; set; }
        public virtual Scene Scene              { get; set; }
        public bool Sticky                      { get; set; }
        public string Name                      { get; set; }
        public bool EnableInput                 { get; set; }


        public PanelWidget()
        {
            Sticky = false;
            Name = "";
            EnableInput = true;
        }


        public bool DoClick(Commander.Player player)
        {
            if (!EnableInput)
                return false;

            if (Click(player))
            {
                if (ClickHandler != null)
                    ClickHandler(this, player);

                return true;
            }

            return false;
        }


        public bool DoHover(Commander.Player player)
        {
            if (!EnableInput)
                return false;

            if (Hover(player))
            {
                if (HoverHandler != null)
                    HoverHandler(this, player);

                return true;
            }

            return false;
        }


        protected abstract bool Click(Commander.Player player);
        protected abstract bool Hover(Commander.Player player);
        public abstract void Draw();
        public abstract void Fade(int from, int to, double length);


        public virtual void Initialize() { }


        Rectangle IVisual.VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Color IVisual.Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
