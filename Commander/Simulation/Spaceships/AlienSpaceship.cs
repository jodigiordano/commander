namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.SimplePersistence;
    using EphemereGames.Core.Visual;


    public class AlienSpaceship
    {
        public Sprite Tentacules;
        public Image Image;


        public AlienSpaceship(double visualPriority)
        {
            Image = new Image("VaisseauAlien")
            {
                SizeX = 8,
                VisualPriority = visualPriority
            };
            
            Tentacules = Persistence.GetAssetCopy<Sprite>("tentacules");
            Tentacules.Taille = 8;
            Tentacules.Origine = Tentacules.Centre;
            Tentacules.VisualPriority = Image.VisualPriority + 0.01f;
        }


        public void Update()
        {
            Tentacules.Update();
        }


        public void Draw(Scene scene)
        {
            Tentacules.Position = Image.Position;

            scene.Add(Image);
            scene.Add(Tentacules);
        }
    }
}
