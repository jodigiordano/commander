namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;

    public class EffetVisuel : AbstractEffet, ICloneable
    {
        [ContentSerializerIgnore]
        public IVisible Objet
        {
            get
            {
                return (IVisible)objet;
            }

            set
            {
                objet = value;
            }
        }


        #region ICloneable Members

        public virtual object Clone()
        {
            EffetVisuel ev = (EffetVisuel)this.MemberwiseClone();
            ev.Termine = false;

            return ev;
        }

        #endregion
    }
}
