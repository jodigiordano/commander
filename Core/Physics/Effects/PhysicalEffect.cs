namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using EphemereGames.Core.Utilities;
    
    public class PhysicalEffect : AbstractEffect
    {
        [ContentSerializerIgnore]
        public IPhysicalObject Objet
        {
            get
            {
                return (IPhysicalObject)Obj;
            }

            set
            {
                Obj = value;
            }
        }


        #region ICloneable Members

        public virtual object Clone()
        {
            PhysicalEffect ev = (PhysicalEffect)this.MemberwiseClone();
            ev.Finished = false;

            return ev;
        }

        #endregion
    }
}
