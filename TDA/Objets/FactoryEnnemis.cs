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
            switch (type)
            {
                case TypeEnnemi.Asteroid:   return 5 + 10 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Centaur:    return 50 + 25 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Comet:      return 5 + 2 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Meteoroid:  return 1 + 5 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Plutoid:    return 25 + 15 * (niveauPointsVie - 1); break;
                case TypeEnnemi.Trojan:     return 25 + 10 * (niveauPointsVie - 1); break;
            }

            return 0;
        }


        public float getVitesse(TypeEnnemi type, int niveauVitesse)
        {
            switch (type)
            {
                case TypeEnnemi.Asteroid: return 1 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Centaur: return 0.8f + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Comet: return 4 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Meteoroid: return 1.5f + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Plutoid: return 2 + 0 * (niveauVitesse - 1); break;
                case TypeEnnemi.Trojan: return 2.5f + 0 * (niveauVitesse - 1); break;
            }

            return 0;
        }


        public int getTaille(TypeEnnemi type)
        {
            switch (type)
            {
                case TypeEnnemi.Asteroid: return 20; break;
                case TypeEnnemi.Centaur: return 30; break;
                case TypeEnnemi.Comet: return 10; break;
                case TypeEnnemi.Meteoroid: return 35; break;
                case TypeEnnemi.Plutoid: return 25; break;
                case TypeEnnemi.Trojan: return 15; break;
            }

            return 0;
        }


        public string getTexture(TypeEnnemi type)
        {
            return type.ToString("g");
        }
    }
}
