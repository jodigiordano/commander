namespace EphemereGames.Core.Visual
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Content;


    public class VisualEffect : AbstractEffect
    {
        [ContentSerializerIgnore]
        public IVisible Object
        {
            get { return (IVisible)Obj; }
            set { Obj = value; }
        }


        public virtual object Clone()
        {
            VisualEffect ev = (VisualEffect)this.MemberwiseClone();
            ev.Finished = false;

            return ev;
        }
    }
}
