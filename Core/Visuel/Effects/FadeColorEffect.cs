namespace EphemereGames.Core.Visuel
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class FadeColorEffect : AbstractEffect
    {
        public Trajet2D Path;
        private bool IsImage;


        protected override void InitializeLogic()
        {
            var iv = Obj as IVisible;

            IsImage = iv == null;

            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.positionDepart().Y * 255);
            else
                iv.Couleur.A = (byte) (Path.positionDepart().Y * 255);
        }


        protected override void LogicLinear()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(ElaspedTime).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(ElaspedTime).Y * 255);
        }


        protected override void LogicAfter()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(Length).Y * 255);
        }


        protected override void LogicNow()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(Length).Y * 255);
        }
    }
}
