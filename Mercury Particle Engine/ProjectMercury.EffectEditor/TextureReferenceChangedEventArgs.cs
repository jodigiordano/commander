﻿/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using ProjectMercury.Emitters;

    public class TextureReferenceChangedEventArgs : EmitterEventArgs
    {
        public TextureReferenceChangedEventArgs(Emitter emitter, TextureReference textureReference)
            : base(emitter)
        {
            this.TextureReference = textureReference;
        }

        public TextureReference TextureReference { get; private set; }
    };
}