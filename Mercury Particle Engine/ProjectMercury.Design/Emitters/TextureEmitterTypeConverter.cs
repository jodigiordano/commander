/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Design.Emitters
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using ProjectMercury.Emitters;
    using ProjectMercury.Design.UITypeEditors;

    public class TextureEmitterTypeConverter : EmitterTypeConverter
    {
        /// <summary>
        /// Adds the descriptors.
        /// </summary>
        /// <param name="descriptors">The descriptors.</param>
        protected override void AddDescriptors(List<SmartMemberDescriptor> descriptors)
        {
            base.AddDescriptors(descriptors);

            var type = typeof(TextureEmitter);

            descriptors.AddRange(new SmartMemberDescriptor[]
            {
                new SmartMemberDescriptor(type.GetProperty("Rotation"),
                    new CategoryAttribute("Texture Emitter"),
                    new EditorAttribute(typeof(AngleEditor), typeof(UITypeEditor)),
                    new DescriptionAttribute("The Rotation of the texture in radians.")),

                new SmartMemberDescriptor(type.GetProperty("Scale"),
                    new CategoryAttribute("Texture Emitter"),
                    new DescriptionAttribute("The scale of the texture.")),

                new SmartMemberDescriptor(type.GetProperty("ApplyPixelColours"),
                    new CategoryAttribute("Texture Emitter"),
                    new DescriptionAttribute("True if the copy should apply colours from the texture.")),

                new SmartMemberDescriptor(type.GetProperty("Threshold"),
                    new CategoryAttribute("Texture Emitter"),
                    new DescriptionAttribute("The threshold over which pixels will trigger the release of particles.")),

                new SmartMemberDescriptor(type.GetProperty("Texture"),
                    //new EditorAttribute(typeof(TexturePicker), typeof(UITypeEditor)),
                    new CategoryAttribute("Texture Emitter"),
                    new DescriptionAttribute("The texture to show."))
            });
        }
    }
}