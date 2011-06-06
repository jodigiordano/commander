namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;


    public class Pool<T> where T : new()
    {
        private Stack<T> objects;

        
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


        private void Initialize(List<T> objs)
        {
            objects = new Stack<T>(objs);
        }


        public T Get()
        {
            if (objects.Count > 0)
                return objects.Pop();

            return new T();
        }


        public void Return(T obj)
        {
            objects.Push(obj);
        }
    }

}
