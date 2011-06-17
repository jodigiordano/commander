namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AlienSpaceship
    {
        private Scene Scene;
        public Sprite Tentacules;
        public Image Representation;


        public AlienSpaceship(Scene scene, double prioriteAffichage)
        {
            Scene = scene;

            Representation = new Image("VaisseauAlien")
            {
                SizeX = 8,
                VisualPriority = prioriteAffichage
            };
            
            Tentacules = Persistence.GetAssetCopy<Sprite>("tentacules");
            Tentacules.Taille = 8;
            Tentacules.Origine = Tentacules.Centre;
            Tentacules.VisualPriority = Representation.VisualPriority + 0.01f;
        }


        //public void Show()
        //{
        //    Scene.Add(Representation);
        //    Scene.Add(Tentacules);
        //}


        //public void Hide()
        //{
        //    Scene.Remove(Representation);
        //    Scene.Remove(Tentacules);
        //}


        public void Update(GameTime gameTime)
        {
            Tentacules.Update();
        }


        public void Draw()
        {
            Tentacules.Position = Representation.Position;

            Scene.Add(Representation);
            Scene.Add(Tentacules);
        }
    }
}
