namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class Planet : CelestialBody
    {
        public Planet(
            Simulator simulator, string name, Vector3 path, Vector3 basePosition, float rotationRad, Size size,
            float speed, string partialImageName, int startingPourc, double visualPriority, bool hasMoons)
            : base(simulator, name, path, basePosition, rotationRad, size, speed, partialImageName, startingPourc,
                   visualPriority, hasMoons)
        {

        }


        public Planet(Simulator simulator, CelestialBodyDescriptor celestialBodyDescriptor, double visualPriority)
            : this(simulator, celestialBodyDescriptor.Name, celestialBodyDescriptor.Path, celestialBodyDescriptor.Position,
            celestialBodyDescriptor.Rotation, celestialBodyDescriptor.Size, celestialBodyDescriptor.Speed,
            celestialBodyDescriptor.Image, celestialBodyDescriptor.StartingPosition, visualPriority,
            celestialBodyDescriptor.HasMoons) { }
    }
}
