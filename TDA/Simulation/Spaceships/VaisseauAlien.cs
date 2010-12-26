namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class VaisseauAlien
    {
        private Scene Scene;
        public Sprite Tentacules;
        public IVisible Representation;


        public VaisseauAlien(Scene scene, float prioriteAffichage)
        {
            Scene = scene;

            Representation = new IVisible
            (
                EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("VaisseauAlien"),
                Vector3.Zero
            );
            Representation.Taille = 8;
            Representation.Origine = Representation.Centre;
            Representation.VisualPriority = prioriteAffichage;

            Tentacules = EphemereGames.Core.Persistance.Facade.GetAssetCopy<Sprite>("tentacules");
            Tentacules.Taille = 8;
            Tentacules.Origine = Tentacules.Centre;
            Tentacules.VisualPriority = Representation.VisualPriority + 0.01f;
        }


        public void Update(GameTime gameTime)
        {
            Tentacules.Update(gameTime);
        }


        public void Draw(GameTime gameTime)
        {
            Tentacules.Position = Representation.Position;

            Scene.ajouterScenable(Representation);
            Scene.ajouterScenable(Tentacules);
        }
    }
}
