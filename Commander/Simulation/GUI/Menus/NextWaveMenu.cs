namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class NextWaveMenu
    {
        public Vector3 Position;
        public bool Visible;

        private Dictionary<EnemyType, EnemyDescriptor> EnnemisQtes;
        private Simulation Simulation;
        private Bubble Bulle;
        private List<Image> RepresentationsEnnemis;
        private List<Text> RepresentationsQtes;
        private List<Text> RepresentationsNiveaux;
        private float PrioriteAffichage;
        private const int NB_REPRESENTATIONS = 10;


        public NextWaveMenu(
            Simulation simulation,
            Dictionary<EnemyType, EnemyDescriptor> ennemisQtes,
            Vector3 positionInitiale,
            float prioriteAffichage)
        {
            Simulation = simulation;
            EnnemisQtes = ennemisQtes;
            Position = positionInitiale;
            PrioriteAffichage = prioriteAffichage;

            RepresentationsEnnemis = new List<Image>();
            RepresentationsQtes = new List<Text>();
            RepresentationsNiveaux = new List<Text>();

            for (int i = 0; i < NB_REPRESENTATIONS; i++)
            {
                Image im = new Image("PixelBlanc")
                {
                    VisualPriority = this.PrioriteAffichage + 0.00001f,
                    SizeX = 4
                };

                RepresentationsEnnemis.Add(im);

                Text t = new Text("Pixelite")
                {
                    VisualPriority = this.PrioriteAffichage,
                    SizeX = 2
                };

                RepresentationsQtes.Add(t);

                t = new Text("Pixelite")
                {
                    VisualPriority = this.PrioriteAffichage,
                    SizeX = 1
                };

                RepresentationsNiveaux.Add(t);
            }

            Bulle = new Bubble(Simulation, Rectangle.Empty, this.PrioriteAffichage + 0.0001f);
            Bulle.BlaPosition = 1;

            Visible = false;
        }


        public void Draw()
        {
            //todo: replug correctly!
            //if (!Visible || EnnemisQtes.Count == 0)
            //    return;

            //Bulle.Dimension.X = (int) this.Position.X;
            //Bulle.Dimension.Y = (int) this.Position.Y;

            //Bulle.Dimension.Width = 100;
            //Bulle.Dimension.Height = 10;

            //int indiceRepresentations = 0;

            //foreach (var ennemiQte in EnnemisQtes)
            //{
            //    RepresentationsEnnemis[indiceRepresentations].Texture = EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(EnemiesFactory.GetTexture(ennemiQte.Key));
            //    RepresentationsEnnemis[indiceRepresentations].Origine = RepresentationsEnnemis[indiceRepresentations].Centre;
                              
            //    RepresentationsEnnemis[indiceRepresentations].Position = this.Position + new Vector3
            //        (
            //            25,
            //            Bulle.Dimension.Height + RepresentationsEnnemis[indiceRepresentations].Rectangle.Height / 2.0f,
            //            0
            //        );

            //    Bulle.Dimension.Height += RepresentationsEnnemis[indiceRepresentations].Rectangle.Height + 10;

            //    RepresentationsNiveaux[indiceRepresentations].Texte = (ennemiQte.Value.LivesLevel).ToString();
            //    RepresentationsNiveaux[indiceRepresentations].Origine = RepresentationsNiveaux[indiceRepresentations].Centre;
            //    RepresentationsNiveaux[indiceRepresentations].Position = new Vector3(this.Position.X + 40, RepresentationsEnnemis[indiceRepresentations].Position.Y, 0);

            //    RepresentationsQtes[indiceRepresentations].Texte = ((int) ennemiQte.Value.CashValue).ToString();
            //    RepresentationsQtes[indiceRepresentations].Origine = RepresentationsQtes[indiceRepresentations].Centre;
            //    RepresentationsQtes[indiceRepresentations].Position = new Vector3(this.Position.X + 80, RepresentationsEnnemis[indiceRepresentations].Position.Y, 0);

            //    Simulation.Scene.Add(RepresentationsEnnemis[indiceRepresentations]);
            //    Simulation.Scene.Add(RepresentationsNiveaux[indiceRepresentations]);
            //    Simulation.Scene.Add(RepresentationsQtes[indiceRepresentations]);

            //    if (indiceRepresentations > NB_REPRESENTATIONS)
            //        break;
            //    else
            //        indiceRepresentations++;
            //}

            //Bulle.Draw();
        }
    }
}
