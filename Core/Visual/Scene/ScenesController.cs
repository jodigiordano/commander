namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class ScenesController
    {
        private Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();
        private Camera Camera = new Camera();

        public SpriteBatch SpriteBatch { get; set; }
        public VisualBuffer Buffer;


        public ScenesController()
        {
            Buffer = new VisualBuffer(Preferences.WindowWidth, Preferences.WindowHeight);
            Camera.Origin = new Vector2(Preferences.WindowWidth / 2.0f, Preferences.WindowHeight / 2.0f);
            Scenes.Clear();

            SpriteBatch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
        }


        public Scene UpdateScene(string sceneName, Scene scene)
        {
            Scene actualScene = Scenes[sceneName];

            if (actualScene != null)
                actualScene.Dispose();

            Scenes[sceneName] = scene;

            return scene;
        }


        public void AddScene(Scene scene)
        {
            Scenes[scene.Name] = scene;
        }


        public Scene GetScene(string sceneName)
        {
            if (!Scenes.ContainsKey(sceneName))
                return null;

            return Scenes[sceneName];
        }


        public void Update(GameTime gameTime)
        {
            string[] wCles = Scenes.Keys.ToArray();

            for (int i = 0; i < wCles.Length; i++)
            {
                Scene scene = Scenes[wCles[i]];

                if (scene != null && scene.EnableUpdate)
                    scene.Update(gameTime);
            }
        }


        public void Draw()
        {
            foreach (var scene in Scenes.Values)
                if (scene != null && scene.EnableVisuals)
                    scene.Draw();

            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            foreach (var scene in Scenes.Values)
                if (scene != null && scene.EnableVisuals)
                {
                    SpriteBatch.Begin(
                        SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.LinearClamp,
                        DepthStencilState.None,
                        RasterizerState.CullNone,
                        null,
                        Camera.Transform);

                    scene.Draw(SpriteBatch);

                    SpriteBatch.End();
                }
        }
    }
}
