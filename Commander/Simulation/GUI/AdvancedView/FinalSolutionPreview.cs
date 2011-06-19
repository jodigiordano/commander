namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class FinalSolutionPreview
    {
        public CelestialBody CelestialBody;
        private Image BlowUpZoneVisual;
        private Simulator Simulator;


        public FinalSolutionPreview(Simulator simulator)
        {
            Simulator = simulator;

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
            BlowUpZoneVisual.SizeX = (((PowerUpLastSolution) Simulator.PowerUpsFactory.Availables[PowerUpType.FinalSolution]).ZoneImpactDestruction / 100) * 2;
            Simulator.Scene.Add(BlowUpZoneVisual);
        }
    }
}
