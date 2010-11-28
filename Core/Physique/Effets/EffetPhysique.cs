namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;
    
    public class EffetPhysique : AbstractEffet, ICloneable
    {
        [ContentSerializerIgnore]
        public IPhysique Objet
        {
            get
            {
                return (IPhysique)objet;
            }

            set
            {
                objet = value;
            }
        }


        #region ICloneable Members

        public virtual object Clone()
        {
            EffetPhysique ev = (EffetPhysique)this.MemberwiseClone();
            ev.Termine = false;

            return ev;
        }

        #endregion
    }
}
