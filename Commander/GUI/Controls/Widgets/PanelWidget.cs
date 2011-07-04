namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    abstract class PanelWidget
    {
        public PanelWidgetHandler Handler;

        public abstract double VisualPriority   { get; set; }
        public abstract Vector3 Position        { get; set; }
        public abstract Vector3 Dimension       { get; set; }

        public virtual Scene Scene              { get; set; }


        public bool DoClick(Circle circle)
        {
            if (Click(circle))
            {
                if (Handler != null)
                    Handler(this);

                return true;
            }

            return false;
        }


        protected abstract bool Click(Circle circle);
        public abstract void Draw();
        public abstract void Fade(int from, int to, double length);
    }
}
