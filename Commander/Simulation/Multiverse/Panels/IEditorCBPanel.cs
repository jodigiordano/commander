namespace EphemereGames.Commander.Simulation
{
    interface IEditorCBPanel
    {
        CelestialBody CelestialBody { get; set; }
        Simulator Simulator         { get; set; }
    }
}
