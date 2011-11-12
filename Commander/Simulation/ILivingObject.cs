namespace EphemereGames.Commander.Simulation
{
    public interface ILivingObject
    {
        float LifePoints    { get; set; }
        float AttackPoints  { get; set; }
        bool  Alive         { get;      }

        void DoHit(ILivingObject by);
        void DoDie();
    }
}
