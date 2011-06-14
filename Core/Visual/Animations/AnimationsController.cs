namespace EphemereGames.Core.Visual
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


        public void Add(Animation a)
        {
            a.Scene = Scene;
            a.Initialize();
            a.Start();

            Animations.Add(a);
        }


        public void Add(List<Animation> animations)
        {
            foreach (var a in animations)
                Add(a);
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


        public void Draw()
        {
            foreach (var a in Animations)
                a.Draw();
        }


        public void PauseAll()
        {
            foreach (var a in Animations)
                a.Paused = true;
        }


        public void Clear()
        {
            foreach (var a in Animations)
                a.Stop();

            Animations.Clear();
        }
    }
}
