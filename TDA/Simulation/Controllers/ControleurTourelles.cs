namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;

    class ControleurTourelles : DrawableGameComponent
    {
        public List<Tourelle> TourellesDepart;
        public ControleurSystemePlanetaire ControleurSystemePlanetaire;
        public delegate void TurretHandler(Tourelle tourelle);
        public event TurretHandler TurretBought;
        public event TurretHandler TurretSold;
        public event TurretHandler TurretUpdated;
        public event TurretHandler TurretReactivated;
        public event PhysicalObjectHandler ObjectCreated;

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

                if (t.RetourDeInactiveCeTick)
                    notifyTurretReactivated(t);

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
                    notifyObjectCreated(projectiles[j]);
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


        private void notifyTourelleAchetee(Tourelle turret)
        {
            if (TurretBought != null)
                TurretBought(turret);
        }


        private void notifyTourelleVendue(Tourelle turret)
        {
            if (TurretSold != null)
                TurretSold(turret);
        }


        private void notifyTourelleMiseAJour(Tourelle turret)
        {
            if (TurretUpdated != null)
                TurretUpdated(turret);
        }


        private void notifyObjectCreated(IObjetPhysique obj)
        {
            if (ObjectCreated != null)
                ObjectCreated(obj);
        }


        private void notifyTurretReactivated(Tourelle turret)
        {
            if (TurretReactivated != null)
                TurretReactivated(turret);
        }
    }
}
