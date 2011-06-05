namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class Path
    {
        private const int MaxVisibleLines = 300;
        private const int MaxCurvePoints = 300;
        private const int CelestialBodyDistance = 70;

        public List<CorpsCeleste> CorpsCelestes;
        public double Length;

        private Trajet3D Trajet;
        private List<double> Temps;
        private List<Vector3> SousPoints;
        private SortedList<int, CorpsCeleste> CorpsCelestesChemin;
        private int DistanceDeuxPoints = 40;
        private Image[] Lignes;
        private Scene Scene;
        private int NbSousPoints;


        public Path(Simulation simulation, Color couleur, TypeBlend melange)
        {
            Scene = simulation.Scene;
            
            Trajet = new Trajet3D();
            Temps = new List<double>(MaxCurvePoints);
            SousPoints = new List<Vector3>(MaxCurvePoints);

            for(int i = 0; i < MaxCurvePoints; i++)
            {
                Temps.Add(0);
                SousPoints.Add(Vector3.Zero);
            }

            NbSousPoints = 0;
            Length = 0;

            Lignes = new Image[MaxVisibleLines];

            for (int i = 0; i < MaxVisibleLines; i++)
            {
                Image line = new Image("LigneTrajet", Vector3.Zero);
                line.Color = couleur;
                line.Blend = melange;
                line.VisualPriority = Preferences.PrioriteSimulationChemin;
                line.SizeX = 1.5f;

                Lignes[i] = line;
            }
        }


        public void Initialize()
        {
            CorpsCelestesChemin = new SortedList<int, CorpsCeleste>();

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                CorpsCeleste corps = CorpsCelestes[i];

                if (corps.Priorite == -1)
                    continue;

                for (int j = 0; j < corps.Turrets.Count; j++)
                    if (corps.Turrets[j].Type == TurretType.Gravitational)
                        ajouterCorpsCeleste(corps);
            }

            recalculerTrajet();
        }


        public byte AlphaChannel
        {
            set
            {
                for (int i = 0; i < MaxVisibleLines; i++)
                    Lignes[i].Color.A = value;
            }
        }


        public CorpsCeleste PremierRelais
        {
            get
            {
                return CorpsCelestesChemin.Values[0];
            }
        }


        public CorpsCeleste DernierRelais
        {
            get
            {
                return (CorpsCelestesChemin.Count == 0) ? null : CorpsCelestesChemin.Values[CorpsCelestesChemin.Count - 1];
            }
        }


        public bool contientCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            return CorpsCelestesChemin.ContainsKey(corpsCeleste.Priorite);
        }


        public void enleverCorpsCeleste(CorpsCeleste corpsCeleste)
        {
            DernierRelais.EstDernierRelais = false;

            CorpsCelestesChemin.Remove(corpsCeleste.Priorite);

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

                CorpsCelestesChemin.Add(corpsCeleste.Priorite, corpsCeleste);

                DernierRelais.EstDernierRelais = true;
            }

            else
                CorpsCelestesChemin.Add(corpsCeleste.Priorite, corpsCeleste);
        }


        public CorpsCeleste prochainRelais(CorpsCeleste relaisActuel)
        {
            int indice = CorpsCelestesChemin.IndexOfValue(relaisActuel);

            CorpsCeleste prochainRelais = (indice == CorpsCelestesChemin.Count - 1) ? relaisActuel : CorpsCelestesChemin.Values[indice + 1];

            return prochainRelais;
        }


        public void Position(double deplacement, ref Vector3 position)
        {
            Trajet.Position(deplacement, ref position);
        }


        public float Pourc(double deplacement)
        {
            return Trajet.Pourc(deplacement);
        }


        public void Update(GameTime gameTime)
        {
            recalculerTrajet();
        }

        private void recalculerTrajet()
        {
            mettreAJourSousPoints();

            Length = 0;
            Temps[0] = Length;

            for (int i = 1; i < NbSousPoints; i++)
            {
                _vecteur1 = Vector3.Subtract(SousPoints[i], SousPoints[i - 1]);
                Length += _vecteur1.Length();
                Temps[i] = Length;
            }

            Trajet.Initialize(SousPoints, Temps, NbSousPoints);
        }


        public void Draw()
        {
            int nbLines = (int) (Length / DistanceDeuxPoints) + 1;

            for (int j = 0; j < nbLines; j++)
            {
                Trajet.Position(j * DistanceDeuxPoints, ref Lignes[j].position);

                Lignes[j].Rotation = Trajet.rotation(j * DistanceDeuxPoints);
                Lignes[j].Color.G = Lignes[j].Color.B = (byte) (255 * (1 - (((float)j + 1) / nbLines)));
                Lignes[j].VisualPriority = Preferences.PrioriteSimulationChemin + Trajet.Pourc(j * DistanceDeuxPoints) / 1000f;
                Scene.ajouterScenable(Lignes[j]);

                if (j >= MaxVisibleLines)
                    break;
            }
        }


        private Vector3 _vecteur1, _vecteur2;
        private Matrix _matrice1;

        private void mettreAJourSousPoints()
        {
            NbSousPoints = 0;

            for (int i = 0; i < CorpsCelestesChemin.Count; i++)
            {
                if (i == 0 || i == CorpsCelestesChemin.Count - 1)
                {
                    SousPoints[NbSousPoints++] = CorpsCelestesChemin.Values[i].Position;
                    continue;
                }

                // Ligne
                Ligne.PointPlusProche
                    (
                        ref CorpsCelestesChemin.Values[i - 1].position,
                        ref CorpsCelestesChemin.Values[i + 1].position,
                        ref CorpsCelestesChemin.Values[i].position,
                        ref _vecteur1
                    );

                // Vecteur perpendiculaire
                Vector3.Subtract(ref CorpsCelestesChemin.Values[i].position, ref _vecteur1, out _vecteur1);

                //Radius
                //float radius = CorpsCelestesChemin.Values[i].TurretsZone.Radius + CelestialBodyDistance;
                float radius = MathHelper.Min(_vecteur1.Length(), CorpsCelestesChemin.Values[i].TurretsZone.Radius + CelestialBodyDistance);

                //Entry point
                Vector3 vecPO = CorpsCelestesChemin.Values[i].position - CorpsCelestesChemin.Values[i - 1].position;
                float POangle = Core.Physique.Utilities.VectorToAngle(ref vecPO);
                POangle += MathHelper.PiOver2;
                Vector3 entryVec = CorpsCelestesChemin.Values[i].position +
                    Core.Physique.Utilities.AngleToVector(POangle) * radius;

                //Exit point
                Vector3 vecPD = CorpsCelestesChemin.Values[i].position - CorpsCelestesChemin.Values[i + 1].position;
                float PDangle = Core.Physique.Utilities.VectorToAngle(ref vecPD);
                PDangle -= MathHelper.PiOver2;
                Vector3 exitVec = CorpsCelestesChemin.Values[i].position +
                    Core.Physique.Utilities.AngleToVector(PDangle) * radius;

                SousPoints[NbSousPoints++] = entryVec;

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

                    SousPoints[NbSousPoints++] = CorpsCelestesChemin.Values[i].position +
                    Core.Physique.Utilities.AngleToVector(next) * radius;
                }

                SousPoints[NbSousPoints++] = exitVec;

                if (CorpsCelestesChemin.Values[i].ContientTourelleGravitationnelleNiveau2)
                {
                    step = MathHelper.TwoPi / 8;

                    for (int y = 1; y < 8; y++)
                    {
                        float next = PDangle - y * step;

                        if (next < 0)
                            next += MathHelper.TwoPi;

                        SousPoints[NbSousPoints++] = CorpsCelestesChemin.Values[i].position +
                        Core.Physique.Utilities.AngleToVector(next) * radius;
                    }

                    SousPoints[NbSousPoints++] = exitVec;
                }
            }
        }
    }
}
