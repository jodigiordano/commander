/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using System;
    using ProjectMercury.EffectEditor.PluginInterfaces;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;

    public class NewModifierEventArgs : EventArgs
    {
        public NewModifierEventArgs(Emitter parentEmitter, IModifierPlugin plugin)
            : base()
        {
            this.ParentEmitter = parentEmitter;
            this.Plugin = plugin;
        }

        public Emitter ParentEmitter { get; private set; }

        public IModifierPlugin Plugin { get; private set; }

        public Modifier AddedModifier { get; set; }
    };
}