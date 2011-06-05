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

    class LuneTrajet : Lune
    {
        private Trajet3D Trajet;
        private double TempsDevantDebut;
        private double TempsDevantFin;

        public LuneTrajet(Simulation simulation, CorpsCeleste corpsCeleste)
            : base(simulation, corpsCeleste)
        {
            PositionRelative = new Vector3(corpsCeleste.Cercle.Radius + Main.Random.Next(10, 30), 0, 0);
            
            float PositionRelativeY = Main.Random.Next(10, 30);

            Vector3 PositionRelativeInverse = PositionRelative;
            PositionRelativeInverse.X = -PositionRelativeInverse.X;

            double step = TempsRotation / 6;

            Trajet = new Trajet3D
            (
                new List<Vector3>()
                {
                    PositionRelative + new Vector3(0, -PositionRelativeY, 0),
                    PositionRelative + new Vector3(30, 0, 0),
                    PositionRelative + new Vector3(0, PositionRelativeY, 0),
                    PositionRelativeInverse + new Vector3(0, PositionRelativeY, 0),
                    PositionRelativeInverse + new Vector3(-30, 0, 0),
                    PositionRelativeInverse + new Vector3(0, -PositionRelativeY, 0),
                    PositionRelative + new Vector3(0, -PositionRelativeY, 0)
                },
                new List<double>()
                {
                    step * 0,
                    step * 1,
                    step * 2,
                    step * 3,
                    step * 4,
                    step * 5,
                    step * 6
                }
            );

            TempsDevantDebut = step * 1;
            TempsDevantFin = step * 4;
            Representation.VisualPriority = (TempsRotationActuel >= TempsDevantDebut && TempsRotationActuel <= TempsDevantFin) ?
                Preferences.PrioriteSimulationTourelle - 0.01f :
                CorpsCeleste.PrioriteAffichage + 0.01f;

            float rotation = MathHelper.ToRadians(Main.Random.Next(0, 180));

            Matrix.CreateRotationZ(rotation, out MatriceRotation);

            Trajet.Position(TempsRotationActuel, ref Position);
            Vector3.Transform(ref Position, ref MatriceRotation, out Position);
            Vector3.Add(ref Position, ref CorpsCeleste.position, out Position);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Representation.VisualPriority = (TempsRotationActuel >= TempsDevantDebut && TempsRotationActuel <= TempsDevantFin) ?
                Preferences.PrioriteSimulationTourelle - 0.01f :
                CorpsCeleste.PrioriteAffichage + 0.01f;

            Trajet.Position(TempsRotationActuel, ref Position);
            Vector3.Transform(ref Position, ref MatriceRotation, out Position);
            Vector3.Add(ref Position, ref CorpsCeleste.position, out Position);
        }
    }
}
