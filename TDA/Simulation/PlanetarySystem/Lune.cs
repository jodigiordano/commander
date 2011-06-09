namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    abstract class Lune
    {
        protected Simulation Simulation;
        protected CorpsCeleste CorpsCeleste;
        protected Matrix MatriceRotation;
        protected Vector3 Position;
        protected Vector3 PositionRelative;
        protected double TempsRotation;
        protected double TempsRotationActuel;
        public Image Representation;
        protected bool SensInverse;


        public Lune(Simulation simulation, CorpsCeleste corpsCeleste, int alpha)
        {
            Simulation = simulation;
            CorpsCeleste = corpsCeleste;

            Representation = new Image("lune" + Main.Random.Next(1, 5))
            {
                SizeX = Main.Random.Next(2, 4),
                VisualPriority = CorpsCeleste.PrioriteAffichage + 0.000001f,
            };

            Representation.Color.A = (byte) alpha;

            SensInverse = Main.Random.Next(0, 2) == 0;
            TempsRotation = Main.Random.Next(3000, 10000);
            TempsRotationActuel = Main.Random.Next(0, (int)TempsRotation);
        }


        public virtual void Update(GameTime gameTime)
        {
            if (SensInverse)
            {
                TempsRotationActuel -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (TempsRotationActuel < 0)
                    TempsRotationActuel = TempsRotation + TempsRotationActuel;
            }

            else
            {
                TempsRotationActuel += gameTime.ElapsedGameTime.TotalMilliseconds;
                TempsRotationActuel %= TempsRotation;
            }
        }


        public void Show()
        {
            Simulation.Scene.Add(Representation);
        }


        public void Hide()
        {
            Simulation.Scene.Remove(Representation);
        }


        public void Draw()
        {
            Representation.Position = this.Position;
        }
    }
}
