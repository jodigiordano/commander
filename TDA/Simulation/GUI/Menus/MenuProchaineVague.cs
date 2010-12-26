namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Utilities;
    using ProjectMercury.Emitters;


    class MenuProchaineVague : DrawableGameComponent
    {
        // Données externes
        public Vector3 Position;


        private Dictionary<EnemyType, EnemyDescriptor> EnnemisQtes;
        private Simulation Simulation;
        private Bulle Bulle;

        private List<IVisible> RepresentationsEnnemis;
        private List<IVisible> RepresentationsQtes;
        private List<IVisible> RepresentationsNiveaux;

        private float PrioriteAffichage;

        private const int NB_REPRESENTATIONS = 10;


        public MenuProchaineVague(
            Simulation simulation,
            Dictionary<EnemyType, EnemyDescriptor> ennemisQtes,
            Vector3 positionInitiale,
            float prioriteAffichage)
            : base(simulation.Main)
        {
            Simulation = simulation;
            EnnemisQtes = ennemisQtes;
            Position = positionInitiale;
            PrioriteAffichage = prioriteAffichage;

            RepresentationsEnnemis = new List<IVisible>();
            RepresentationsQtes = new List<IVisible>();
            RepresentationsNiveaux = new List<IVisible>();

            for (int i = 0; i < NB_REPRESENTATIONS; i++)
            {
                IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
                iv.VisualPriority = this.PrioriteAffichage + 0.00001f;
                RepresentationsEnnemis.Add(iv);

                iv = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
                iv.Taille = 2;
                iv.VisualPriority = this.PrioriteAffichage;
                RepresentationsQtes.Add(iv);

                iv = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
                iv.Taille = 1;
                iv.VisualPriority = this.PrioriteAffichage;
                RepresentationsNiveaux.Add(iv);
            }

            Bulle = new Bulle(Simulation, Rectangle.Empty, this.PrioriteAffichage + 0.0001f);
            Bulle.PositionBla = 1;

            Visible = false;
        }


        public override void Draw(GameTime gameTime)
        {
            if (!Visible || EnnemisQtes.Count == 0)
                return;

            Bulle.Dimension.X = (int) this.Position.X;
            Bulle.Dimension.Y = (int) this.Position.Y;

            Bulle.Dimension.Width = 100;
            Bulle.Dimension.Height = 10;

            int indiceRepresentations = 0;

            foreach (var ennemiQte in EnnemisQtes)
            {
                RepresentationsEnnemis[indiceRepresentations].Texture = EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(FactoryEnnemis.Instance.getTexture(ennemiQte.Key));
                RepresentationsEnnemis[indiceRepresentations].Origine = RepresentationsEnnemis[indiceRepresentations].Centre;
                              
                RepresentationsEnnemis[indiceRepresentations].Position = this.Position + new Vector3
                    (
                        25,
                        Bulle.Dimension.Height + RepresentationsEnnemis[indiceRepresentations].Rectangle.Height / 2.0f,
                        0
                    );

                Bulle.Dimension.Height += RepresentationsEnnemis[indiceRepresentations].Rectangle.Height + 10;

                RepresentationsNiveaux[indiceRepresentations].Texte = (ennemiQte.Value.LivesLevel).ToString();
                RepresentationsNiveaux[indiceRepresentations].Origine = RepresentationsNiveaux[indiceRepresentations].Centre;
                RepresentationsNiveaux[indiceRepresentations].Position = new Vector3(this.Position.X + 40, RepresentationsEnnemis[indiceRepresentations].Position.Y, 0);

                RepresentationsQtes[indiceRepresentations].Texte = ((int) ennemiQte.Value.CashValue).ToString();
                RepresentationsQtes[indiceRepresentations].Origine = RepresentationsQtes[indiceRepresentations].Centre;
                RepresentationsQtes[indiceRepresentations].Position = new Vector3(this.Position.X + 80, RepresentationsEnnemis[indiceRepresentations].Position.Y, 0);

                Simulation.Scene.ajouterScenable(RepresentationsEnnemis[indiceRepresentations]);
                Simulation.Scene.ajouterScenable(RepresentationsNiveaux[indiceRepresentations]);
                Simulation.Scene.ajouterScenable(RepresentationsQtes[indiceRepresentations]);

                if (indiceRepresentations > NB_REPRESENTATIONS)
                    break;
                else
                    indiceRepresentations++;
            }

            Bulle.Draw(null);
        }
    }
}
