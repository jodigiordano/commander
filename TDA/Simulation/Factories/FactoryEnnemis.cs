namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    using ProjectMercury.Emitters;

    public enum TypeEnnemi
    {
        Asteroid,
        Comet,
        Plutoid,
        Centaur,
        Trojan,
        Meteoroid,
        Inconnu
    };

    class FactoryEnnemis
    {
        //=====================================================================
        // Attributs
        //=====================================================================

        private Simulation Simulation;
        private static List<Ennemi> EnnemisDisponibles;
        private Dictionary<TypeEnnemi, Pool<Ennemi>> PoolsEnnemis;

        private static Dictionary<TypeEnnemi, Color> CouleursEnnemis = new Dictionary<TypeEnnemi, Color>()
        {
            { TypeEnnemi.Asteroid, new Color(255, 178, 12) },
            { TypeEnnemi.Comet, new Color(255, 142, 161) },
            { TypeEnnemi.Plutoid, new Color(7, 143, 255) },
            { TypeEnnemi.Centaur, new Color(92, 198, 11) },
            { TypeEnnemi.Trojan, new Color(255, 66, 217) },
            { TypeEnnemi.Meteoroid, new Color(239, 0, 0) }
        };


        //=====================================================================
        // Singleton
        //=====================================================================

        private static FactoryEnnemis instance = new FactoryEnnemis();

        public FactoryEnnemis()
        {
            PoolsEnnemis = new Dictionary<TypeEnnemi, Pool<Ennemi>>();
            PoolsEnnemis.Add(TypeEnnemi.Asteroid, new Pool<Ennemi>());
            PoolsEnnemis.Add(TypeEnnemi.Centaur, new Pool<Ennemi>());
            PoolsEnnemis.Add(TypeEnnemi.Comet, new Pool<Ennemi>());
            PoolsEnnemis.Add(TypeEnnemi.Meteoroid, new Pool<Ennemi>());
            PoolsEnnemis.Add(TypeEnnemi.Plutoid, new Pool<Ennemi>());
            PoolsEnnemis.Add(TypeEnnemi.Trojan, new Pool<Ennemi>());
        }


        public static FactoryEnnemis Instance
        {
            get { return instance; }
        }


        //=====================================================================
        // Services
        //=====================================================================

        public Ennemi creerEnnemi(Simulation simulation, TypeEnnemi type, int niveauVitesse, int niveauPointsVie, int valeurUnites)
        {
            this.Simulation = simulation;

            Ennemi e = PoolsEnnemis[type].recuperer();

            e.Simulation = Simulation;
            e.Vitesse = getVitesse(type, niveauVitesse);
            e.PointsVie = e.PointsVieDepart = getPointsVie(type, niveauPointsVie); ;
            e.ValeurUnites = valeurUnites;
            e.Id = Ennemi.NextID;

            if (e.Type == TypeEnnemi.Inconnu)
            {
                e.Type = type;
                e.Nom = type.ToString("g");
                e.Couleur = CouleursEnnemis[type];
            }

            return e;
        }


        public void retournerEnnemi(Ennemi ennemi)
        {
            PoolsEnnemis[ennemi.Type].retourner(ennemi);
        }


        public List<Ennemi> GetEnnemisDisponibles(Simulation simulation)
        {
            if (EnnemisDisponibles == null)
            {
                EnnemisDisponibles = new List<Ennemi>();
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Asteroid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Centaur, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Comet, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Meteoroid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Plutoid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, TypeEnnemi.Trojan, 1, 1, 1));
            }

            return EnnemisDisponibles;
        }


        public float getPointsVie(TypeEnnemi type, int niveauPointsVie)
        {
            float pointsDeVie = 0;

            switch (type)
            {
                case TypeEnnemi.Asteroid:   pointsDeVie = 5 + 10 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Centaur:    pointsDeVie = 50 + 25 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Comet:      pointsDeVie = 5 + 2 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Meteoroid:  pointsDeVie = 1 + 5 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Plutoid:    pointsDeVie = 25 + 15 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Trojan:     pointsDeVie = 25 + 10 * (niveauPointsVie - 1); break;
            }

            return pointsDeVie;
        }


        public float getVitesse(TypeEnnemi type, int niveauVitesse)
        {
            float vitesse = 0;

            switch (type)
            {
                case TypeEnnemi.Asteroid: vitesse = 1 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Centaur: vitesse = 0.8f + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Comet: vitesse = 4 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Meteoroid: vitesse = 1.5f + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Plutoid: vitesse = 2 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Trojan: vitesse = 2.5f + 0 * (niveauVitesse - 1); break;
            }

            return vitesse;
        }


        public int getTaille(TypeEnnemi type)
        {
            int taille = 0;

            switch (type)
            {
                case TypeEnnemi.Asteroid: taille = 20; break;
                case TypeEnnemi.Centaur: taille = 30; break;
                case TypeEnnemi.Comet: taille = 10; break;
                case TypeEnnemi.Meteoroid: taille = 35; break;
                case TypeEnnemi.Plutoid: taille = 25; break;
                case TypeEnnemi.Trojan: taille = 15; break;
            }

            return taille;
        }


        public string getTexture(TypeEnnemi type)
        {
            return type.ToString("g");
        }
    }
}
