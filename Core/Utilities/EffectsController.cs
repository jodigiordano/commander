namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;


    public class EffectsController<T>
    {
        private Dictionary<int, Effect<T>> Effects;
        private Dictionary<int, List<T>> Objects;
        private List<int> ToDelete;
        private Pool<List<T>> PoolOfLists;


        public EffectsController()
        {
            Effects = new Dictionary<int,Effect<T>>();
            Objects = new Dictionary<int,List<T>>();
            ToDelete = new List<int>();
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
                Effect<T> effect = kvp.Value;
                List<T> objects = Objects[kvp.Key];

                effect.Update(elapsedTime);

                foreach (var obj in objects)
                {
                    effect.Obj = obj;
                    effect.UpdateObj();
                }

                if (effect.Terminated)
                    ToDelete.Add(kvp.Key);
            }

            foreach (var effectId in ToDelete)
            {
                Effect<T> effect = Effects[effectId];

                if (effect.TerminatedCallback != null && !effect.TerminatedOverride)
                    effect.TerminatedCallback(effect.Id);

                effect.Return();
                PoolOfLists.Return(Objects[effectId]);

                Effects.Remove(effectId);
                Objects.Remove(effectId);
            }
        }


        public int Add(T obj, Effect<T> effect, IntegerHandler callback)
        {
            if (Effects.ContainsKey(effect.Id))
                Objects[effect.Id].Add(obj);
            else
            {
                var l = PoolOfLists.Get();
                l.Clear();
                l.Add(obj);
                Effects.Add(effect.Id, effect);
                Objects.Add(effect.Id, l);

                effect.TerminatedCallback = callback;
                effect.Initialize();
            }

            return effect.Id;
        }


        public int Add(T obj, Effect<T> effect)
        {
            return Add(obj, effect, null);
        }


        public List<int> Add(T obj, List<Effect<T>> effects)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < effects.Count; i++)
                ids.Add(Add(obj, effects[i], null));

            return ids;
        }


        public void Clear()
        {
            foreach (var kvp in Effects)
            {
                var effect = kvp.Value;

                if (effect.TerminatedCallback != null && !effect.TerminatedOverride)
                    effect.TerminatedCallback(effect.Id);

                effect.Return();
                PoolOfLists.Return(Objects[kvp.Key]);
            }

            Objects.Clear();
            Effects.Clear();
            ToDelete.Clear();
        }


        public void StopAllKeepCurrentState()
        {
            Clear();
        }


        public void CancelEffect(int id)
        {
            Effect<T> effect;

            if (!Effects.TryGetValue(id, out effect))
                return;

            effect.Return();
            PoolOfLists.Return(Objects[id]);

            Effects.Remove(id);
            Objects.Remove(id);
        }


        public void CancelEffect(List<int> ids)
        {
            foreach (var id in ids)
                CancelEffect(id);
        }


        public void CancelCallback(int id)
        {
            Effect<T> effect;

            if (!Effects.TryGetValue(id, out effect))
                return;

            effect.TerminatedOverride = true;
        }
    }
}
