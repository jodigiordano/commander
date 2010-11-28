//=============================================================================
//
// Englobe un ProjectMercury.ParticleEffect et lui ajoute des propriétés
// pour être utilisé dans notre système d'affichage
//
//=============================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Core.Persistance;
    using Core.Utilities;
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
        public List<IScenable> Composants       { get; set; }


        /// <summary>
        /// Priorité d'affichage pour un même Z
        /// </summary>
        public float PrioriteAffichage { get; set; }



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
        private TypeMelange melange;
        public TypeMelange Melange
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


        public ProjectMercury.Renderers.PointSpriteRenderer Renderer;

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
            Composants = null;
            Position = Vector3.Zero;
            Melange = TypeMelange.Additif;
            PrioriteAffichage = 0;
        }


        public void Initialize()
        {
            //todo: clearer les particules existantes.

            //if (ParticleEffect == null)
            //{
                ProjectMercury.ParticleEffect effetModele = Core.Persistance.Facade.recuperer<ProjectMercury.ParticleEffect>(Nom);

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
        private static TypeMelange convertirBlendMode(ProjectMercury.BlendMode blendMode)
        {
            TypeMelange melange;

            switch (blendMode)
            {
                case ProjectMercury.BlendMode.Add:          melange = TypeMelange.Additif;        break;
                case ProjectMercury.BlendMode.Alpha:        melange = TypeMelange.Alpha;          break;
                case ProjectMercury.BlendMode.Multiply:     melange = TypeMelange.Multiplicatif;  break;
                case ProjectMercury.BlendMode.None:         melange = TypeMelange.Aucun;          break;
                case ProjectMercury.BlendMode.Subtract:     melange = TypeMelange.Soustraire;     break;
                default:                                    melange = TypeMelange.Additif;        break;
            }

            return melange;
        }

        private static ProjectMercury.BlendMode convertirTypeMelange(TypeMelange melange)
        {
            ProjectMercury.BlendMode blendMode;

            switch (melange)
            {
                case TypeMelange.Additif:          blendMode = ProjectMercury.BlendMode.Add;        break;
                case TypeMelange.Alpha:             blendMode = ProjectMercury.BlendMode.Alpha;     break;
                case TypeMelange.Multiplicatif:     blendMode = ProjectMercury.BlendMode.Multiply;  break;
                case TypeMelange.Aucun:             blendMode = ProjectMercury.BlendMode.None;      break;
                case TypeMelange.Soustraire:        blendMode = ProjectMercury.BlendMode.Subtract;  break;
                default:                            blendMode = ProjectMercury.BlendMode.Add;       break;
            }

            return blendMode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.Renderer.RenderEffect(this.ParticleEffect, ref Scene.Camera.Transformee);
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
