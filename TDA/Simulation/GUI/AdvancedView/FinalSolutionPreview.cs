namespace TDA
{
    using Core.Visuel;
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

            BlowUpZoneVisual = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("CercleBlanc"), Vector3.Zero, Simulation.Scene);
            BlowUpZoneVisual.Couleur = new Color(Color.Red, 100);
            BlowUpZoneVisual.Origine = BlowUpZoneVisual.Centre;
            BlowUpZoneVisual.PrioriteAffichage = Preferences.PrioriteGUIEtoiles - 0.002f;
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
