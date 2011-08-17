namespace EphemereGames.Commander
{
    using System;


    class News : IComparable<News>
    {
        public DateTime Date;
        public string Title;
        public string Description;


        public int CompareTo(News other)
        {
            return other.Date.CompareTo(Date);
        }
    }
}
