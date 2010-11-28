//=====================================================================
//
// Gestionnaire des animations
//
// Le gestionnaire des animations est un objet que peut posséder une
// Scène, par exemple, pour y entreposer ses Animations qui seront
// mises à jour à chaque tick. C'est donc tout simplement un helper
// dont le fonctionnement est le suivant :
//
// L'objet (la Scène par exemple) créer un gestionnaire d'animations
// et peut décider si les animations sont exécutées toutes en même
// temps ou si elles sont chaînées (les animations insérées
// ultérieurement attendent la fin des animations insérées
// précédemment)
//
// todo: si utile, on peut faire en sorte que les deux paradigmes
// s'exécutent en parallèle
//
// Normalement, un objet devrait faire un appel à jouer()
// dans son Update et un appel à Draw(SpriteBatch) dans son Draw.
//
// Il est aussi possible de pauser toutes les animations ou de
// se débarasser de toutes les animations en cours.
//
//=====================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Visuel
{
    public class GestionnaireAnimations
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        private List<Animation> animations = new List<Animation>(); // animations dans l'ordre d'insertion


        //=====================================================================
        // Constructeurs
        //=====================================================================

        public GestionnaireAnimations() { }


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        public Animation Premiere
        {
            get
            {
                if (animations.Count > 0)
                    return animations[0];
                
                else
                    return null;
            }
        }

        public List<Animation> Composants
        {
            get { return animations; }
        }


        //=====================================================================
        // Logique
        //=====================================================================
        
        //
        // Insertion
        //

        public void inserer(Scene scene, Animation animation)
        {
            animation.Scene = scene;
            animation.Initialize();

            animations.Add(animation);
        }


        public void inserer(Scene scene, List<Animation> listeAnimations)
        {
            foreach (var animation in listeAnimations)
                inserer(scene, animation);
        }


        //
        // Mise à jour
        //

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].EnPause) { continue; }

                if (animations[i].estTerminee(gameTime))
                {
                    animations[i].stop();
                    animations.RemoveAt(i);
                    i--;
                    continue;
                }

                animations[i].suivant(gameTime);
            }
        }


        //
        // Dessiner toutes les animations
        //

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < animations.Count; i++)
                animations[i].Draw(spriteBatch);
        }


        //
        // Pauser toutes les animations
        //

        public void pauserTous()
        {
            for (int i = 0; i < animations.Count; i++)
                animations[i].EnPause = true;
        }


        //
        // Vider la liste des animations
        //

        public void vider()
        {
            animations.Clear();
        }
    }
}
