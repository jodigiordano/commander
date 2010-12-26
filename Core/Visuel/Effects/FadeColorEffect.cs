namespace EphemereGames.Core.Visuel
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class FadeColorEffect : VisualEffect
    {
        public Trajet2D Path { get; set; }


        protected override void InitializeLogic()
        {
            Object.Couleur.A = (byte) (Path.positionDepart().Y * 255);
        }

        protected override void LogicLinear()
        {
            Object.Couleur.A = (byte) (Path.position(ElaspedTime).Y * 255);
        }

        protected override void LogicAfter()
        {
            Object.Couleur.A = (byte) (Path.position(Length).Y * 255);
        }

        protected override void LogicNow()
        {
            Object.Couleur.A = (byte) (Path.position(Length).Y * 255);
        }
    }
}
