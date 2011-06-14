namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Utilities;


    public class FadeColorEffect : Effect<IVisual>
    {
        public Path2D Path;

        public static Pool<FadeColorEffect> Pool = new Pool<FadeColorEffect>();


        protected override void InitializeLogic()
        {
            Obj.Alpha = (byte) (Path.positionDepart().Y * 255);
        }


        protected override void LogicLinear()
        {
            Obj.Alpha = (byte) (Path.position(ElaspedTime).Y * 255);
        }


        protected override void LogicAfter()
        {
            Obj.Alpha = (byte) (Path.position(Length).Y * 255);
        }


        protected override void LogicNow()
        {
            Obj.Alpha = (byte) (Path.position(Length).Y * 255);
        }


        internal override void Return()
        {
            Pool.Return((FadeColorEffect) this);
        }
    }
}
