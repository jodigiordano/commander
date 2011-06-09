namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class AnimationsController
    {
        public List<Animation> Animations { get; private set; }

        private Scene Scene;


        public AnimationsController(Scene scene)
        {
            Scene = scene;
            Animations = new List<Animation>();
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


        public void Add(Animation animation)
        {
            animation.Scene = Scene;
            animation.Initialize();
            animation.Start();

            Animations.Add(animation);
        }


        public void Add(List<Animation> animations)
        {
            foreach (var animation in animations)
                Add(animation);
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Animations.Count; i++)
            {
                if (Animations[i].Paused)
                {
                    continue;
                }

                if (Animations[i].Finished(gameTime))
                {
                    Animations[i].Stop();
                    Animations.RemoveAt(i);
                    i--;
                    continue;
                }

                else
                {
                    Animations[i].Update(gameTime);
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Animations.Count; i++)
                Animations[i].Draw();
        }


        public void PauseAll()
        {
            for (int i = 0; i < Animations.Count; i++)
                Animations[i].Paused = true;
        }


        public void Clear()
        {
            foreach (var animation in Animations)
                animation.Stop();

            Animations.Clear();
        }
    }
}
