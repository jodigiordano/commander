namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class AnimationsController
    {
        private Scene Scene;
        private List<Animation> Animations = new List<Animation>();


        public AnimationsController(Scene scene)
        {
            Scene = scene;
        }


        public Animation First
        {
            get
            {
                if (Animations.Count > 0)
                    return Animations[0];
                
                else
                    return null;
            }
        }


        public List<Animation> Components
        {
            get { return Animations; }
        }


        public void Insert(Animation animation)
        {
            animation.Scene = Scene;
            animation.Initialize();

            Animations.Add(animation);
        }


        public void Insert(List<Animation> animations)
        {
            foreach (var animation in animations)
                Insert(animation);
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Animations.Count; i++)
            {
                if (Animations[i].Paused) { continue; }

                if (Animations[i].Finished(gameTime))
                {
                    Animations[i].Stop();
                    Animations.RemoveAt(i);
                    i--;
                    continue;
                }

                Animations[i].Update(gameTime);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Animations.Count; i++)
                Animations[i].Draw(spriteBatch);
        }


        public void PauseAll()
        {
            for (int i = 0; i < Animations.Count; i++)
                Animations[i].Paused = true;
        }


        public void Clear()
        {
            Animations.Clear();
        }
    }
}
