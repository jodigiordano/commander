namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class ControleurEnnemis : DrawableGameComponent
    {
        public List<Ennemi> Ennemis                                                     { get; private set; }
        public LinkedList<Wave> Vagues                                                  { get; set; }
        public Dictionary<EnemyType, EnemyDescriptor> CompositionProchaineVague         { get; set; }
        public VaguesInfinies VaguesInfinies                                            { get; set; }
        public Path Chemin                                                            { get; set; }
        public Path CheminProjection                                                  { get; set; }
        public int NbEnnemis                                                            { get { return Ennemis.Count; } }
        public delegate void VagueTermineeHandler();
        public event VagueTermineeHandler VagueTerminee;
        public delegate void VagueDebuteeHandler();
        public event VagueDebuteeHandler VagueDebutee;
        public event PhysicalObjectHandler ObjetDetruit;
        public event PhysicalObjectHandler ObjetCree;
        public delegate void EnnemiAtteintFinTrajetHandler(Ennemi ennemi, CorpsCeleste corpsCeleste);
        public event EnnemiAtteintFinTrajetHandler EnnemiAtteintFinTrajet;
        private Simulation Simulation;
        private LinkedListNode<Wave> ProchaineVague                    { get; set; }
        private List<Wave> VaguesActives                               { get; set; }
        private double CompteurVague                                    { get; set; }

        public Vector3 PourcentageMinerauxDonnes;
        public int ValeurTotalMineraux;
        public int NbPackViesDonnes;
        public List<Mineral> Mineraux;
        private List<KeyValuePair<int, MineralType>> DistributionMineraux;

        private int NbEnnemisCrees;

        public ControleurEnnemis(Simulation simulation)
            : base(simulation.Main)
        {
            Simulation = simulation;

            Ennemis = new List<Ennemi>();
            Vagues = new LinkedList<Wave>();
            VaguesActives = new List<Wave>();
            Mineraux = new List<Mineral>();
            DistributionMineraux = new List<KeyValuePair<int, MineralType>>();
            CompositionProchaineVague = new Dictionary<EnemyType, EnemyDescriptor>();
        }

        public override void Initialize()
        {
            CompteurVague = 0;
            NbEnnemisCrees = 0;

            ProchaineVague = Vagues.First;

            RecalculateCompositionNextWave();

            if (VaguesInfinies != null)
                return;


            // Générer la distribution des minéraux

            DistributionMineraux.Clear();

            int nbEnnemis = 0;

            foreach (var vague in Vagues)
                nbEnnemis += vague.NbEnemies;

            Vector3 valeurUnitaire = new Vector3(
                Simulation.MineralsFactory.GetValue(MineralType.Cash10),
                Simulation.MineralsFactory.GetValue(MineralType.Cash25),
                Simulation.MineralsFactory.GetValue(MineralType.Cash150));
            Vector3 valeurParType = PourcentageMinerauxDonnes * ValeurTotalMineraux; //atention: float
            Vector3 qteParType = valeurParType / valeurUnitaire;


            for (int i = 0; i < qteParType.X; i++)
                DistributionMineraux.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, nbEnnemis), MineralType.Cash10));

            for (int i = 0; i < qteParType.Y; i++)
                DistributionMineraux.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, nbEnnemis), MineralType.Cash25));

            for (int i = 0; i < qteParType.Z; i++)
                DistributionMineraux.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, nbEnnemis), MineralType.Cash150));

            for (int i = 0; i < NbPackViesDonnes; i++)
                DistributionMineraux.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, nbEnnemis), MineralType.Life1));

            DistributionMineraux.Sort(delegate(KeyValuePair<int, MineralType> m1, KeyValuePair<int, MineralType> m2)
            {
                return m1.Key.CompareTo(m2.Key);
            });

            DistributionMineraux.Reverse();
        }


        protected virtual void notifyVagueTerminee()
        {
            if (VagueTerminee != null)
                VagueTerminee();
        }


        protected virtual void notifyVagueDebutee()
        {
            if (VagueDebutee != null)
                VagueDebutee();
        }


        protected virtual void notifyObjetDetruit(IObjetPhysique objet)
        {
            if (ObjetDetruit != null)
                ObjetDetruit(objet);
        }

        protected virtual void notifyObjetCree(IObjetPhysique objet)
        {
            if (ObjetCree != null)
                ObjetCree(objet);
        }


        protected virtual void notifyEnnemiAtteintFinTrajet(Ennemi ennemi, CorpsCeleste corpsCeleste)
        {
            if (EnnemiAtteintFinTrajet != null)
                EnnemiAtteintFinTrajet(ennemi, corpsCeleste);
        }


        public override void Update(GameTime gameTime)
        {
            // Pour les ennemis qui meurent par eux-memes
            for (int i = Ennemis.Count - 1; i > -1; i--)
                if (!Ennemis[i].EstVivant)
                    Ennemis.RemoveAt(i);

            for (int i = 0; i < Ennemis.Count; i++)
                Ennemis[i].Update(gameTime);

            for (int i = Mineraux.Count - 1; i > -1; i--)
            {
                Mineraux[i].Update(gameTime);

                if (!Mineraux[i].EstVivant)
                    Mineraux.RemoveAt(i);

            }

            gererVagues(gameTime);
        }


        private void gererVagues(GameTime gameTime)
        {

            CompteurVague += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (ProchaineVague != null && ProchaineVague.Value.StartingTime <= CompteurVague)
            {
                CompteurVague = 0;

                VaguesActives.Add(ProchaineVague.Value);

                if (VaguesInfinies == null)
                    ProchaineVague = ProchaineVague.Next;
                else
                    ProchaineVague.Value = VaguesInfinies.getProchaineVague();

                RecalculateCompositionNextWave();

                notifyVagueDebutee();
            }

            for (int i = VaguesActives.Count - 1; i > -1; i--)
            {
                Vector3 positionDepart = Chemin.PremierRelais.Position;
                Vector3 offset = new Vector3((positionDepart.X >= 0) ? 100 : -100, (positionDepart.Y >= 0) ? 100 : -100, 0);

                VaguesActives[i].StartingPosition = positionDepart + offset;
                VaguesActives[i].Update(gameTime);

                List<Ennemi> listeEnnemis = VaguesActives[i].Enemies;

                Ennemis.AddRange(listeEnnemis);

                for (int j = 0; j < listeEnnemis.Count; j++)
                {
                    Ennemi ennemi = listeEnnemis[j];

                    ennemi.Path = this.Chemin;
                    ennemi.CheminProjection = this.CheminProjection;
                    ennemi.RelaisAtteint += new Ennemi.RelaisAtteintHandler(this.listenRelaisAtteint);
                    ennemi.Translation.Y = Main.Random.Next(-20, 20);
                    ennemi.Position = this.Chemin.PremierRelais.Position + ennemi.Translation;
                    ennemi.Initialize();

                    while (DistributionMineraux.Count > 0 && DistributionMineraux[DistributionMineraux.Count - 1].Key == NbEnnemisCrees)
                    {
                        ennemi.Mineraux.Add(Simulation.MineralsFactory.CreateMineral(DistributionMineraux[DistributionMineraux.Count - 1].Value, ennemi.RepresentationVivant.VisualPriority + 0.01f));

                        DistributionMineraux.RemoveAt(DistributionMineraux.Count - 1);
                    }

                    Simulation.Scene.Effets.Add(ennemi.RepresentationVivant, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 1000));

                    NbEnnemisCrees++;

                    notifyObjetCree(ennemi);
                }

                if (VaguesActives[i].IsFinished)
                {
                    VaguesActives.RemoveAt(i);
                    notifyVagueTerminee();
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Ennemis.Count; i++)
                Ennemis[i].Draw(null);
        }


        public void doObjetTouche(IObjetPhysique objet, IObjetPhysique par)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi != null && ennemi.EstVivant)
            {
                ennemi.doTouche((IObjetVivant)par);

                if (!ennemi.EstVivant)
                {
                    ennemi.doMeurt();

                    foreach (var vague in VaguesActives)
                        vague.doEnemyDestroyed(ennemi);

                    List<Mineral> mineraux = ennemi.Mineraux;

                    for (int i = 0; i < mineraux.Count; i++)
                    {
                        Mineral mineral = mineraux[i];

                        mineral.Position = ennemi.Position;

                        Vector3 direction = ennemi.Position - ennemi.PositionDernierProjectileTouche;
                        direction.Normalize();
                        mineral.Direction = direction;
                    }

                    Mineraux.AddRange(mineraux);

                    //Ennemis.Remove(ennemi); //sera fait au prochain tick

                    notifyObjetDetruit(ennemi);
                }

                return;
            }



            Mineral min = objet as Mineral;

            if (min != null)
            {
                min.doMeurt();

                Mineraux.Remove(min);

                notifyObjetDetruit(min);

                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");

                return;
            }
        }


        public void doProchaineVagueDemandee()
        {
            if (ProchaineVague != null)
            {
                CompteurVague = 0;

                VaguesActives.Add(ProchaineVague.Value);
                ProchaineVague = (VaguesInfinies == null) ? ProchaineVague.Next : new LinkedListNode<Wave>(VaguesInfinies.getProchaineVague());

                RecalculateCompositionNextWave();

                notifyVagueDebutee();
            }
        }


        public void listenRelaisAtteint(Ennemi ennemi)
        {
            ennemi.doMeurt();

            foreach (var vague in VaguesActives)
                vague.doEnemyDestroyed(ennemi);

            notifyEnnemiAtteintFinTrajet(ennemi, Chemin.DernierRelais);
        }


        private void RecalculateCompositionNextWave()
        {
            CompositionProchaineVague.Clear();

            if (ProchaineVague != null)
                foreach (var kvp in ProchaineVague.Value.Composition)
                    CompositionProchaineVague.Add(kvp.Key, kvp.Value);
        }
    }
}
