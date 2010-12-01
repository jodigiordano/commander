namespace TDA
{
    using System;
    using System.Collections.Generic;
    using RainingSundays.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Utilities;

    class TrajectoireCorpsCelestes : DrawableGameComponent
    {
        private Scene Scene;
        private Dictionary<CorpsCeleste, Trajet3D> CorpsCelestesTrajectoires;
        private Dictionary<CorpsCeleste, ParticuleEffectWrapper> Lignes;
        private double tempsTour = 5000;
        private double compteurTempsTour = 0;
        private int echantillonnageCent = 5;

        public TrajectoireCorpsCelestes(Simulation simulation, List<CorpsCeleste> corpsCelestes)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            CorpsCelestesTrajectoires = new Dictionary<CorpsCeleste, Trajet3D>();

            for (int i = 0; i < corpsCelestes.Count; i++)
            {
                CorpsCeleste corpsCeleste = corpsCelestes[i];

                List<Vector3> positions = new List<Vector3>();
                List<double> temps = new List<double>();

                for (int j = 0; j < 100 / echantillonnageCent + echantillonnageCent; j++)
                {
                    positions.Add(corpsCeleste.positionPourc(j * echantillonnageCent));
                    temps.Add(tempsTour / (100 / echantillonnageCent) * j);
                }

                CorpsCelestesTrajectoires.Add(corpsCeleste, new Trajet3D(positions.ToArray(), temps.ToArray()));

                Lignes = new Dictionary<CorpsCeleste, ParticuleEffectWrapper>();

                foreach (var kvp in CorpsCelestesTrajectoires)
                {
                    Lignes.Add(kvp.Key, Scene.Particules.recuperer("trajetCorpsCeleste"));
                    Lignes[kvp.Key].PrioriteAffichage = Preferences.PrioriteSimulationChemin;
                }

            }
        }


        public override void Update(GameTime gameTime)
        {
            compteurTempsTour += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (compteurTempsTour > tempsTour)
                compteurTempsTour = 0;
        }


        public override void Draw(GameTime gameTime)
        {
            //foreach (var kvp in CorpsCelestesTrajectoires)
            //    Lignes[kvp.Key].Emettre(kvp.Value.position(compteurTempsTour));
        }
    }
}
