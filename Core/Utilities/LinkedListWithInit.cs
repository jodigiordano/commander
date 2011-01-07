namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;
 

    public class LinkedListWithInit<T> : LinkedList<T>
    {
        public void Add(T item)
        {
            ((ICollection<T>) this).Add(item);
        }
    }
}