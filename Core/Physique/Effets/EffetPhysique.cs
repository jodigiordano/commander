namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using EphemereGames.Core.Utilities;
    
    public class EffetPhysique : AbstractEffect, ICloneable
    {
        [ContentSerializerIgnore]
        public IPhysique Objet
        {
            get
            {
                return (IPhysique)Obj;
            }

            set
            {
                Obj = value;
            }
        }


        #region ICloneable Members

        public virtual object Clone()
        {
            EffetPhysique ev = (EffetPhysique)this.MemberwiseClone();
            ev.Finished = false;

            return ev;
        }

        #endregion
    }
}
