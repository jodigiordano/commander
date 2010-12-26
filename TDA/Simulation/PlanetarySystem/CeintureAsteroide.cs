namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class CeintureAsteroide :CorpsCeleste
    {
        private class RepAsteroide
        {
            public IVisible Representation;
            public float VitesseRotation;
            public int VitesseDeplacement;
            public Vector3 Offset;
            public double TempsOffset;
        }

        private const int NB_REPASTEROIDES = 200;
        private RepAsteroide[] RepAsteroides;

        public CeintureAsteroide(Simulation simulation, String nom, Vector3 positionBase, float rayon, double tempsRotation, List<IVisible> representations, int pourcDepart)
            : base(simulation, nom, positionBase, Vector3.Zero, rayon, tempsRotation, representations[0], pourcDepart, Preferences.PrioriteSimulationCeintureAsteroides, false, 0)
        {
            RepAsteroides = new RepAsteroide[NB_REPASTEROIDES];
 
            for (int i = 0; i < NB_REPASTEROIDES; i++)
            {
                RepAsteroide rep = new RepAsteroide();
                rep.Representation = (IVisible) representations[Main.Random.Next(0, representations.Count)].Clone();
                rep.Representation.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                rep.TempsOffset = Main.Random.Next((int)(-tempsRotation/2), (int)(tempsRotation/2));
                rep.Offset = new Vector3(Main.Random.Next(-50,50),Main.Random.Next(-50,50 ),0);
                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + rep.TempsOffset) % this.TempsRotation, ref this.PositionBase, ref rep.Offset, ref rep.Representation.position);
                rep.Representation.Couleur.A = 60;
                rep.VitesseRotation = Main.Random.Next(-100, 100) / 10000.0f;
                rep.VitesseDeplacement = Main.Random.Next(1, 10);
                rep.Representation.Taille = Main.Random.Next(10, 70) / 30.0f;


                RepAsteroides[i] = rep;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NB_REPASTEROIDES; i++)
            {
                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + RepAsteroides[i].TempsOffset) % TempsRotation, ref this.PositionBase, ref RepAsteroides[i].Offset, ref RepAsteroides[i].Representation.position);
                RepAsteroides[i].Representation.Rotation += RepAsteroides[i].VitesseRotation;

                Simulation.Scene.ajouterScenable(RepAsteroides[i].Representation);
            }

            for (int i = 0; i < Emplacements.Count; i++)
                Emplacements[i].Draw(gameTime);
        }

    }
}
