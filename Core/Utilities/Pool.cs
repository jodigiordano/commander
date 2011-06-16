namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;


    public class Pool<T> where T : new()
    {
        public int MaxInstances { get; set; }
        public int LiveInstances { get; private set; }

        private Stack<T> Objects;

        
        public Pool() : this(0) {}


        public Pool(int size)
        {
            List<T> objs = new List<T>(size);

            for (int i = 0; i < size; i++)
                objs.Add(new T());

            Initialize(objs);
        }


        public Pool(List<T> objs)
        {
            Initialize(objs);
        }


        public T Get()
        {
            if (LiveInstances >= MaxInstances)
                return default(T);

            LiveInstances++;

            if (Objects.Count > 0)
                return Objects.Pop();

            return new T();
        }


        public void Return(T obj)
        {
            Objects.Push(obj);

            LiveInstances--;
        }


        private void Initialize(List<T> objs)
        {
            Objects = new Stack<T>(objs);
            MaxInstances = Int32.MaxValue;
            LiveInstances = 0;
        }
    }

}
