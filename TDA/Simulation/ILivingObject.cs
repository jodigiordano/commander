namespace EphemereGames.Commander
{
    interface ILivingObject
    {
        float LifePoints    { get; set; }
        float AttackPoints  { get; set; }
        bool  Alive         { get;      }

        void DoHit(ILivingObject par);
        void DoDie();
    }
}
