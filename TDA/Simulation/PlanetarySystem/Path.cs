namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using System;


    class Path : IComparer<CorpsCeleste>
    {
        private const int MaxVisibleLines = 300;
        private const int MaxCurvePoints = 300;
        private const int CelestialBodyDistance = 70;

        public List<CorpsCeleste> CelestialBodies;
        public double Length;

        private Trajet3D InnerPath;
        private List<double> Times;
        private List<Vector3> Points;
        private OrderedBag<CorpsCeleste> CelestialBodiesPath;
        private int DistanceTwoPoints = 40;
        private Image[] Lines;
        private Scene Scene;
        private int NbPoints;
        private int PreviousNbLinesToDraw;


        public Path(Simulation simulation, Color couleur, TypeBlend melange)
        {
            Scene = simulation.Scene;
            
            InnerPath = new Trajet3D();
            Times = new List<double>(MaxCurvePoints);
            Points = new List<Vector3>(MaxCurvePoints);

            for(int i = 0; i < MaxCurvePoints; i++)
            {
                Times.Add(0);
                Points.Add(Vector3.Zero);
            }

            NbPoints = 0;
            Length = 0;

            Lines = new Image[MaxVisibleLines];

            for (int i = 0; i < MaxVisibleLines; i++)
            {
                Image line = new Image("LigneTrajet", Vector3.Zero);
                line.Color = couleur;
                line.Blend = melange;
                line.VisualPriority = Preferences.PrioriteSimulationChemin;
                line.SizeX = 1.5f;

                Lines[i] = line;
            }

            PreviousNbLinesToDraw = 0;
        }


        public void Initialize()
        {
            CelestialBodiesPath = new OrderedBag<CorpsCeleste>(this);

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                CorpsCeleste corps = CelestialBodies[i];

                if (corps.Priorite == -1)
                    continue;

                for (int j = 0; j < corps.Turrets.Count; j++)
                    if (corps.Turrets[j].Type == TurretType.Gravitational)
                        ajouterCorpsCeleste(corps);
            }

            RecalculatePath();
        }


        public byte AlphaChannel
        {
            set
            {
                for (int i = 0; i < MaxVisibleLines; i++)
                    Lines[i].Color.A = value;
            }
        }


        public CorpsCeleste PremierRelais
        {
            get
            {
                return CelestialBodiesPath.GetFirst();
            }
        }


        public CorpsCeleste DernierRelais
        {
            get
            {
                return CelestialBodiesPath.Count == 0 ? null : CelestialBodiesPath.GetLast();
            }
        }


        public bool contientCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            return CelestialBodiesPath.Contains(corpsCeleste);
        }


        public void enleverCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            DernierRelais.EstDernierRelais = false;

            CelestialBodiesPath.Remove(corpsCeleste);

            DernierRelais.EstDernierRelais = true;
        }


        public void ajouterCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            if (contientCorpsCeleste(corpsCeleste))
                return;

            if (corpsCeleste.Priorite == -1)
                return;

            if (!corpsCeleste.ContientTourelleGravitationnelle)
                return;

            if (DernierRelais != null)
            {
                DernierRelais.EstDernierRelais = false;

                CelestialBodiesPath.Add(corpsCeleste);

                DernierRelais.EstDernierRelais = true;
            }

            else
                CelestialBodiesPath.Add(corpsCeleste);
        }


        public void Position(double deplacement, ref Vector3 position)
        {
            InnerPath.Position(deplacement, ref position);
        }


        public float Pourc(double deplacement)
        {
            return InnerPath.Pourc(deplacement);
        }


        public void Direction(double time, out Vector3 result)
        {
            InnerPath.Direction(time, out result);
        }


        public void Update(GameTime gameTime)
        {
            RecalculatePath();
        }


        public void Draw()
        {
            int nbLines = (int) (Length / DistanceTwoPoints) + 1;
            int toAddOrDelete = nbLines - PreviousNbLinesToDraw;
            
            int cnt = Math.Abs(toAddOrDelete);

            for (int i = 0; i < cnt; i++)
                if (toAddOrDelete > 0)
                    Scene.Add(Lines[PreviousNbLinesToDraw + i]);
                else
                    Scene.Remove(Lines[PreviousNbLinesToDraw - i - 1]);

            PreviousNbLinesToDraw = nbLines;
            
            for (int j = 0; j < nbLines; j++)
            {
                InnerPath.Position(j * DistanceTwoPoints, ref Lines[j].position);

                Lines[j].Rotation = InnerPath.rotation(j * DistanceTwoPoints);
                Lines[j].Color.G = Lines[j].Color.B = (byte) (255 * (1 - (((float)j + 1) / nbLines)));
                Lines[j].VisualPriority = Preferences.PrioriteSimulationChemin + InnerPath.Pourc(j * DistanceTwoPoints) / 1000f;

                if (j >= MaxVisibleLines)
                    break;
            }
        }


        private void RecalculatePath()
        {
            mettreAJourSousPoints();

            Length = 0;
            Times[0] = Length;

            for (int i = 1; i < NbPoints; i++)
            {
                _vecteur1 = Vector3.Subtract(Points[i], Points[i - 1]);
                Length += _vecteur1.Length();
                Times[i] = Length;
            }

            InnerPath.Initialize(Points, Times, NbPoints);
        }


        private Vector3 _vecteur1, _vecteur2;
        private Matrix _matrice1;

        private void mettreAJourSousPoints()
        {
            NbPoints = 0;

            for (int i = 0; i < CelestialBodiesPath.Count; i++)
            {
                if (i == 0 || i == CelestialBodiesPath.Count - 1)
                {
                    Points[NbPoints++] = CelestialBodiesPath[i].Position;
                    continue;
                }

                // Ligne
                Ligne.PointPlusProche
                    (
                        ref CelestialBodiesPath[i - 1].position,
                        ref CelestialBodiesPath[i + 1].position,
                        ref CelestialBodiesPath[i].position,
                        ref _vecteur1
                    );

                // Vecteur perpendiculaire
                Vector3.Subtract(ref CelestialBodiesPath[i].position, ref _vecteur1, out _vecteur1);

                //Radius
                //float radius = CorpsCelestesChemin.Values[i].TurretsZone.Radius + CelestialBodyDistance;
                float radius = MathHelper.Min(_vecteur1.Length(), CelestialBodiesPath[i].TurretsZone.Radius + CelestialBodyDistance);

                //Entry point
                Vector3 vecPO = CelestialBodiesPath[i].position - CelestialBodiesPath[i - 1].position;
                float POangle = Core.Physique.Utilities.VectorToAngle(ref vecPO);
                POangle += MathHelper.PiOver2;
                Vector3 entryVec = CelestialBodiesPath[i].position +
                    Core.Physique.Utilities.AngleToVector(POangle) * radius;

                //Exit point
                Vector3 vecPD = CelestialBodiesPath[i].position - CelestialBodiesPath[i + 1].position;
                float PDangle = Core.Physique.Utilities.VectorToAngle(ref vecPD);
                PDangle -= MathHelper.PiOver2;
                Vector3 exitVec = CelestialBodiesPath[i].position +
                    Core.Physique.Utilities.AngleToVector(PDangle) * radius;

                Points[NbPoints++] = entryVec;

                if (POangle < 0)
                    POangle += MathHelper.TwoPi;

                if (PDangle < 0)
                    PDangle += MathHelper.TwoPi;

                float angle = (POangle < PDangle) ? POangle + (MathHelper.TwoPi - PDangle) : POangle - PDangle;
                float step = angle / 4;

                for (int y = 1; y < 4; y++)
                {
                    float next = POangle - y * step;

                    if (next < 0)
                        next += MathHelper.TwoPi;

                    Points[NbPoints++] = CelestialBodiesPath[i].position +
                    Core.Physique.Utilities.AngleToVector(next) * radius;
                }

                Points[NbPoints++] = exitVec;

                if (CelestialBodiesPath[i].ContientTourelleGravitationnelleNiveau2)
                {
                    step = MathHelper.TwoPi / 8;

                    for (int y = 1; y < 8; y++)
                    {
                        float next = PDangle - y * step;

                        if (next < 0)
                            next += MathHelper.TwoPi;

                        Points[NbPoints++] = CelestialBodiesPath[i].position +
                        Core.Physique.Utilities.AngleToVector(next) * radius;
                    }

                    Points[NbPoints++] = exitVec;
                }
            }
        }


        public int Compare(CorpsCeleste c1, CorpsCeleste c2)
        {
            if (c1.Priorite > c2.Priorite)
                return 1;

            if (c1.Priorite < c2.Priorite)
                return -1;

            return 0;
        }
    }
}
