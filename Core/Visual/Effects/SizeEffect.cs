namespace EphemereGames.Core.Visual
{
    using System;
    using EphemereGames.Core.Utilities;


    public class SizeEffect : Effect<IVisual>
    {
        public Path2D Path { get; set; }

        public static Pool<SizeEffect> Pool = new Pool<SizeEffect>();


        protected override void InitializeLogic() {}


        protected override void LogicLinear()
        {
            Obj.Size = Path.GetPosition(ElaspedTime);
        }


        protected override void LogicAfter()
        {
            Obj.Size = Path.GetPosition(ElaspedTime);
        }


        protected override void LogicNow()
        {
            Obj.Size = Path.GetPosition(Length);
        }


        internal override void Return()
        {
            Pool.Return((SizeEffect) this);
        }
    }
}
