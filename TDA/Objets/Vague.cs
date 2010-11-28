namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class Vague
    {
        public double TempsApparition;
        public Vector3 PositionDepart;
        public List<Ennemi> Ennemis;
        public Dictionary<TypeEnnemi, DescripteurEnnemi> Composition;

        private double TempsDebut;

        private List<KeyValuePair<DescripteurEnnemi, double>> EnnemisACreer;
        private Dictionary<int, Ennemi> EnnemisCrees;

        private Simulation Simulation;

        public int NbEnnemis;

        public Vague(Simulation simulation, DescripteurVague descripteur)
        {
            this.Simulation = simulation;

            EnnemisACreer = new List<KeyValuePair<DescripteurEnnemi, double>>();
            EnnemisCrees = new Dictionary<int, Ennemi>();
            Ennemis = new List<Ennemi>();
            Composition = new Dictionary<TypeEnnemi, DescripteurEnnemi>();

            TempsApparition = descripteur.TempsDepart;

            double dernierTempsCreation = 0;

            for (int i = 0; i < descripteur.Ennemis.Count; i++)
            {
                DescripteurEnnemi e = descripteur.Ennemis[i];

                double frequence = FactoryEnnemis.Instance.getTaille(e.Type) + (int)descripteur.Distances[i] / FactoryEnnemis.Instance.getVitesse(e.Type, e.NiveauVitesse) * (1000f / 60f);

                EnnemisACreer.Add(new KeyValuePair<DescripteurEnnemi, double>(e, dernierTempsCreation + frequence));

                //ark?
                if (Composition.ContainsKey(e.Type))
                    Composition[e.Type].Valeur++;
                else
                {
                    DescripteurEnnemi desc = new DescripteurEnnemi();
                    desc.Type = e.Type;
                    desc.Valeur = 1;
                    desc.NiveauVitesse = e.NiveauVitesse;
                    desc.NiveauPointsVie = e.NiveauPointsVie;

                    Composition.Add(e.Type, desc);
                }

                dernierTempsCreation += frequence + e.PauseApres;
            }

            EnnemisACreer.Sort(delegate(KeyValuePair<DescripteurEnnemi, double> e1, KeyValuePair<DescripteurEnnemi, double> e2)
            {
                return e1.Value.CompareTo(e2.Value);
            });

            EnnemisACreer.Reverse();

            NbEnnemis = EnnemisACreer.Count;
        }


        public void Update(GameTime gameTime)
        {
            TempsDebut += gameTime.ElapsedGameTime.TotalMilliseconds;

            Ennemis.Clear();

            if (EnnemisACreer.Count == 0)
                return;

            int i = EnnemisACreer.Count - 1;

            while (i >= 0 && EnnemisACreer[i].Value <= TempsDebut)
            {
                DescripteurEnnemi description = EnnemisACreer[i].Key;

                Ennemi e = FactoryEnnemis.Instance.creerEnnemi
                (
                    Simulation,
                    description.Type,
                    description.NiveauVitesse,
                    description.NiveauPointsVie,
                    description.Valeur
                );

                e.Position = PositionDepart;

                EnnemisACreer.RemoveAt(i);
                Ennemis.Add(e);
                EnnemisCrees.Add(e.GetHashCode(), e);

                i--;
            }
        }

        public bool EstTerminee
        {
            get
            {
                return EnnemisACreer.Count == 0 && EnnemisCrees.Count == 0;
            }
        }


        public void doEnnemiDetruit(Ennemi ennemi)
        {
            EnnemisCrees.Remove(ennemi.GetHashCode());
        }
    }
}
