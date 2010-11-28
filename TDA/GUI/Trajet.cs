namespace TDA
{
    using System;
    using System.Collections.Generic;
    using RainingSundays.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Utilities;

    class Trajet : DrawableGameComponent
    {
        public SortedList<int, CorpsCeleste> Chemin;
        private int DistanceDeuxPoints = 40;
        private List<IVisible> Lignes;
        private Scene Partie;
        private Trajet3D TrajetVisuel;

        public Trajet(Partie partie, SortedList<int, CorpsCeleste> chemin)
            : base(partie.Main)
        {
            Partie = partie;
            Chemin = chemin;
            Lignes = new List<IVisible>();
            
            for (int i = 0; i < 200; i++)
            {
                IVisible ligne = new IVisible(partie.Main.Content.Load<Texture2D>("ligne_trajet"), Vector3.Zero, partie);
                ligne.PrioriteAffichage = 0.99f;
                ligne.Origine = ligne.Centre;
                ligne.Couleur = new Color(Color.Red, 100);

                Lignes.Add(ligne);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            int indiceLignes = 0;

            List<Vector3> positions = new List<Vector3>();
            List<double> temps = new List<double>();
            double compteur = 0;

            for (int i = 0; i < Chemin.Count; i++)
                positions.AddRange(Chemin.Values[i].SousPoints);

            temps.Add(compteur);

            for (int i = 1; i < positions.Count; i++)
            {
                compteur += (positions[i] - positions[i - 1]).Length();

                temps.Add(compteur);
            }

            TrajetVisuel = new Trajet3D(positions.ToArray(), temps.ToArray());

            for (double j = 0; j < compteur; j += DistanceDeuxPoints)
            {
                Lignes[indiceLignes].Position = TrajetVisuel.position(j);
                Lignes[indiceLignes].Rotation = TrajetVisuel.rotation(j); //(MathHelper.PiOver2) + (float)Math.Atan2(direction.Y, direction.X);

                Partie.ajouterScenable(Lignes[indiceLignes]);

                indiceLignes++;
            }
        }
    }
}
