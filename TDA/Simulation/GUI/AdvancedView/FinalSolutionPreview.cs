namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class FinalSolutionPreview
    {
        public CorpsCeleste CelestialBody;
        private IVisible BlowUpZoneVisual;
        private Simulation Simulation;

        public FinalSolutionPreview(Simulation simulation)
        {
            Simulation = simulation;

            BlowUpZoneVisual = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("CercleBlanc"), Vector3.Zero);
            BlowUpZoneVisual.Couleur = Color.Red;
            BlowUpZoneVisual.Couleur.A = 100;
            BlowUpZoneVisual.Origine = BlowUpZoneVisual.Centre;
            BlowUpZoneVisual.VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f;
        }

        public void Draw()
        {
            if (CelestialBody == null)
                return;

            BlowUpZoneVisual.Position = CelestialBody.Position;
            BlowUpZoneVisual.Taille = (CelestialBody.ZoneImpactDestruction.Rayon / 100) * 2;
            Simulation.Scene.ajouterScenable(BlowUpZoneVisual);
        }
    }
}
