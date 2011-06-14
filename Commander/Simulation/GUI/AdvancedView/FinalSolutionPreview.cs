namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class FinalSolutionPreview
    {
        public CelestialBody CelestialBody;
        private Image BlowUpZoneVisual;
        private Simulation Simulation;


        public FinalSolutionPreview(Simulation simulation)
        {
            Simulation = simulation;

            BlowUpZoneVisual = new Image("CercleBlanc")
            {
                Color = new Color(255, 0, 0, 100),
                VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f
            };
        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            BlowUpZoneVisual.Position = CelestialBody.Position;
            BlowUpZoneVisual.SizeX = (((PowerUpLastSolution) Simulation.PowerUpsFactory.Availables[PowerUpType.FinalSolution]).ZoneImpactDestruction / 100) * 2;
            Simulation.Scene.Add(BlowUpZoneVisual);
        }
    }
}
