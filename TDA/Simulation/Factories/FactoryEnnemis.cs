namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using ProjectMercury.Emitters;

    public enum EnemyType
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
        private Dictionary<EnemyType, Pool<Ennemi>> PoolsEnnemis;

        private static Dictionary<EnemyType, Color> CouleursEnnemis = new Dictionary<EnemyType, Color>()
        {
            { EnemyType.Asteroid, new Color(255, 178, 12) },
            { EnemyType.Comet, new Color(255, 142, 161) },
            { EnemyType.Plutoid, new Color(7, 143, 255) },
            { EnemyType.Centaur, new Color(92, 198, 11) },
            { EnemyType.Trojan, new Color(255, 66, 217) },
            { EnemyType.Meteoroid, new Color(239, 0, 0) }
        };


        //=====================================================================
        // Singleton
        //=====================================================================

        private static FactoryEnnemis instance = new FactoryEnnemis();

        public FactoryEnnemis()
        {
            PoolsEnnemis = new Dictionary<EnemyType, Pool<Ennemi>>();
            PoolsEnnemis.Add(EnemyType.Asteroid, new Pool<Ennemi>());
            PoolsEnnemis.Add(EnemyType.Centaur, new Pool<Ennemi>());
            PoolsEnnemis.Add(EnemyType.Comet, new Pool<Ennemi>());
            PoolsEnnemis.Add(EnemyType.Meteoroid, new Pool<Ennemi>());
            PoolsEnnemis.Add(EnemyType.Plutoid, new Pool<Ennemi>());
            PoolsEnnemis.Add(EnemyType.Trojan, new Pool<Ennemi>());
        }


        public static FactoryEnnemis Instance
        {
            get { return instance; }
        }


        //=====================================================================
        // Services
        //=====================================================================

        public Ennemi creerEnnemi(Simulation simulation, EnemyType type, int niveauVitesse, int niveauPointsVie, int valeurUnites)
        {
            this.Simulation = simulation;

            Ennemi e = PoolsEnnemis[type].recuperer();

            e.Simulation = Simulation;
            e.Vitesse = getVitesse(type, niveauVitesse);
            e.PointsVie = e.PointsVieDepart = getPointsVie(type, niveauPointsVie); ;
            e.ValeurUnites = valeurUnites;
            e.ValeurPoints = niveauPointsVie;
            e.Id = Ennemi.NextID;

            if (e.Type == EnemyType.Inconnu)
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
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Asteroid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Centaur, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Comet, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Meteoroid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Plutoid, 1, 1, 1));
                EnnemisDisponibles.Add(creerEnnemi(simulation, EnemyType.Trojan, 1, 1, 1));
            }

            return EnnemisDisponibles;
        }


        public float getPointsVie(EnemyType type, int niveauPointsVie)
        {
            float pointsDeVie = 0;

            switch (type)
            {
                case EnemyType.Asteroid:   pointsDeVie = 5 + 10 * (niveauPointsVie - 1); break;
                case EnemyType.Centaur:    pointsDeVie = 50 + 25 * (niveauPointsVie - 1); break;
                case EnemyType.Comet:      pointsDeVie = 5 + 2 * (niveauPointsVie - 1); break;
                case EnemyType.Meteoroid:  pointsDeVie = 1 + 5 * (niveauPointsVie - 1); break;
                case EnemyType.Plutoid:    pointsDeVie = 25 + 15 * (niveauPointsVie - 1); break;
                case EnemyType.Trojan:     pointsDeVie = 25 + 10 * (niveauPointsVie - 1); break;
            }

            return pointsDeVie;
        }


        public float getVitesse(EnemyType type, int niveauVitesse)
        {
            float vitesse = 0;

            switch (type)
            {
                case EnemyType.Asteroid: vitesse = 1 + 0 * (niveauVitesse - 1); break;
                case EnemyType.Centaur: vitesse = 0.8f + 0 * (niveauVitesse - 1); break;
                case EnemyType.Comet: vitesse = 4 + 0 * (niveauVitesse - 1); break;
                case EnemyType.Meteoroid: vitesse = 1.5f + 0 * (niveauVitesse - 1); break;
                case EnemyType.Plutoid: vitesse = 2 + 0 * (niveauVitesse - 1); break;
                case EnemyType.Trojan: vitesse = 2.5f + 0 * (niveauVitesse - 1); break;
            }

            return vitesse;
        }


        public int getTaille(EnemyType type)
        {
            int taille = 0;

            switch (type)
            {
                case EnemyType.Asteroid: taille = 20; break;
                case EnemyType.Centaur: taille = 30; break;
                case EnemyType.Comet: taille = 10; break;
                case EnemyType.Meteoroid: taille = 35; break;
                case EnemyType.Plutoid: taille = 25; break;
                case EnemyType.Trojan: taille = 15; break;
            }

            return taille;
        }


        public string getTexture(EnemyType type)
        {
            return type.ToString("g");
        }
    }
}
