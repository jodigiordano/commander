namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using ProjectMercury.Emitters;


    class MenuProchaineVague : DrawableGameComponent
    {
        // Données externes
        public Vector3 Position;


        private Dictionary<TypeEnnemi, DescripteurEnnemi> EnnemisQtes;
        private Simulation Simulation;
        private Bulle Bulle;

        private List<IVisible> RepresentationsEnnemis;
        private List<IVisible> RepresentationsQtes;
        private List<IVisible> RepresentationsNiveaux;

        private float PrioriteAffichage;

        private const int NB_REPRESENTATIONS = 10;


        public MenuProchaineVague(
            Simulation simulation,
            Dictionary<TypeEnnemi, DescripteurEnnemi> ennemisQtes,
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
                IVisible iv = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero, Simulation.Scene);
                iv.PrioriteAffichage = this.PrioriteAffichage + 0.00001f;
                RepresentationsEnnemis.Add(iv);

                iv = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
                iv.Taille = 2;
                iv.PrioriteAffichage = this.PrioriteAffichage;
                RepresentationsQtes.Add(iv);

                iv = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Vector3.Zero, Simulation.Scene);
                iv.Taille = 1;
                iv.PrioriteAffichage = this.PrioriteAffichage;
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
                RepresentationsEnnemis[indiceRepresentations].Texture = Core.Persistance.Facade.recuperer<Texture2D>(FactoryEnnemis.Instance.getTexture(ennemiQte.Key));
                RepresentationsEnnemis[indiceRepresentations].Origine = RepresentationsEnnemis[indiceRepresentations].Centre;
                              
                RepresentationsEnnemis[indiceRepresentations].Position = this.Position + new Vector3
                    (
                        25,
                        Bulle.Dimension.Height + RepresentationsEnnemis[indiceRepresentations].Rectangle.Height / 2.0f,
                        0
                    );

                Bulle.Dimension.Height += RepresentationsEnnemis[indiceRepresentations].Rectangle.Height + 10;

                RepresentationsNiveaux[indiceRepresentations].Texte = (ennemiQte.Value.NiveauPointsVie + ennemiQte.Value.NiveauVitesse).ToString();
                RepresentationsNiveaux[indiceRepresentations].Origine = RepresentationsNiveaux[indiceRepresentations].Centre;
                RepresentationsNiveaux[indiceRepresentations].Position = new Vector3(this.Position.X + 40, RepresentationsEnnemis[indiceRepresentations].Position.Y, 0);

                RepresentationsQtes[indiceRepresentations].Texte = ((int) ennemiQte.Value.Valeur).ToString();
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
