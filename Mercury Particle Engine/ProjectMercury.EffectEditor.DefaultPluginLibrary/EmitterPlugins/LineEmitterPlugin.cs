﻿/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor.DefaultPluginLibrary.Emitterplugins
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using Microsoft.Xna.Framework;
    using ProjectMercury.EffectEditor.PluginInterfaces;
    using ProjectMercury.Emitters;

    [Export(typeof(IEmitterPlugin))]
    public class LineEmitterPlugin : IEmitterPlugin
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "Line Emitter"; }
        }

        /// <summary>
        /// Gets the author of the plugin.
        /// </summary>
        public string Author
        {
            get { return "Matt Davey"; }
        }

        /// <summary>
        /// Gets the name of the plugin library, if any.
        /// </summary>
        public string Library
        {
            get { return "DefaultPluginLibrary"; }
        }

        /// <summary>
        /// Gets the version numbe of the plugin.
        /// </summary>
        public Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        /// <summary>
        /// Gets the minimum version of the engine with which the plugin is compatible.
        /// </summary>
        public Version MinimumRequiredVersion
        {
            get { return new Version(3, 1, 0, 0); }
        }

        /// <summary>
        /// Gets the display name for the Emitter type provided by the plugin.
        /// </summary>
        public string DisplayName
        {
            get { return "Line Emitter"; }
        }

        /// <summary>
        /// Gets the description for the Emitter type provided by the plugin.
        /// </summary>
        public string Description
        {
            get { return "An Emitter which releases Particles at a random point along a line."; }
        }

        /// <summary>
        /// Gets the icon to display for the Emitter type provided by the plugin.
        /// </summary>
        public Icon DisplayIcon
        {
            get { return Icons.Emitter; }
        }

        /// <summary>
        /// Creates a default instance of the Emitter type provided by the plugin.
        /// </summary>
        /// <returns>An instance of the Emitter type provided by the plugin.</returns>
        public Emitter CreateDefaultInstance()
        {
            return new LineEmitter
            {
                BlendMode = BlendMode.Add,
                Budget = 1000,
                ReleaseColour = Vector3.One,
                ReleaseOpacity = 1f,
                ReleaseQuantity = 1,
                ReleaseRotation = 0f,
                ReleaseScale = 16f,
                ReleaseSpeed = 50f,
                Term = 2f,

                Angle = 0f,
                EmitBothWays = false,
                Length = 150,
                Rectilinear = false
            };
        }

        /// <summary>
        /// Recreates the Emitter with the provided new values for Budget and Term.
        /// </summary>
        /// <param name="existingInstance">The existing instance of the Emitter.</param>
        /// <param name="budget">The required Budget value for the recreated instance.</param>
        /// <param name="term">The required Term value for the recreated instance.</param>
        /// <returns>A clone of the existing Emitter instance with the new values for Budget and Term.</returns>
        public Emitter Recreate(Emitter existingInstance, int budget, float term)
        {
            LineEmitter e = (LineEmitter)existingInstance;

            return new LineEmitter
            {
                BlendMode = existingInstance.BlendMode,
                Budget = budget,
                ReleaseColour = existingInstance.ReleaseColour,
                ReleaseOpacity = existingInstance.ReleaseOpacity,
                ReleaseQuantity = existingInstance.ReleaseQuantity,
                ReleaseRotation = existingInstance.ReleaseRotation,
                ReleaseScale = existingInstance.ReleaseScale,
                ReleaseSpeed = existingInstance.ReleaseSpeed,
                Term = term,

                Angle = e.Angle,
                EmitBothWays = e.EmitBothWays,
                Length = e.Length,
                Rectilinear = e.Rectilinear
            };
        }
    }
}