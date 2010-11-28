/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor.PluginInterfaces
{
    using System;
    using System.Drawing;
    using ProjectMercury.Emitters;

    /// <summary>
    /// Defines the interface for an Emitter plugin.
    /// </summary>
    public interface IEmitterPlugin : IPlugin
    {
        /// <summary>
        /// Creates a default instance of the Emitter type provided by the plugin.
        /// </summary>
        /// <returns>An instance of the Emitter type provided by the plugin.</returns>
        Emitter CreateDefaultInstance();

        /// <summary>
        /// Recreates the Emitter with the provided new values for Budget and Term.
        /// </summary>
        /// <param name="existingInstance">The existing instance of the Emitter.</param>
        /// <param name="budget">The required Budget value for the recreated instance.</param>
        /// <param name="term">The required Term value for the recreated instance.</param>
        /// <returns>A clone of the existing Emitter instance with the new values for Budget and Term.</returns>
        [Obsolete("Emitter can now be re-initialised directly.", true)]
        Emitter Recreate(Emitter existingInstance, int budget, float term);
    }
}