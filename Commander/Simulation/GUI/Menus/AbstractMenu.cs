namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;

    abstract class AbstractMenu
    {
        public Vector3 Position;
        private Vector2 Taille;
        protected Simulation Simulation;

        public Bubble Bulle;


        public AbstractMenu(Simulation simulation)
        {
            this.Simulation = simulation;

            this.Taille = Vector2.Zero;

            Bulle = new Bubble(Simulation, new Rectangle(), Preferences.PrioriteGUIPanneauCorpsCeleste + 0.05f);
        }


        public virtual void Draw()
        {
            this.Position = BasePosition;
            this.Taille = MenuSize;

            bool tropADroite = Position.X + this.Taille.X + 50 > 640 - Preferences.DeadZoneXbox.X;
            bool tropBas = Position.Y + this.Taille.Y > 370 - Preferences.DeadZoneXbox.Y;

            if (tropADroite && tropBas)
            {
                this.Position += new Vector3(-this.Taille.X - 50, -this.Taille.Y - 10, 0);
                Bulle.BlaPosition = 2;
            }

            else if (tropADroite)
            {
                this.Position += new Vector3(-this.Taille.X - 50, 0, 0);
                Bulle.BlaPosition = 1;
            }

            else if (tropBas)
            {
                this.Position += new Vector3(0, -this.Taille.Y - 50, 0);
                Bulle.BlaPosition = 3;
            }

            else
            {
                this.Position += new Vector3(50, -10, 0);
                Bulle.BlaPosition = 0;
            }

            Bulle.Dimension = new Rectangle((int)Position.X, (int)Position.Y, (int)Taille.X, (int)Taille.Y);
        }

        protected abstract Vector2 MenuSize { get; }
        protected abstract Vector3 BasePosition { get; }
    }
}
