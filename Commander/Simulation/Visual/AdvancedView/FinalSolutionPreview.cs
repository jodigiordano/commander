namespace EphemereGames.Commander.Simulation.Player
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class FinalSolutionPreview
    {
        private Image BlowUpZoneVisual;
        private Simulator Simulator;
        private SimPlayer Owner;


        public FinalSolutionPreview(Simulator simulator, SimPlayer owner)
        {
            Simulator = simulator;
            Owner = owner;

            BlowUpZoneVisual = new Image("CercleBlanc")
            {
                Color = new Color(255, 0, 0, 100),
                VisualPriority = VisualPriorities.Default.PowerUpFinalSolution
            };
        }


        public void Draw()
        {
            if (Owner.ActualSelection.CelestialBody == null ||
                !Simulator.Data.Level.AvailablePowerUps.ContainsKey(PowerUpType.FinalSolution))
                return;

            BlowUpZoneVisual.Position = Owner.ActualSelection.CelestialBody.Position;
            BlowUpZoneVisual.SizeX = (((PowerUpLastSolution) Simulator.Data.Level.AvailablePowerUps[PowerUpType.FinalSolution]).ZoneImpactDestruction / 100) * 2;
            Simulator.Scene.Add(BlowUpZoneVisual);
        }
    }
}
