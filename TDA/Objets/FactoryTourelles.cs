namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    public enum TypeTourelle
    {
        Base,
        Missile,
        Gravitationnelle,
        LaserMultiple,
        LaserSimple,
        Inconnu,
        GravitationnelleAlien,
        SlowMotion
    };

    static class FactoryTourelles
    {
        public static Simulation Simulation { get; set; }

        private static List<Tourelle> TourellesDisponibles;

        public static Tourelle creerTourelle(Simulation simulation, TypeTourelle type)
        {
            Simulation = simulation;

            switch (type)
            {
                case TypeTourelle.Base:                     return setBase();                   break;
                case TypeTourelle.Gravitationnelle:         return setGravitationnelle();       break;
                case TypeTourelle.LaserMultiple:            return setLaserMultiple();          break;
                case TypeTourelle.LaserSimple:              return setLaserSimple();            break;
                case TypeTourelle.Missile:                  return setMissile();                break;
                case TypeTourelle.GravitationnelleAlien:    return setGravitationnelleAlien();  break;
                case TypeTourelle.SlowMotion:               return setSlowMotion();             break;
                default:                                    return setBase();                   break;
            }
        }

        public static List<Tourelle> GetTourellesDisponibles()
        {
            if (TourellesDisponibles == null)
            {
                TourellesDisponibles = new List<Tourelle>();
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.Base));
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.Gravitationnelle));
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.LaserMultiple));
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.LaserSimple));
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.Missile));
                TourellesDisponibles.Add(creerTourelle(Simulation, TypeTourelle.SlowMotion));
            }

            return TourellesDisponibles;
        }

        private static Tourelle setBase()
        {
            TourelleBase tourelle = new TourelleBase(Simulation);

            tourelle.Niveaux = getNiveauxBase1();

            setRepresentations(tourelle);

            tourelle.representation.Origine = new Vector2(24, 36);

            return tourelle;
        }


        private static Tourelle setSlowMotion()
        {
            TourelleSlowMotion tourelle = new TourelleSlowMotion(Simulation);

            tourelle.Niveaux = getNiveauxSlowMotion1();

            setRepresentations(tourelle);

            tourelle.representation.Taille = 4;
            tourelle.representationBase.Taille = 4;
            tourelle.representation.Origine = tourelle.representation.Centre;

            return tourelle;
        }


        private static Tourelle setGravitationnelle()
        {
            TourelleGravitationnelle tourelle = new TourelleGravitationnelle(Simulation);

            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 1000, 500, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 500, TypeProjectile.Aucun, "tourelleGravitationnelleAntenne", "tourelleGravitationnelleBase", 0, null, 0));
            niveaux.AddLast(new NiveauTourelle(2, 500, 750, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 500, TypeProjectile.Aucun, "tourelleGravitationnelleAntenne", "tourelleGravitationnelleBase", 0, null, 0));

            tourelle.Niveaux = niveaux;

            setRepresentations(tourelle);

            tourelle.representation.Taille = 4;
            tourelle.representationBase.Taille = 4;
            tourelle.representation.PrioriteAffichage = tourelle.representationBase.PrioriteAffichage - 0.01f;

            return tourelle;
        }


        private static Tourelle setGravitationnelleAlien()
        {
            TourelleGravitationnelle tourelle = new TourelleGravitationnelle(Simulation);

            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 1000, 500, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 1000, TypeProjectile.Aucun, "tourelleAlien", "tourelleAlienBase", 0, null, 0));

            tourelle.Niveaux = niveaux;

            setRepresentations(tourelle);

            tourelle.VitesseRotationBase = (tourelle.VitesseRotationAntenne > 0) ? -0.02f : 0.02f;
            tourelle.representation.PrioriteAffichage = tourelle.representationBase.PrioriteAffichage - 0.01f;
            tourelle.representation.Couleur.A = 60;
            tourelle.representationBase.Couleur.A = 60;

            return tourelle;
        }


        private static Tourelle setLaserMultiple()
        {
            TourelleLasersMultiples tourelle = new TourelleLasersMultiples(Simulation);

            tourelle.Niveaux = getNiveauxLasersMultiples1();

            setRepresentations(tourelle);

            tourelle.representation.Origine = new Vector2(6, 8);
            tourelle.representation.Taille = 4;
            tourelle.representationBase.Taille = 4;

            return tourelle;
        }


        private static Tourelle setLaserSimple()
        {
            TourelleLasersSimples tourelle = new TourelleLasersSimples(Simulation);

            tourelle.Niveaux = getNiveauxLaserSimple1();

            setRepresentations(tourelle);

            tourelle.representation.Origine = new Vector2(6, 8);
            tourelle.representation.Taille = 4;
            tourelle.representationBase.Taille = 4;

            return tourelle;
        }


        private static Tourelle setMissile()
        {
            TourelleMissiles tourelle = new TourelleMissiles(Simulation);

            tourelle.Niveaux = getNiveauxMissiles1();

            setRepresentations(tourelle);

            tourelle.representation.Origine = new Vector2(6, 8);
            tourelle.representation.Taille = 4;
            tourelle.representationBase.Taille = 4;

            return tourelle;
        }

        private static LinkedList<NiveauTourelle> getNiveauxLaserSimple1()
        {
            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 10, 5, new Cercle(Vector3.Zero, 50), 1700, 1, 500, TypeProjectile.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.15f, null, 0));
            niveaux.AddLast(new NiveauTourelle(2, 20, 15, new Cercle(Vector3.Zero, 65), 1650, 1, 1000, TypeProjectile.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.25f, null, 0));
            niveaux.AddLast(new NiveauTourelle(3, 40, 35, new Cercle(Vector3.Zero, 80), 1600, 1, 1500, TypeProjectile.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.4f, null, 0));
            niveaux.AddLast(new NiveauTourelle(4, 70, 70, new Cercle(Vector3.Zero, 95), 1550, 1, 2000, TypeProjectile.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 0.65f, null, 0));
            niveaux.AddLast(new NiveauTourelle(5, 100, 120, new Cercle(Vector3.Zero, 110), 1500, 1, 2500, TypeProjectile.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 0.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(6, 130, 185, new Cercle(Vector3.Zero, 125), 1450, 1, 3000, TypeProjectile.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 1f, null, 0));
            niveaux.AddLast(new NiveauTourelle(7, 160, 265, new Cercle(Vector3.Zero, 140), 1400, 1, 3500, TypeProjectile.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 1.4f, null, 0));
            niveaux.AddLast(new NiveauTourelle(8, 190, 360, new Cercle(Vector3.Zero, 155), 1350, 1, 4000, TypeProjectile.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 1.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(9, 210, 465, new Cercle(Vector3.Zero, 170), 1300, 1, 4500, TypeProjectile.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 2.5f, null, 0));
            niveaux.AddLast(new NiveauTourelle(10, 240, 585, new Cercle(Vector3.Zero, 185), 1250, 1, 5000, TypeProjectile.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 4f, null, 0));

            return niveaux;
        }


        private static LinkedList<NiveauTourelle> getNiveauxSlowMotion1()
        {
            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 100, 50, new Cercle(Vector3.Zero, 100), 2000, 1, 2000, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon1", "tourelleSlowMotionBase", 0.2f, null, 0));
            niveaux.AddLast(new NiveauTourelle(2, 120, 110, new Cercle(Vector3.Zero, 120), 1800, 1, 3500, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon1", "tourelleSlowMotionBase", 0.4f, null, 0));
            niveaux.AddLast(new NiveauTourelle(3, 140, 180, new Cercle(Vector3.Zero, 140), 1600, 1, 5000, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.6f, null, 0));
            niveaux.AddLast(new NiveauTourelle(4, 160, 260, new Cercle(Vector3.Zero, 160), 1400, 1, 6500, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(5, 180, 350, new Cercle(Vector3.Zero, 180), 1300, 1, 8000, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.9f, null, 0));
            niveaux.AddLast(new NiveauTourelle(6, 200, 450, new Cercle(Vector3.Zero, 200), 1200, 1, 9500, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 1.0f, null, 0));
            niveaux.AddLast(new NiveauTourelle(7, 220, 560, new Cercle(Vector3.Zero, 220), 1100, 1, 11000, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 1.1f, null, 0));
            niveaux.AddLast(new NiveauTourelle(8, 240, 680, new Cercle(Vector3.Zero, 240), 1000, 1, 12500, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.2f, null, 0));
            niveaux.AddLast(new NiveauTourelle(9, 280, 820, new Cercle(Vector3.Zero, 260), 900, 1, 14000, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.3f, null, 0));
            niveaux.AddLast(new NiveauTourelle(10, 300, 970, new Cercle(Vector3.Zero, 280), 800, 1, 15500, TypeProjectile.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.4f, null, 0));

            return niveaux;
        }

        private static LinkedList<NiveauTourelle> getNiveauxBase1()
        {
            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 30, 15, new Cercle(Vector3.Zero, 75), 650, 1, 1000, TypeProjectile.Base, "tourelleBase1", "tourelleBaseBase", 5, null, 0));
            niveaux.AddLast(new NiveauTourelle(2, 60, 45, new Cercle(Vector3.Zero, 100), 600, 1, 2000, TypeProjectile.Base, "tourelleBase1", "tourelleBaseBase", 7, null, 0));
            niveaux.AddLast(new NiveauTourelle(3, 90, 90, new Cercle(Vector3.Zero, 125), 550, 1, 3000, TypeProjectile.Base, "tourelleBase1", "tourelleBaseBase", 10, null, 0));
            niveaux.AddLast(new NiveauTourelle(4, 120, 150, new Cercle(Vector3.Zero, 150), 500, 2, 4000, TypeProjectile.Base, "tourelleBase2", "tourelleBaseBase", 7, null, 0));
            niveaux.AddLast(new NiveauTourelle(5, 150, 225, new Cercle(Vector3.Zero, 175), 450, 2, 5000, TypeProjectile.Base, "tourelleBase2", "tourelleBaseBase", 10, null, 0));
            niveaux.AddLast(new NiveauTourelle(6, 180, 315, new Cercle(Vector3.Zero, 200), 400, 2, 6000, TypeProjectile.Base, "tourelleBase2", "tourelleBaseBase", 12, null, 0));
            niveaux.AddLast(new NiveauTourelle(7, 210, 420, new Cercle(Vector3.Zero, 225), 350, 2, 7000, TypeProjectile.Base, "tourelleBase2", "tourelleBaseBase", 14, null, 0));
            niveaux.AddLast(new NiveauTourelle(8, 240, 540, new Cercle(Vector3.Zero, 250), 300, 3, 8000, TypeProjectile.Base, "tourelleBase3", "tourelleBaseBase", 9, null, 0));
            niveaux.AddLast(new NiveauTourelle(9, 270, 675, new Cercle(Vector3.Zero, 275), 250, 3, 9000, TypeProjectile.Base, "tourelleBase3", "tourelleBaseBase", 11, null, 0));
            niveaux.AddLast(new NiveauTourelle(10, 300, 825, new Cercle(Vector3.Zero, 300), 200, 3, 10000, TypeProjectile.Base, "tourelleBase3", "tourelleBaseBase", 12, null, 0));

            return niveaux;
        }


        private static LinkedList<NiveauTourelle> getNiveauxLasersMultiples1()
        {
            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 300, 100, new Cercle(Vector3.Zero, 120), 2000, 1, 4000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase", 0.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(2, 200, 300, new Cercle(Vector3.Zero, 140), 1900, 1, 6000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase", 1.0f, null, 0));
            niveaux.AddLast(new NiveauTourelle(3, 300, 600, new Cercle(Vector3.Zero, 160), 1800, 2, 10000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.6f, null, 0));
            niveaux.AddLast(new NiveauTourelle(4, 400, 1000, new Cercle(Vector3.Zero, 180), 1700, 2, 15000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(5, 500, 1500, new Cercle(Vector3.Zero, 200), 1600, 2, 20000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.9f, null, 0));
            niveaux.AddLast(new NiveauTourelle(6, 600, 2100, new Cercle(Vector3.Zero, 220), 1500, 2, 25000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 1.0f, null, 0));
            niveaux.AddLast(new NiveauTourelle(7, 700, 2750, new Cercle(Vector3.Zero, 240), 1400, 3, 30000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.7f, null, 0));
            niveaux.AddLast(new NiveauTourelle(8, 800, 3600, new Cercle(Vector3.Zero, 280), 1300, 3, 35000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.8f, null, 0));
            niveaux.AddLast(new NiveauTourelle(9, 900, 4500, new Cercle(Vector3.Zero, 300), 1200, 3, 40000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.9f, null, 0));
            niveaux.AddLast(new NiveauTourelle(10, 1000, 5500, new Cercle(Vector3.Zero, 320), 1100, 3, 45000, TypeProjectile.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 1.0f, null, 0));

            return niveaux;
        }


        private static LinkedList<NiveauTourelle> getNiveauxMissiles1()
        {
            LinkedList<NiveauTourelle> niveaux = new LinkedList<NiveauTourelle>();

            niveaux.AddLast(new NiveauTourelle(1, 150, 75, new Cercle(Vector3.Zero, 150), 2600, 1, 3000, TypeProjectile.Missile, "tourelleMissileCanon1", "tourelleMissileBase", 30, new Cercle(Vector3.Zero, 50), 1.8f));
            niveaux.AddLast(new NiveauTourelle(2, 100, 125, new Cercle(Vector3.Zero, 200), 2400, 1, 5000, TypeProjectile.Missile, "tourelleMissileCanon1", "tourelleMissileBase", 40, new Cercle(Vector3.Zero, 60), 2.0f));
            niveaux.AddLast(new NiveauTourelle(3, 150, 200, new Cercle(Vector3.Zero, 250), 2200, 1, 7000, TypeProjectile.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 50, new Cercle(Vector3.Zero, 70), 2.2f));
            niveaux.AddLast(new NiveauTourelle(4, 200, 300, new Cercle(Vector3.Zero, 275), 2000, 1, 9000, TypeProjectile.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 60, new Cercle(Vector3.Zero, 80), 2.4f));
            niveaux.AddLast(new NiveauTourelle(5, 250, 425, new Cercle(Vector3.Zero, 300), 1800, 1, 11000, TypeProjectile.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 70, new Cercle(Vector3.Zero, 90), 2.6f));
            niveaux.AddLast(new NiveauTourelle(6, 300, 575, new Cercle(Vector3.Zero, 325), 1600, 1, 13000, TypeProjectile.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 80, new Cercle(Vector3.Zero, 100), 2.8f));
            niveaux.AddLast(new NiveauTourelle(7, 350, 750, new Cercle(Vector3.Zero, 350), 1400, 1, 15000, TypeProjectile.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 90, new Cercle(Vector3.Zero, 110), 3.0f));
            niveaux.AddLast(new NiveauTourelle(8, 400, 950, new Cercle(Vector3.Zero, 400), 1200, 1, 17000, TypeProjectile.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 100, new Cercle(Vector3.Zero, 120), 3.2f));
            niveaux.AddLast(new NiveauTourelle(9, 450, 1175, new Cercle(Vector3.Zero, 425), 1000, 1, 19000, TypeProjectile.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 110, new Cercle(Vector3.Zero, 130), 3.4f));
            niveaux.AddLast(new NiveauTourelle(10, 500, 1425, new Cercle(Vector3.Zero, 450), 800, 1, 21000, TypeProjectile.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 120, new Cercle(Vector3.Zero, 140), 3.6f));

            return niveaux;
        }


        private static void setRepresentations(Tourelle tourelle)
        {
            tourelle.representationBase = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(tourelle.Niveaux.First.Value.RepresentationBase), Vector3.Zero, Simulation.Scene);
            tourelle.representationBase.Origine = tourelle.representationBase.Centre;

            tourelle.representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(tourelle.Niveaux.First.Value.Representation), Vector3.Zero, Simulation.Scene);
            tourelle.representation.Origine = tourelle.representation.Centre;

            tourelle.PrioriteAffichage = Preferences.PrioriteSimulationTourelle;
        }
    }
}
