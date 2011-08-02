namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    abstract class PanelWidget : IVisual
    {
        public PanelWidgetHandler ClickHandler;
        public PanelWidgetHandler HoverHandler;

        public abstract double VisualPriority   { get; set; }
        public abstract Vector3 Position        { get; set; }
        public abstract Vector3 Dimension       { get; set; }
        public abstract byte Alpha              { get; set; }
        public virtual Scene Scene              { get; set; }
        public bool Sticky                      { get; set; }
        public string Name                      { get; set; }


        public bool DoClick(Circle circle)
        {
            if (Click(circle))
            {
                if (ClickHandler != null)
                    ClickHandler(this);

                return true;
            }

            return false;
        }


        public bool DoHover(Circle circle)
        {
            if (Hover(circle))
            {
                if (HoverHandler != null)
                    HoverHandler(this);

                return true;
            }

            return false;
        }


        protected abstract bool Click(Circle circle);
        protected abstract bool Hover(Circle circle);
        public abstract void Draw();
        public abstract void Fade(int from, int to, double length);


        public virtual void Initialize() { }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
