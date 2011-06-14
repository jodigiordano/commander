namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class FadeColorEffect : AbstractEffect
    {
        public Path2D Path;
        private bool IsImage;
        private bool IsText;


        protected override void InitializeLogic()
        {
            IsImage = Obj as Image != null;
            IsText = Obj as Text != null;

            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.positionDepart().Y * 255);
            else if (IsText)
                ((Text) Obj).Color.A = (byte) (Path.positionDepart().Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.positionDepart().Y * 255);
        }


        protected override void LogicLinear()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(ElaspedTime).Y * 255);
            else if (IsText)
                ((Text) Obj).Color.A = (byte) (Path.position(ElaspedTime).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(ElaspedTime).Y * 255);
        }


        protected override void LogicAfter()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else if (IsText)
                ((Text) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(Length).Y * 255);
        }


        protected override void LogicNow()
        {
            if (IsImage)
                ((Image) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else if (IsText)
                ((Text) Obj).Color.A = (byte) (Path.position(Length).Y * 255);
            else
                ((IVisible) Obj).Couleur.A = (byte) (Path.position(Length).Y * 255);
        }
    }
}
