//=============================================================================
//
// Englobe un ProjectMercury.ParticleEffect et lui ajoute des propriétés
// pour être utilisé dans notre système d'affichage
//
//=============================================================================

namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Persistance;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;

    public class ParticuleEffectWrapper : IScenable, IContenu
    {

        //=============================================================================
        // Attributs, Getters, Setters
        //=============================================================================

        /// <summary>
        /// Nom du système de particules
        /// </summary>
        public string Nom                       { get; set; }

        /// <summary>
        /// Liste des émetteurs
        /// </summary>
        public List<IScenable> Components       { get; set; }


        /// <summary>
        /// Priorité d'affichage pour un même Z
        /// </summary>
        public float VisualPriority { get; set; }



        /// <summary>
        /// Position dont seule le Z a du sens.
        /// </summary>
        /// <remarks>
        /// N'aura plus de sens si on veut que les particules soient 3D.
        /// Dans ce cas il va falloir qu'au trigger, on passe un Vector3D
        /// et qu'au niveau de ProjectMercury qu'on garde une position 3D
        /// de chaque particule pour ensuite fait un tri + transformation
        /// à la Scène. PAIN IN THE ASS qu'on se garde pour plus tard (lire: jamais)
        /// </remarks>
        public Vector3 Position { get; set; }


        /// <summary>
        /// Mélange utilisé par le système
        /// </summary>
        private TypeBlend melange;
        public TypeBlend Blend
        {
            get { return melange; }
            set
            {
                melange = value;

                if (ParticleEffect == null)
                    return;

                foreach (var effet in ParticleEffect)
                    effet.BlendMode = convertirTypeMelange(value);
            }
        }


        //public TypeMelange Melange     { get; set; }

        /// <summary>
        /// Scène dans laquelle sont émises les particules
        /// </summary>
        public Scene Scene { get; set; }


        public ProjectMercury.Renderers.SpriteBatchRenderer Renderer;

        /// <summary>
        /// Le ParticleEffect du projet Mercury, qui est composé de 1 ou plusieurs Emitters gérés par EmitterWrapper
        /// </summary>
        /// <remarks>
        /// Utiliser un système d'allocation de priorité parce que
        /// en ce moment tous les composants ont la même priorité, ce
        /// qui créer une race condition lors de l'affichage (et donc un flickering)
        /// Les composants au début de la liste devraient avoir une priorité plus grande
        /// (donc errr plus petite) que ceux en fin de liste
        /// </remarks>
        public ProjectMercury.ParticleEffect ParticleEffect { get; set; }



        //=============================================================================
        // Constructeur
        //=============================================================================

        public ParticuleEffectWrapper()
        {
            Nom = "";
            Components = null;
            Position = Vector3.Zero;
            Blend = TypeBlend.Add;
            VisualPriority = 0;
        }


        public void Initialize()
        {
            //todo: clearer les particules existantes.

            //if (ParticleEffect == null)
            //{
                ProjectMercury.ParticleEffect effetModele = EphemereGames.Core.Persistance.Facade.GetAsset<ProjectMercury.ParticleEffect>(Nom);

                ParticleEffect = effetModele.DeepCopy();

                // doit être fait ici car l'allocation des tableaux de particules se fait dans cette méthode
                ParticleEffect.Initialise();
            //}
        }


        //=============================================================================
        // Logique
        //=============================================================================

        /// <summary>
        /// Émettre une ou plusieurs particules du système
        /// </summary>
        /// <param name="position">Position d'où sont émises les particules</param>
        public void Emettre(ref Vector2 position)
        {
            this.ParticleEffect.Trigger(ref position);
        }


        /// <summary>
        /// Émettre une ou plusieurs particules du système
        /// </summary>
        /// <param name="position">Position d'où sont émises les particules</param>
        public void Emettre(ref Vector3 position)
        {
            Vector2 position2d = new Vector2(position.X, position.Y);

            this.ParticleEffect.Trigger(ref position2d);
        }


        /// <summary>
        /// Déplacer un effet de particule de +varPosition+
        /// </summary>
        /// <param name="position"></param>
        public void Deplacer(ref Vector2 varPosition)
        {
            for (int i = 0; i < ParticleEffect.Count; i++)
                for (int j = 0; j < ParticleEffect[i].ActiveParticlesCount; j++)
                    Vector2.Add(ref ParticleEffect[i].Particles[j].Position, ref varPosition, out ParticleEffect[i].Particles[j].Position);
        }


        /// <summary>
        /// Déplacer un effet de particule de +varPosition+
        /// </summary>
        /// <param name="position"></param>
        public void Deplacer(ref Vector3 varPosition)
        {
            Vector2 position2d = new Vector2(varPosition.X, varPosition.Y);

            Deplacer(ref position2d);
        }


        /// <summary>
        /// Conversion du système de blend mode Mercury vers notre système de blend mode
        /// </summary>
        /// <param name="blendMode">Blend mode du projet Mercury</param>
        /// <returns>Blend mode de notre système</returns>
        private static TypeBlend convertirBlendMode(ProjectMercury.Emitters.EmitterBlendMode blendMode)
        {
            TypeBlend melange;

            switch (blendMode)
            {
                case ProjectMercury.Emitters.EmitterBlendMode.Add:          melange = TypeBlend.Add;        break;
                case ProjectMercury.Emitters.EmitterBlendMode.Alpha:        melange = TypeBlend.Alpha;          break;
                case ProjectMercury.Emitters.EmitterBlendMode.None:         melange = TypeBlend.None;          break;
                default:                                                    melange = TypeBlend.Add;        break;
            }

            return melange;
        }

        private static ProjectMercury.Emitters.EmitterBlendMode convertirTypeMelange(TypeBlend melange)
        {
            ProjectMercury.Emitters.EmitterBlendMode blendMode;

            switch (melange)
            {
                case TypeBlend.Add:          blendMode = ProjectMercury.Emitters.EmitterBlendMode.Add;        break;
                case TypeBlend.Alpha:             blendMode = ProjectMercury.Emitters.EmitterBlendMode.Alpha;     break;
                case TypeBlend.None:             blendMode = ProjectMercury.Emitters.EmitterBlendMode.None;      break;
                default:                            blendMode = ProjectMercury.Emitters.EmitterBlendMode.Add;       break;
            }

            return blendMode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.Renderer.RenderEffect(this.ParticleEffect, ref Scene.Camera.Transform);
        }

        public string TypeAsset
        {
            get { return "Particules"; }
        }

        public object charger(string nom, string chemin, Dictionary<string, string> parametres, Microsoft.Xna.Framework.Content.ContentManager contenu)
        {
            // charge la particule à partir du fichier XML (une seule fois)
            ProjectMercury.ParticleEffect particule = contenu.Load<ProjectMercury.ParticleEffect>(chemin);

            // charge les images qui doivent (pour l'instant) se trouver dans WindowsContent
            particule.LoadContent(contenu);

            return particule;
        }

        public object Clone()
        {
            return this;
        }
    }
}
