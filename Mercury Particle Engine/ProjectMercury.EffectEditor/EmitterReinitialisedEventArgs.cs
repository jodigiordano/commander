﻿/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using System;
    using ProjectMercury.Emitters;

    public class EmitterReinitialisedEventArgs : EventArgs
    {
        public EmitterReinitialisedEventArgs(Emitter emitter, int budget, float term)
            : base()
        {
            this.Emitter = emitter;
            this.Budget = budget;
            this.Term = term;
        }

        public Emitter Emitter { get; private set; }

        public int Budget { get; private set; }

        public float Term { get; private set; }
    }
}