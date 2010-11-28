//=====================================================================
// << Singleton >>
// Gestion des transitions entre les scènes
// 
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class GestionnaireTransitions
    {
        //=============================================================================
        // Événements
        //=============================================================================

        public virtual event EventHandler TransitionDemarree;
        public virtual event EventHandler TransitionTerminee;

        protected virtual void OnTransitionDemarree(EventArgs e)
        {
            if (TransitionDemarree != null)
                TransitionDemarree(this, e);
        }

        protected virtual void OnTransitionTerminee(EventArgs e)
        {
            if (TransitionTerminee != null)
                TransitionTerminee(this, e);
        }


        //=====================================================================
        // Attributs privés
        //=====================================================================

        // Instance du gestionnaire
        private static GestionnaireTransitions instance = new GestionnaireTransitions();

        // Descriptions des transitions
        private Dictionary<String, Transition> descriptionTransitions;

        // Effets appliqués sur les scènes, ce qui créer la transition
        private GestionnaireEffets ge;

        // Animations jouées lors de la transition
        private GestionnaireAnimations ga;

        // Transition en cours
        private Transition transitionEnCours;
        private Dictionary<Scene, DescriptionTransition> scenesEnTransition;
        private double tempsRestant;


        //=====================================================================
        // Initialisation
        //=====================================================================

        private GestionnaireTransitions()
        {
            EnTransition = false;
            descriptionTransitions = new Dictionary<string, Transition>();

            ge = new GestionnaireEffets();
            ga = new GestionnaireAnimations();
        }


        //=====================================================================
        // Services
        //=====================================================================

        /// <summary>
        /// Obtenir l'instance du gestionnaire
        /// </summary>
        public static GestionnaireTransitions Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        /// Transition en cours ?
        /// </summary>
        public bool EnTransition { get; private set; }


        /// <summary>
        /// Ajouter une description de transition dans le gestionnaire
        /// </summary>
        /// <param name="nom">nom de la transition</param>
        /// <param name="transition">description de la transition</param>
        public void ajouter(String nom, Transition transition)
        {
            descriptionTransitions[nom] = transition;
        }


        /// <summary>
        /// Effectuer une transition entre deux ou plusieurs scènes
        /// </summary>
        /// <param name="nomTransition">Nom de la transition</param>
        public void transition(String nomTransition)
        {
            #if DEBUG
            if (EnTransition)
                throw new Exception("Deja en transition.");
            #endif

            // récupérer la description des transitions
            transitionEnCours = descriptionTransitions[nomTransition];

            // noter qu'on est en transition
            EnTransition = true;
            tempsRestant = transitionEnCours.Duree;

            // récupérer les scènes en transition
            // initialiser les effets à appliquer durant la transition
            scenesEnTransition = new Dictionary<Scene, DescriptionTransition>();
            
            ge.vider();
            ga.vider();

            for (int i = 0; i < transitionEnCours.Descriptions.Count; i++)
            {
                Scene scene = GestionnaireScenes.Instance.recuperer(transitionEnCours.Descriptions[i].NomScene);

                ge.ajouter(scene, transitionEnCours.Descriptions[i].Effets);
                ga.inserer(scene, transitionEnCours.Descriptions[i].Animations);

                scenesEnTransition.Add(scene, transitionEnCours.Descriptions[i]);

                if (!transitionEnCours.Descriptions[i].FocusApres)
                    scene.onFocusLost();
                else
                    scene.onTransitionTowardFocus();
            }

            // bloquer la saisie durant les transitions
            //Main.GestionTouches.Desactive = true;
            OnTransitionDemarree(new EventArgs());

            debutTransition();
        }


        /// <summary>
        /// Mettre à jour la transition
        /// </summary>
        /// <param name="gameTime">Temps actuel</param>
        public void Update(GameTime gameTime)
        {
            ge.Update(gameTime);
            ga.Update(gameTime);

            tempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (tempsRestant <= 0)
            {
                EnTransition = false;
                finTransition();
                ge.stop();
                ga.vider();

                //Main.GestionTouches.Desactive = false;
                OnTransitionTerminee(new EventArgs());
            }
        }


        /// <summary>
        /// Débuter la transition
        /// </summary>
        private void debutTransition()
        {
            // Setter les attributs de départ des scènes
            foreach (var kvp in scenesEnTransition)
            {
                kvp.Key.EnPause = kvp.Value.EnPausePendant;
                kvp.Key.EstVisible = true;
                kvp.Key.EnFocus = kvp.Value.FocusApres;
                kvp.Key.PrioriteAffichage = kvp.Value.PrioriteAffichagePendant;
            }

            // Créer les effets
        }


        /// <summary>
        /// Terminer la transition
        /// </summary>
        private void finTransition()
        {
            // Toutes les autres scènes non-décrites dans le XML
            // sont considérées comme arrêtées, donc ni updatées ni affichées
            GestionnaireScenes.Instance.ToutesArretees();

            // Setter les attributs de fin des scènes
            foreach (var kvp in scenesEnTransition)
            {
                kvp.Key.EnPause = kvp.Value.EnPauseApres;
                kvp.Key.EstVisible = kvp.Value.VisibleApres;
                //kvp.Key.Focus = kvp.Value.FocusApres;

                if (kvp.Key.Scene != null)
                    kvp.Key.Scene.PrioriteAffichage = kvp.Value.PrioriteAffichageApres;

                if (kvp.Key.EnFocus)
                    kvp.Key.onFocus();
            }
        }


        public void Draw()
        {
            ga.Draw(null);
        }
    }
}
