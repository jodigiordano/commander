//=====================================================================
// 
// Pool d'objets
//
//=====================================================================

namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;

    public class Pool<T> : ICloneable where T : new()
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        private Stack<T> pile;


        //=====================================================================
        // Constructeurs
        //=====================================================================

        public Pool()
        {
            pile = new Stack<T>();
        }

        public Pool(int taille)
        {
            pile = new Stack<T>(taille);

            for (int i = 0; i < taille; i++)
                pile.Push(new T());
        }


        /// <summary>
        /// Initialise le pool avec une liste d'objets
        /// </summary>
        /// <param name="objets"></param>
        public Pool(T[] objets)
        {
            pile = new Stack<T>(objets.Length);

            for (int i = 0; i < objets.Length; i++)
                pile.Push(objets[i]);
        }


        //=====================================================================
        // Logique
        //=====================================================================

        public T recuperer()
        {
            return (pile.Count > 0) ? pile.Pop() : new T();
        }

        public void retourner(T objet)
        {
            pile.Push(objet);
        }

        public object Clone()
        {
            Pool<T> pool = new Pool<T>();

            foreach (var objet in pile)
                pool.pile.Push(objet);

            return pool;
        }
    }

}
