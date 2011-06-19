namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;

    class LiteSimulator
    {
        public LevelDescriptor Descripteur;

        private class CorpsCelesteLite
        {
            public Vector3 Position;
            public Vector3 Offset;
            public float Rotation;
            public Size Taille;
            public double TempsRotation;

            private Vector3 PositionBase;
            public double TempsDepart;
            public Circle Cercle;
            public double TempsRotationActuel;
            private Matrix MatriceRotation;

            public CorpsCelesteLite(Vector3 position, Vector3 offset, float rotation, int positionDepart, Size taille, double vitesse)
            {
                Position = position;
                Offset = offset;
                Rotation = MathHelper.ToRadians(rotation);
                Taille = taille;
                TempsRotation = vitesse;

                PositionBase = Position;
                TempsDepart = TempsRotation * (positionDepart / 100.0f);
                Cercle = new Circle(PositionBase, (int)Taille);
                TempsRotationActuel = TempsRotation * (positionDepart / 100.0f);
                Matrix.CreateRotationZ(Rotation, out MatriceRotation);
            }

            public void deplacer()
            {
                if (TempsRotation == 0)
                    return;

                Position.X = PositionBase.X * (float)Math.Cos((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);
                Position.Y = PositionBase.Y * (float)Math.Sin((MathHelper.TwoPi / TempsRotation) * TempsRotationActuel);

                Vector3.Transform(ref Position, ref MatriceRotation, out Position);
                Vector3.Add(ref Position, ref Offset, out Position);

                Cercle.Position = Position;
            }
        }

        private List<CorpsCelesteLite> CorpsCelestes;

        public LiteSimulator()
        {
            CorpsCelestes = new List<CorpsCelesteLite>();
        }

        public void Initialize()
        {
            CorpsCelestes.Clear();

            foreach (var descCorps in Descripteur.PlanetarySystem)
            {
                CorpsCelestes.Add(
                    new CorpsCelesteLite(
                        descCorps.Position,
                        descCorps.Offset,
                        descCorps.Rotation,
                        descCorps.StartingPosition,
                        descCorps.Size,
                        descCorps.Speed));
            }
        }

        public bool dansLesBornes(CelestialBodyDescriptor descripteur, RectanglePhysique cadre)
        {
            CorpsCelesteLite corpsCeleste = new CorpsCelesteLite
            (
                descripteur.Position,
                descripteur.Offset,
                descripteur.Rotation,
                descripteur.StartingPosition,
                descripteur.Size,
                descripteur.Speed
            );

            // au temps 0
            corpsCeleste.TempsRotationActuel = 0;
            corpsCeleste.deplacer();
            if (!cadre.Includes(corpsCeleste.Position + new Vector3((int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(-(int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, (int)descripteur.Size, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, -(int)descripteur.Size, 0)))
                return false;

            // au temps 1/4
            corpsCeleste.TempsRotationActuel = corpsCeleste.TempsRotation / 4;
            corpsCeleste.deplacer();
            if (!cadre.Includes(corpsCeleste.Position + new Vector3((int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(-(int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, (int)descripteur.Size, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, -(int)descripteur.Size, 0)))
                return false;

            // au temps 1/2
            corpsCeleste.TempsRotationActuel = corpsCeleste.TempsRotation / 2;
            corpsCeleste.deplacer();
            if (!cadre.Includes(corpsCeleste.Position + new Vector3((int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(-(int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, (int)descripteur.Size, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, -(int)descripteur.Size, 0)))
                return false;

            // au temps 3/4
            corpsCeleste.TempsRotationActuel = corpsCeleste.TempsRotation * (3.0/4.0);
            corpsCeleste.deplacer();
            if (!cadre.Includes(corpsCeleste.Position + new Vector3((int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(-(int)descripteur.Size, 0, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, (int)descripteur.Size, 0)) ||
                !cadre.Includes(corpsCeleste.Position + new Vector3(0, -(int)descripteur.Size, 0)))
                return false;

            return true;
        }


        public List<Vector3> pointsConsideres = new List<Vector3>();
        public List<Circle> cerclesConsideres = new List<Circle>();

        public bool collisionPlanetaire(CelestialBodyDescriptor descripteur, List<CelestialBodyDescriptor> autres)
        {
            CorpsCelesteLite corpsCeleste = new CorpsCelesteLite
            (
                descripteur.Position,
                descripteur.Offset,
                descripteur.Rotation,
                descripteur.StartingPosition,
                descripteur.Size,
                descripteur.Speed
            );

            List<CorpsCelesteLite> autresCorpsCelestes = new List<CorpsCelesteLite>();

            foreach (var corps in autres)
                autresCorpsCelestes.Add(
                    new CorpsCelesteLite(
                        corps.Position,
                        corps.Offset,
                        corps.Rotation,
                        corps.StartingPosition,
                        corps.Size,
                        corps.Speed
                    ));

            pointsConsideres.Clear();
            cerclesConsideres.Clear();

            for (int i = 0; i < autresCorpsCelestes.Count; i++)
            {
                CorpsCelesteLite autre = autresCorpsCelestes[i];

                double tempsRotation = Math.Max(corpsCeleste.TempsRotation, autre.TempsRotation);

                for (double x = 0; x <= tempsRotation; x += 250)
                {
                    corpsCeleste.TempsRotationActuel = (corpsCeleste.TempsDepart + x) % corpsCeleste.TempsRotation;
                    corpsCeleste.deplacer();

                    autre.TempsRotationActuel = (autre.TempsDepart + x) % autre.TempsRotation;
                    autre.deplacer();

                    pointsConsideres.Add(corpsCeleste.Position);
                    pointsConsideres.Add(autre.Position);
                    cerclesConsideres.Add(new Circle(corpsCeleste.Cercle.Position, corpsCeleste.Cercle.Radius));
                    cerclesConsideres.Add(new Circle(autre.Cercle.Position, autre.Cercle.Radius));

                    if (corpsCeleste.Cercle.Intersects(autre.Cercle))
                        return true;
                }
            }

            return false;
        }
    }
}
