namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using Core.Physique;

    class Chemin
    {
        private const int NB_TRAITS = 300;
        private const int NB_POINTS_MAX = 300;

        public List<CorpsCeleste> CorpsCelestes;

        private Trajet3D Trajet;
        private Vector3[] Positions;
        private double[] Temps;
        private SortedList<int, CorpsCeleste> CorpsCelestesChemin;
        private int DistanceDeuxPoints = 40;
        private IVisible[] Lignes;
        private Scene Scene;
        public double Longueur;

        public Chemin(Simulation simulation, Color couleur, TypeMelange melange)
        {
            Scene = simulation.Scene;

            // Representation visuelle
            Positions = new Vector3[NB_POINTS_MAX];
            Temps = new double[NB_POINTS_MAX];
            Trajet = new Trajet3D();
            Longueur = 0;

            Lignes = new IVisible[NB_TRAITS];

            for (int i = 0; i < NB_TRAITS; i++)
            {
                IVisible ligne = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("LigneTrajet"), Vector3.Zero);
                ligne.PrioriteAffichage = Preferences.PrioriteSimulationChemin;
                ligne.Origine = ligne.Centre;
                ligne.Couleur = couleur;
                ligne.Melange = melange;

                Lignes[i] = ligne;
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

                for (int j = 0; j < corps.Emplacements.Count; j++)
                {
                    Emplacement emplacement = corps.Emplacements[j];

                    if (emplacement.EstOccupe && emplacement.Tourelle.Type == TypeTourelle.Gravitationnelle)
                        ajouterCorpsCeleste(corps);
                }
            }

            recalculerTrajet();
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
            DernierRelais.PrioriteAffichage = DernierRelais.PrioriteAffichageBackup;

            CorpsCelestesChemin.Remove(corpsCeleste.Priorite);

            DernierRelais.EstDernierRelais = true;
            DernierRelais.PrioriteAffichage = Preferences.PrioriteSimulationEnnemi + 0.01f;
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
                DernierRelais.PrioriteAffichage = DernierRelais.PrioriteAffichageBackup;

                CorpsCelestesChemin.Add(corpsCeleste.Priorite, corpsCeleste);

                DernierRelais.EstDernierRelais = true;
                DernierRelais.PrioriteAffichage = Preferences.PrioriteSimulationEnnemi + 0.01f;
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


        public void Update(GameTime gameTime)
        {
            recalculerTrajet();
        }

        private void recalculerTrajet()
        {
            mettreAJourSousPoints();

            //int NbPointsDansTrajet = (CorpsCelestesChemin.Count - 2) * 3 + 2;
            int NbPointsDansTrajet = 0;

            int k = 0;
            for (int i = 0; i < CorpsCelestesChemin.Count; i++)
                if (i == 0 || i == CorpsCelestesChemin.Count - 1)
                {
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[0];
                    NbPointsDansTrajet++;
                }
                else if (CorpsCelestesChemin.Values[i].ContientTourelleGravitationnelleNiveau2)
                {
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[0];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[1];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[2];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[3];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[0];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[1];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[2];

                    NbPointsDansTrajet += 7;
                }
                else
                {
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[0];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[1];
                    Positions[k++] = CorpsCelestesChemin.Values[i].SousPoints[2];

                    NbPointsDansTrajet += 3;
                }

            Longueur = 0;
            Temps[0] = Longueur;

            for (int i = 1; i < NbPointsDansTrajet; i++)
            {
                Vector3.Subtract(ref Positions[i], ref Positions[i - 1], out _vecteur1);

                Longueur += _vecteur1.Length();

                Temps[i] = Longueur;
            }

            Trajet.Initialize(Positions, Temps, NbPointsDansTrajet);
        }


        public void Draw()
        {
            int indiceLignes = 0;

            for (double j = 0; j < Longueur; j += DistanceDeuxPoints)
            {
                Trajet.Position(j, ref Lignes[indiceLignes].position);

                Lignes[indiceLignes].Rotation = Trajet.rotation(j);
                Scene.ajouterScenable(Lignes[indiceLignes]);

                indiceLignes++;

                if (indiceLignes >= NB_TRAITS)
                    break;
            }
        }


        private Vector3 _vecteur1, _vecteur2;
        private Matrix _matrice1;

        private void mettreAJourSousPoints()
        {
            for (int i = 0; i < CorpsCelestesChemin.Count; i++)
            {
                if (i == 0 || i == CorpsCelestesChemin.Count - 1)
                    CorpsCelestesChemin.Values[i].SousPoints[0] = CorpsCelestesChemin.Values[i].Position;

                else
                {
                    // Point le plus proche sur le vecteur qui relie le corps céleste précédent et le suivant
                    Ligne.PointPlusProche
                    (
                        ref CorpsCelestesChemin.Values[i - 1].position,
                        ref CorpsCelestesChemin.Values[i + 1].position,
                        ref CorpsCelestesChemin.Values[i].position,
                        ref _vecteur1
                    );

                    // Vecteur perpendiculaire unitaire
                    Vector3.Subtract(ref CorpsCelestesChemin.Values[i].position, ref _vecteur1, out _vecteur1);

                    _vecteur1.Normalize();

                    if (float.IsNaN(_vecteur1.X))
                        _vecteur1 = Vector3.Zero;

                    //(Bx - Ax) * (Cy - Ay) - (By - Ay) * (Cx - Ax)
                    double coteLigne =
                        (CorpsCelestesChemin.Values[i - 1].position.X - CorpsCelestesChemin.Values[i + 1].position.X) *
                        (CorpsCelestesChemin.Values[i].position.Y - CorpsCelestesChemin.Values[i + 1].position.Y) -
                        (CorpsCelestesChemin.Values[i - 1].position.Y - CorpsCelestesChemin.Values[i + 1].position.Y) *
                        (CorpsCelestesChemin.Values[i].position.X - CorpsCelestesChemin.Values[i + 1].position.X);


                    if (coteLigne > 0)
                    {
                        Matrix.CreateRotationZ(-MathHelper.PiOver2, out _matrice1);
                        Vector3.Transform(ref _vecteur1, ref _matrice1, out _vecteur2); //v1: direction, v2: perpendiculaire
                        _vecteur2.Normalize();

                        Vector3.Multiply(ref _vecteur2, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[0] = CorpsCelestesChemin.Values[i].position + _vecteur2;

                        Vector3.Multiply(ref _vecteur1, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[1] = CorpsCelestesChemin.Values[i].position + _vecteur2;

                        Matrix.CreateRotationZ(MathHelper.PiOver2, out _matrice1);
                        Vector3.Transform(ref _vecteur1, ref _matrice1, out _vecteur2);
                        _vecteur2.Normalize();

                        Vector3.Multiply(ref _vecteur2, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[2] = CorpsCelestesChemin.Values[i].position + _vecteur2;

                        if (CorpsCelestesChemin.Values[i].ContientTourelleGravitationnelleNiveau2)
                        {
                            Vector3.Multiply(ref _vecteur1, -(CorpsCelestesChemin.Values[i].Cercle.Rayon + 50), out _vecteur2);
                            CorpsCelestesChemin.Values[i].SousPoints[3] = CorpsCelestesChemin.Values[i].position + _vecteur2;
                        }
                    }

                    else
                    {
                        Matrix.CreateRotationZ(MathHelper.PiOver2, out _matrice1);
                        Vector3.Transform(ref _vecteur1, ref _matrice1, out _vecteur2);

                        if (_vecteur2 != Vector3.Zero)
                            _vecteur2.Normalize();

                        Vector3.Multiply(ref _vecteur2, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[0] = CorpsCelestesChemin.Values[i].Position + _vecteur2;

                        Vector3.Multiply(ref _vecteur1, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[1] = CorpsCelestesChemin.Values[i].Position + _vecteur2;

                        Matrix.CreateRotationZ(-MathHelper.PiOver2, out _matrice1);
                        Vector3.Transform(ref _vecteur1, ref _matrice1, out _vecteur2);

                        if (_vecteur2 != Vector3.Zero)
                        _vecteur2.Normalize();

                        Vector3.Multiply(ref _vecteur2, CorpsCelestesChemin.Values[i].Cercle.Rayon + 50, out _vecteur2);
                        CorpsCelestesChemin.Values[i].SousPoints[2] = CorpsCelestesChemin.Values[i].Position + _vecteur2;

                        if (CorpsCelestesChemin.Values[i].ContientTourelleGravitationnelleNiveau2)
                        {
                            Vector3.Multiply(ref _vecteur1, -(CorpsCelestesChemin.Values[i].Cercle.Rayon + 50), out _vecteur2);
                            CorpsCelestesChemin.Values[i].SousPoints[3] = CorpsCelestesChemin.Values[i].position + _vecteur2;
                        }
                    }
                }
            }
        }

    }
}
