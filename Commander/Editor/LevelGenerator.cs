namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class LevelGenerator
    {
        private static int[] TempsRotationPossible = new int[]
        {
            10000,
            30000,
            60000,
            120000,
            240000,
            360000,
            480000,
            600000,
            900000,
            1200000
        };

        private static Dictionary<Size, RectanglePhysique> Cadres = new Dictionary<Size, RectanglePhysique>()
        {
            { Size.Small,  new RectanglePhysique(-640 + 123, -370 + 112, 1098, 560) },
            { Size.Normal, new RectanglePhysique(-640 + 147, -370 + 136, 1050, 512) },
            { Size.Big,  new RectanglePhysique(-640 + 179, -370 + 168,  986, 448) }
        };

        private static Dictionary<Size, Vector3> DistancesMin = new Dictionary<Size, Vector3>()
        {
            { Size.Small,  new Vector3( 64,  92, 120) },
            { Size.Normal, new Vector3( 92, 120, 144) },
            { Size.Big,  new Vector3(120, 144, 176) }
        };

        private static Dictionary<EnemyType, string> RepresentationsEnnemis = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Asteroid, "Asteroid" },
            { EnemyType.Centaur, "Centaur" },
            { EnemyType.Plutoid, "Plutoid" },
            { EnemyType.Comet, "Comet" },
            { EnemyType.Trojan, "Trojan" },
            { EnemyType.Meteoroid, "Meteoroid" }
        };


        public int NbCorpsCelestes;
        public int NbCorpsCelestesFixes;
        public List<TurretType> TourellesDisponibles;
        public List<PowerUpType> PowerUpsDisponibles;
        public bool AvecEtoileAuMilieu;
        public int NbPlanetesCheminDeDepart;
        public int ViesPlaneteAProteger;
        public int ArgentExtra;
        public int NbPacksVie;
        public int NbEmplacements;
        public int ArgentDepart;
        public List<EnemyType> EnnemisPresents;
        public LevelDescriptor LevelDescriptor;
        public bool SystemeCentre;

        private LiteSimulation SimulationLite;
        private List<CelestialBodyDescriptor> CorpsCelestes;

        public LevelGenerator()
        {
            SimulationLite = new LiteSimulation();
            CorpsCelestes = new List<CelestialBodyDescriptor>();
        }

        public void genererGameplay()
        {
            LevelDescriptor.Background = "fondecran" + Main.Random.Next(1, 23);
            LevelDescriptor.Player.Lives = ViesPlaneteAProteger;
            LevelDescriptor.Player.Money = ArgentDepart;
            LevelDescriptor.MineralsValue = ArgentExtra;
            LevelDescriptor.LifePacks = NbPacksVie;
        }

        public void genererSystemePlanetaire()
        {
            LevelDescriptor = LevelsFactory.GetEmptyDescriptor();

            CorpsCelestes.Clear();

            genererCorpsCelestes();
            genererCheminDepart(); //doit etre fait avant les priorites pour un peu plus de random(awsom)nesssssssss
            genererPriorites();
            genererPlaneteAProteger();
            genererCeintureAsteroides();

            if (AvecEtoileAuMilieu)
                genererEtoile();
        }

        private void genererCorpsCelestes()
        {
            for (int i = 0; i < NbCorpsCelestes; i++)
            {
                CelestialBodyDescriptor dcc = genererCorpsCeleste(i < NbCorpsCelestesFixes);

                CorpsCelestes.Add(dcc);

                LevelDescriptor.PlanetarySystem.Add(dcc);
            }
        }

        public void genererCeintureAsteroides()
        {
            CelestialBodyDescriptor ceinture = LevelDescriptor.PlanetarySystem[0];

            ceinture.Position = new Vector3((Main.Random.Next(0, 2) == 0) ? 700 : -700, -400, 0);
            ceinture.PathPriority = 0;
            ceinture.Speed = TempsRotationPossible[Main.Random.Next(TempsRotationPossible.Length / 2, TempsRotationPossible.Length)];
            ceinture.Images = new List<string>();
            ceinture.StartingPosition = Main.Random.Next(0, 100);

            foreach (var ennemi in EnnemisPresents)
                ceinture.Images.Add(RepresentationsEnnemis[ennemi]);
        }

        private void genererEtoile()
        {
            CelestialBodyDescriptor c = new CelestialBodyDescriptor();
            c.Name = "Etoile";
            c.Position = Vector3.Zero;
            c.Size = Size.Big;
            c.Image = "planete2";
            c.ParticulesEffect = "etoile";
            c.PathPriority = -1;
            c.InBackground = true;
            c.Invincible = false;
            c.CanSelect = false;
            c.Speed = 0;

            LevelDescriptor.PlanetarySystem.Add(c);
        }

        private void genererPlaneteAProteger()
        {
            CelestialBodyDescriptor aProteger = LevelDescriptor.PlanetarySystem[LevelDescriptor.PlanetarySystem.Count - 1];

            aProteger.PathPriority = int.MaxValue;

            aProteger.HasGravitationalTurret = true;

            LevelDescriptor.CelestialBodyToProtect = aProteger.PathPriority;
        }

        public List<Vector3> pointsConsideres()
        {
            return SimulationLite.pointsConsideres;
        }

        public List<Circle> cerclesConsideres()
        {
            return SimulationLite.cerclesConsideres;
        }

        private CelestialBodyDescriptor genererCorpsCeleste(bool fixe)
        {
            CelestialBodyDescriptor d = new CelestialBodyDescriptor();

            d.Name = "Planete" + Main.Random.Next(0, int.MaxValue); //todo
            d.Invincible = false;
            d.InBackground = false;

            d.Invincible = false;
            d.CanSelect = true;
            d.PathPriority = -1;

            int nbTentatives = 0;

            RectanglePhysique rp = null;

            do
            {
                nbTentatives++;

                int taille = Main.Random.Next(0, 3);
                d.Size = (taille == 0) ? Size.Small : (taille == 1) ? Size.Normal : Size.Big;
                d.Image = (fixe) ? "stationSpatiale" + Main.Random.Next(1, 3).ToString() :
                                            "planete" + Main.Random.Next(2, 8).ToString();

                //RepresentationParticules = null; //todo

                d.Speed = (fixe) ? 0 : TempsRotationPossible[Main.Random.Next(0, TempsRotationPossible.Length)];

                rp = Cadres[d.Size];

                d.Position = new Vector3(Main.Random.Next(rp.X, rp.X + rp.Width), Main.Random.Next(rp.Y, rp.Y + rp.Height), 0);

                d.Offset = (SystemeCentre) ?
                    Vector3.Zero :
                    new Vector3(Main.Random.Next((int)(rp.X - d.Position.X + 200), (int)(rp.X + rp.Width - d.Position.X - 200)), Main.Random.Next((int)(rp.Y - d.Position.Y + 100), (int)(rp.Y + rp.Width - d.Position.Y - 100)), 0);

                d.Rotation = Main.Random.Next(-360, 360);
                d.StartingPosition = Main.Random.Next(0, 100);
            }
            while (!SimulationLite.dansLesBornes(d, rp) || SimulationLite.collisionPlanetaire(d, CorpsCelestes));

            Console.WriteLine("nb tentatives: " + nbTentatives);
            
            return d;
        }


        private void genererPriorites()
        {
            int priorite = 1;

            CorpsCelestes.Sort(delegate(CelestialBodyDescriptor corps1, CelestialBodyDescriptor corps2)
            {
                if (corps1.Position.Length() > corps2.Position.Length())
                    return 1;

                if (corps1.Position.Length() < corps2.Position.Length())
                    return -1;

                return 0;
            });

            foreach (var corpsCeleste in CorpsCelestes)
                corpsCeleste.PathPriority = priorite++;
        }


        private void genererCheminDepart()
        {
            int nbPlanetesCheminDepartEffectif = 0;

            foreach (var planete in CorpsCelestes)
            {
                if (planete.HasGravitationalTurret)
                    nbPlanetesCheminDepartEffectif++;
            }

            nbPlanetesCheminDepartEffectif = Math.Min(nbPlanetesCheminDepartEffectif, NbPlanetesCheminDeDepart);

            List<CelestialBodyDescriptor> corps = new List<CelestialBodyDescriptor>(CorpsCelestes);

            while (corps.Count > 0 && nbPlanetesCheminDepartEffectif > 0)
            {
                int indice = Main.Random.Next(0, corps.Count);

                CelestialBodyDescriptor corpsSelectionne = corps[indice];

                corps.RemoveAt(indice);

                if (!corpsSelectionne.HasGravitationalTurret)
                {
                    corpsSelectionne.HasGravitationalTurret = true;

                    nbPlanetesCheminDepartEffectif--;
                }
            }
        }
    }
}
