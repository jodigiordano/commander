namespace EphemereGames.Core.Visuel
{
    using System;
    using EphemereGames.Core.Utilities;


    public class SizeEffect : VisualEffect
    {
        public Trajet2D Path { get; set; }


        protected override void InitializeLogic() {}


        protected override void LogicLinear()
        {
            Object.TailleVecteur = Path.position(ElaspedTime);
        }


        protected override void LogicAfter()
        {
            Object.TailleVecteur = Path.position(ElaspedTime);
        }


        protected override void LogicNow()
        {
            Object.TailleVecteur = Path.position(Length);
        }
    }
}
