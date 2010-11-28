namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using Core.Visuel;
    using Core.Utilities;

    class ControleurTourelles : DrawableGameComponent
    {
        public List<Tourelle> TourellesDepart;
        public ControleurSystemePlanetaire ControleurSystemePlanetaire;
        public delegate void TourelleAcheteeHandler(Tourelle tourelle);
        public event TourelleAcheteeHandler TourelleAchetee;
        public delegate void TourelleVendueHandler(Tourelle tourelle);
        public event TourelleVendueHandler TourelleVendue;
        public delegate void TourelleMiseAJourHandler(Tourelle tourelle);
        public event TourelleMiseAJourHandler TourelleMiseAJour;
        public event ObjetCreeHandler ObjetCree;

        public List<Tourelle> Tourelles { get; private set; }

        private Simulation Simulation;
        private Dictionary<Tourelle, Ennemi> AssociationsAFaireCeTick;


        public ControleurTourelles(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            Tourelles = new List<Tourelle>();
            AssociationsAFaireCeTick = new Dictionary<Tourelle, Ennemi>();
        }


        public override void Initialize()
        {
            for (int i = 0; i < TourellesDepart.Count; i++)
                Tourelles.Add(TourellesDepart[i]);
        }


        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Tourelles.Count; i++)
            {
                Tourelle t = Tourelles[i];

                t.Update(gameTime);

                Ennemi ennemiAttaque;

                if (t.TempsDernierProjectileLance <= 0 && AssociationsAFaireCeTick.TryGetValue(t, out ennemiAttaque))
                {
                    t.EnnemiAttaque = ennemiAttaque;
                    t.Spectateur = false;
                }
                else if (t.EnnemiAttaque != null && !t.EnnemiAttaque.EstVivant)
                {
                    t.EnnemiAttaque = null;
                    t.Spectateur = true;
                }

                else
                    t.Spectateur = true;
            }

            for (int i = 0; i < Tourelles.Count; i++)
            {
                List<Projectile> projectiles = Tourelles[i].ProjectilesCeTick(gameTime);

                for (int j = 0; j < projectiles.Count; j++)
                    notifyObjetCree(projectiles[j]);
            }

            // Pour l'instant, à chaque tick, les tourelles sont dissociées de leurs ennemis
            // et le système de collision fait en sorte que les ennemis sont réassocié de nouveau
            // Une tourelle est associé avec le premier ennemi qui entre en collision avec sa zone d'activation
            // Une meilleure implémentation serait un événement EntreDansZoneActivation et SortDeZoneActivation
            // lancé par le ControleurCollisions et capturé par ce controleur, qui ensuite associerait/dissocierait
            // l'ennemi d'une tourelle. Résultat: moins d'événements lancés à chaque tick.
            AssociationsAFaireCeTick.Clear();
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Tourelles.Count; i++)
                if (Tourelles[i].Visible)
                    Tourelles[i].Draw(null);
        }


        public void doAcheterTourelle(TypeTourelle type, CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            Tourelle tourelle = FactoryTourelles.creerTourelle(Simulation, type);
          
            bool estAjoute = ControleurSystemePlanetaire.ajouterTourelle(tourelle, corpsCeleste, emplacement);

            if (estAjoute)
            {
                Tourelles.Add(tourelle);

                notifyTourelleAchetee(tourelle);
            }
        }


        public void doVendreTourelle(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            //récupérer la tourelle
            Tourelle tourelle = emplacement.Tourelle;

            //enlever la tourelle
            bool estEnleve = tourelle.PeutVendre && ControleurSystemePlanetaire.enleverTourelle(corpsCeleste, emplacement);

            if (estEnleve)
            {
                tourelle.doMeurt();
                Tourelles.Remove(tourelle);
                notifyTourelleVendue(tourelle);
            }
        }


        public void doMettreAJourTourelle(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            //récupérer la tourelle
            Tourelle tourelle = emplacement.Tourelle;

            if (tourelle.miseAJour())
                notifyTourelleMiseAJour(tourelle);
        }


        public void doDansZoneActivationTourelle(Tourelle tourelle, IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi == null)
                return;

            if (!AssociationsAFaireCeTick.ContainsKey(tourelle))
                AssociationsAFaireCeTick.Add(tourelle, null);

            AssociationsAFaireCeTick[tourelle] = ennemi;
        }


        public void doObjetDetruit(IObjetPhysique objet)
        {
            CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            if (corpsCeleste == null)
                return;

            for (int i = 0; i < corpsCeleste.Emplacements.Count; i++)
                if (corpsCeleste.Emplacements[i].EstOccupe)
                    Tourelles.Remove(corpsCeleste.Emplacements[i].Tourelle);
        }


        protected virtual void notifyTourelleAchetee(Tourelle tourelle)
        {
            if (TourelleAchetee != null)
                TourelleAchetee(tourelle);
        }


        protected virtual void notifyTourelleVendue(Tourelle tourelle)
        {
            if (TourelleVendue != null)
                TourelleVendue(tourelle);
        }


        protected virtual void notifyTourelleMiseAJour(Tourelle tourelle)
        {
            if (TourelleMiseAJour != null)
                TourelleMiseAJour(tourelle);
        }


        protected virtual void notifyObjetCree(IObjetPhysique objet)
        {
            if (ObjetCree != null)
                ObjetCree(objet);
        }
    }
}
