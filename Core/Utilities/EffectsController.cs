namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;


    public class EffectsController<T>
    {
        private Dictionary<Effect<T>, List<T>> Effects;
        private List<KeyValuePair<Effect<T>, List<T>>> ToDelete;
        private Pool<List<T>> PoolOfLists;


        public EffectsController()
        {
            Effects = new Dictionary<Effect<T>, List<T>>();
            ToDelete = new List<KeyValuePair<Effect<T>,List<T>>>();
            PoolOfLists = new Pool<List<T>>();
        }

        
        public int ActiveEffectsCount
        {
            get
            {
                return Effects.Count;
            }
        }

        
        public virtual void Update(float elapsedTime)
        {
            ToDelete.Clear();

            foreach (var kvp in Effects)
            {
                Effect<T> effect = kvp.Key;

                effect.Update(elapsedTime);

                foreach (var obj in kvp.Value)
                {
                    effect.Obj = obj;
                    effect.UpdateObj();
                }

                if (effect.Terminated)
                    ToDelete.Add(kvp);
            }

            foreach (var kvp in ToDelete)
            {
                Effect<T> effect = kvp.Key;

                if (effect.TerminatedCallback != null && !effect.TerminatedOverride)
                    effect.TerminatedCallback();

                effect.Return();
                PoolOfLists.Return(kvp.Value);

                Effects.Remove(effect);
            }
        }


        public Effect<T> Add(T obj, Effect<T> effect, NoneHandler callback)
        {
            if (Effects.ContainsKey(effect))
                Effects[effect].Add(obj);
            else
            {
                var l = PoolOfLists.Get();
                l.Clear();
                l.Add(obj);
                Effects.Add(effect, l);

                effect.TerminatedCallback = callback;
                effect.Initialize();
            }

            return effect;
        }


        public Effect<T> Add(T obj, Effect<T> effect)
        {
            return Add(obj, effect, null);
        }


        public void Add(T obj, List<Effect<T>> effects)
        {
            for (int i = 0; i < effects.Count; i++)
                Add(obj, effects[i], null);
        }


        public void Clear()
        {
            foreach (var kvp in Effects)
            {
                if (kvp.Key.TerminatedCallback != null && !kvp.Key.TerminatedOverride)
                    kvp.Key.TerminatedCallback();

                kvp.Key.Return();
                PoolOfLists.Return(kvp.Value);
            }

            Effects.Clear();
            ToDelete.Clear();
        }
    }
}
