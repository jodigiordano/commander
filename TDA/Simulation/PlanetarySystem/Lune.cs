namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using ProjectMercury.Modifiers;

    abstract class Lune : DrawableGameComponent
    {
        protected Simulation Simulation;
        protected CorpsCeleste CorpsCeleste;
        protected Matrix MatriceRotation;
        protected Vector3 Position;
        protected Vector3 PositionRelative;
        protected double TempsRotation;
        protected double TempsRotationActuel;
        public IVisible Representation;
        protected bool SensInverse;

        public Lune(Simulation simulation, CorpsCeleste corpsCeleste)
            : base(simulation.Main)
        {
            Simulation = simulation;
            CorpsCeleste = corpsCeleste;

            Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("lune" + Main.Random.Next(1, 5)), Vector3.Zero);
            Representation.Taille = Main.Random.Next(2, 4);
            Representation.Origine = Representation.Centre;
            Representation.VisualPriority = CorpsCeleste.PrioriteAffichage + 0.000001f;
            Representation.Couleur.A = 100;

            SensInverse = Main.Random.Next(0, 2) == 0;
            TempsRotation = Main.Random.Next(3000, 10000);
            TempsRotationActuel = Main.Random.Next(0, (int)TempsRotation);
        }

        public override void Update(GameTime gameTime)
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


        public override void Draw(GameTime gameTime)
        {
            Representation.Position = this.Position;

            Simulation.Scene.ajouterScenable(Representation);
        }


    }
}
