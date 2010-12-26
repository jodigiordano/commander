namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public class EffectsController
    {
        protected List<AbstractEffect> Effects { get; set; }


        public EffectsController()
        {
            Effects = new List<AbstractEffect>();
        }

        
        public int NbEffetsActifs
        {
            get
            {
                return Effects.Count;
            }
        }

        
        public virtual void Update(GameTime gameTime)
        {
            for (int i = Effects.Count - 1; i > -1; i--)
            {
                AbstractEffect effect = Effects[i];

                if (!effect.Finished)
                    Effects[i].Update(gameTime);
                else
                    Effects.RemoveAt(i);
            }
        }


        public void Add(object obj, AbstractEffect effect)
        {
            effect.Obj = obj;

            Effects.Add(effect);
        }

        
        public void Add(object obj, List<AbstractEffect> effects)
        {
            for (int i = 0; i < effects.Count; i++)
                Add(obj, effects[i]);
        }


        public void Stop()
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Finished = true;
                Effects[i].Update(null);
                Effects[i].Initialize();                
            }
        }


        public void Clear()
        {
            Effects.Clear();
        }


        public bool AllFinished(List<AbstractEffect> effects)
        {
            foreach (var effet in effects)
                if (!effet.Finished)
                    return false;

            return true;
        }
    }
}
