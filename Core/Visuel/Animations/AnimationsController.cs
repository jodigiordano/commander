namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class AnimationsController
    {
        private List<Animation> animations = new List<Animation>();


        public Animation First
        {
            get
            {
                if (animations.Count > 0)
                    return animations[0];
                
                else
                    return null;
            }
        }


        public List<Animation> Components
        {
            get { return animations; }
        }


        public void Insert(Scene scene, Animation animation)
        {
            animation.Scene = scene;
            animation.Initialize();

            animations.Add(animation);
        }


        public void Insert(Scene scene, List<Animation> animations)
        {
            foreach (var animation in animations)
                Insert(scene, animation);
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].Paused) { continue; }

                if (animations[i].Finished(gameTime))
                {
                    animations[i].Stop();
                    animations.RemoveAt(i);
                    i--;
                    continue;
                }

                animations[i].Update(gameTime);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < animations.Count; i++)
                animations[i].Draw(spriteBatch);
        }


        public void PauseAll()
        {
            for (int i = 0; i < animations.Count; i++)
                animations[i].Paused = true;
        }


        public void Clear()
        {
            animations.Clear();
        }
    }
}
