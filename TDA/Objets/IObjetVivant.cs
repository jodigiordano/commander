namespace TDA
{
    interface IObjetVivant
    {
        float PointsVie      { get; set; }
        float PointsAttaque  { get; set; }
        bool  EstVivant      { get;      }

        void doTouche(IObjetVivant par);
        void doMeurt();
    }
}
