/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using System;
    using System.Collections.Generic;
    using ProjectMercury.EffectEditor.PluginInterfaces;
    using ProjectMercury.Renderers;

    public delegate void SerializeEventHandler              (object sender, SerializeEventArgs e);
    public delegate void NewEmitterEventHandler             (object sender, NewEmitterEventArgs e);
    public delegate void CloneEmitterEventHandler           (object sender, CloneEmitterEventArgs e);
    public delegate void EmitterEventHandler                (object sender, EmitterEventArgs e);
    public delegate void NewModifierEventHandler            (object sender, NewModifierEventArgs e);
    public delegate void CloneModifierEventHandler          (object sender, CloneModifierEventArgs e);
    public delegate void ModifierEventHandler               (object sender, ModifierEventArgs e);
    public delegate void EmitterReinitialisedEventHandler   (object sender, EmitterReinitialisedEventArgs e);
    public delegate void NewTextureReferenceEventHandler    (object sender, NewTextureReferenceEventArgs e);
    public delegate void TextureReferenceChangedEventHandler(object sender, TextureReferenceChangedEventArgs e);

    public interface IInterfaceProvider : IDisposable
    {
        event EventHandler                          Ready;
        event SerializeEventHandler                 Serialize;
        event SerializeEventHandler                 Deserialize;
        event NewEmitterEventHandler                EmitterAdded;
        event CloneEmitterEventHandler              EmitterCloned;
        event EmitterEventHandler                   EmitterRemoved;
        event NewModifierEventHandler               ModifierAdded;
        event CloneModifierEventHandler             ModifierCloned;
        event ModifierEventHandler                  ModifierRemoved;
        event EmitterReinitialisedEventHandler      EmitterReinitialised;
        event NewTextureReferenceEventHandler       TextureReferenceAdded;
        event TextureReferenceChangedEventHandler   TextureReferenceChanged;
        event EventHandler                          NewParticleEffect;

        bool TriggerRequired(out float x, out float y);
        void Draw(ParticleEffect effect, Renderer renderer);

        void SetEmitterPlugins      (IEnumerable<IEmitterPlugin> plugins);
        void SetModifierPlugins     (IEnumerable<IModifierPlugin> plugins);
        void SetSerializationPlugins(IEnumerable<IEffectSerializationPlugin> plugins);
        void SetParticleEffect      (ParticleEffect effect);

        IEnumerable<TextureReference> TextureReferences { get; set; }
    }
}