namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class EditorLevelGenerator
    {
        private static LiteSimulator LiteSimulator = new LiteSimulator();

        public static List<Size> PossibleSizes = new List<Size>()
        {
            Size.Small,
            Size.Normal,
            Size.Big
        };

        public static List<float> PossibleRotationTimes = new List<float>()
        {
            float.MaxValue,
            1200000,
            900000,
            600000,
            480000,
            360000,
            240000,
            120000,
            60000,
            30000,
            10000,
        };

        private static Dictionary<Size, PhysicalRectangle> Limits = new Dictionary<Size, PhysicalRectangle>()
        {
            { Size.Small,  new PhysicalRectangle(-640 + 123, -370 + 112, 1098, 560) },
            { Size.Normal, new PhysicalRectangle(-640 + 147, -370 + 136, 1050, 512) },
            { Size.Big,  new PhysicalRectangle(-640 + 179, -370 + 168,  986, 448) }
        };

        private static Dictionary<Size, Vector3> MinimalDistances = new Dictionary<Size, Vector3>()
        {
            { Size.Small,  new Vector3( 64,  92, 120) },
            { Size.Normal, new Vector3( 92, 120, 144) },
            { Size.Big,  new Vector3(120, 144, 176) }
        };

        private static Dictionary<EnemyType, string> EnemiesImages = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Asteroid, "Asteroid" },
            { EnemyType.Centaur, "Centaur" },
            { EnemyType.Plutoid, "Plutoid" },
            { EnemyType.Comet, "Comet" },
            { EnemyType.Trojan, "Trojan" },
            { EnemyType.Meteoroid, "Meteoroid" }
        };


        //public int NbCorpsCelestes;
        //public int NbCorpsCelestesFixes;
        //public List<TurretType> TourellesDisponibles;
        //public List<PowerUpType> PowerUpsDisponibles;
        //public bool AvecEtoileAuMilieu;
        //public int NbPlanetesCheminDeDepart;
        //public int ViesPlaneteAProteger;
        //public int ArgentExtra;
        //public int NbPacksVie;
        //public int NbEmplacements;
        //public int ArgentDepart;
        //public List<EnemyType> EnnemisPresents;
        //public LevelDescriptor LevelDescriptor;
        //public bool SystemeCentre;

        //private LiteSimulator SimulatorLite;
        //private List<CelestialBodyDescriptor> CorpsCelestes;

        //public LevelGenerator()
        //{
        //    SimulatorLite = new LiteSimulator();
        //    CorpsCelestes = new List<CelestialBodyDescriptor>();
        //}

        //public void genererGameplay()
        //{
        //    LevelDescriptor.Background = "fondecran" + Main.Random.Next(1, 23);
        //    LevelDescriptor.Player.Lives = ViesPlaneteAProteger;
        //    LevelDescriptor.Player.Money = ArgentDepart;
        //    LevelDescriptor.MineralsValue = ArgentExtra;
        //    LevelDescriptor.LifePacks = NbPacksVie;
        //}

        //public void genererSystemePlanetaire()
        //{
        //    LevelDescriptor = new LevelDescriptor();

        //    CorpsCelestes.Clear();

        //    GenerateCelestialBodies();
        //    GenerateStartingPath(); //doit etre fait avant les priorites pour un peu plus de random(awsom)nesssssssss
        //    GeneratePathPriorities();
        //    GenerateCelestialBodyToProtect();
        //    genererCeintureAsteroides();
        //}

        //private void GenerateCelestialBodies()
        //{
        //    //for (int i = 0; i < NbCorpsCelestes; i++)
        //    //{
        //    //    CelestialBodyDescriptor dcc = GenerateCelestialBody(i < NbCorpsCelestesFixes);

        //    //    CorpsCelestes.Add(dcc);

        //    //    LevelDescriptor.PlanetarySystem.Add(dcc);
        //    //}
        //}

        //public void genererCeintureAsteroides()
        //{
        //    CelestialBodyDescriptor ceinture = LevelDescriptor.PlanetarySystem[0];

        //    ceinture.Position = new Vector3((Main.Random.Next(0, 2) == 0) ? 700 : -700, -400, 0);
        //    ceinture.PathPriority = 0;
        //    ceinture.Speed = PossibleRotationTimes[Main.Random.Next(PossibleRotationTimes.Length / 2, PossibleRotationTimes.Length)];
        //    ceinture.Images = new List<string>();
        //    ceinture.StartingPosition = Main.Random.Next(0, 100);

        //    foreach (var ennemi in EnnemisPresents)
        //        ceinture.Images.Add(EnemiesImages[ennemi]);
        //}


        //private void GenerateCelestialBodyToProtect()
        //{
        //    CelestialBodyDescriptor aProteger = LevelDescriptor.PlanetarySystem[LevelDescriptor.PlanetarySystem.Count - 1];

        //    aProteger.PathPriority = int.MaxValue;

        //    aProteger.HasGravitationalTurret = true;

        //    LevelDescriptor.CelestialBodyToProtect = aProteger.PathPriority;
        //}


        public static CelestialBody GenerateCelestialBody(Simulator simulator, List<CelestialBody> CelestialBodies, double visualPriority)
        {
            return new CelestialBody(simulator, GenerateCelestialBody(CelestialBodies), visualPriority);
        }


        private static CelestialBodyDescriptor GenerateCelestialBody(List<CelestialBody> CelestialBodies)
        {
            CelestialBodyDescriptor d = new CelestialBodyDescriptor()
            {
                Name = "Planete" + Main.Random.Next(0, int.MaxValue),
                Invincible = false,
                InBackground = false,
                CanSelect = true,
                PathPriority = -1,
                Image = "planete" + Main.Random.Next(1, 8).ToString()
            };


            PhysicalRectangle rp = null;

            do
            {
                int size = Main.Random.Next(0, 3);
                d.Size = (size == 0) ? Size.Small : (size == 1) ? Size.Normal : Size.Big;
                d.Speed = PossibleRotationTimes[Main.Random.Next(5, PossibleRotationTimes.Count)]; //to be assured that the planet has a valid path

                rp = Limits[d.Size];

                d.Position = new Vector3(Main.Random.Next(rp.X, rp.X + rp.Width), Main.Random.Next(rp.Y, rp.Y + rp.Height), 0);

                d.Offset = new Vector3(Main.Random.Next((int)(rp.X - d.Position.X + 200), (int)(rp.X + rp.Width - d.Position.X - 200)), Main.Random.Next((int)(rp.Y - d.Position.Y + 100), (int)(rp.Y + rp.Width - d.Position.Y - 100)), 0);

                d.Rotation = Main.Random.Next(-360, 360);
                d.StartingPosition = Main.Random.Next(0, 100);
            }
            while (!LiteSimulator.InBorders(d, rp) || LiteSimulator.CollidesWithOthers(d, CelestialBodies));

            return d;
        }


        //private void GeneratePathPriorities()
        //{
        //    int priorite = 1;

        //    CorpsCelestes.Sort(delegate(CelestialBodyDescriptor corps1, CelestialBodyDescriptor corps2)
        //    {
        //        if (corps1.Position.Length() > corps2.Position.Length())
        //            return 1;

        //        if (corps1.Position.Length() < corps2.Position.Length())
        //            return -1;

        //        return 0;
        //    });

        //    foreach (var corpsCeleste in CorpsCelestes)
        //        corpsCeleste.PathPriority = priorite++;
        //}


        //private void GenerateStartingPath()
        //{
        //    int nbPlanetesCheminDepartEffectif = 0;

        //    foreach (var planete in CorpsCelestes)
        //    {
        //        if (planete.HasGravitationalTurret)
        //            nbPlanetesCheminDepartEffectif++;
        //    }

        //    nbPlanetesCheminDepartEffectif = Math.Min(nbPlanetesCheminDepartEffectif, NbPlanetesCheminDeDepart);

        //    List<CelestialBodyDescriptor> corps = new List<CelestialBodyDescriptor>(CorpsCelestes);

        //    while (corps.Count > 0 && nbPlanetesCheminDepartEffectif > 0)
        //    {
        //        int indice = Main.Random.Next(0, corps.Count);

        //        CelestialBodyDescriptor corpsSelectionne = corps[indice];

        //        corps.RemoveAt(indice);

        //        if (!corpsSelectionne.HasGravitationalTurret)
        //        {
        //            corpsSelectionne.HasGravitationalTurret = true;

        //            nbPlanetesCheminDepartEffectif--;
        //        }
        //    }
        //}
    }
}
