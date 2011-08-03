namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Choices
    {
        private Dictionary<Text, CelestialBody> Texts;
        private Simulator Simulator;

        private List<int> VisualEffectsIds;


        public Choices(Simulator simulator, double visualPriority)
        {
            Simulator = simulator;

            Texts = new Dictionary<Text, CelestialBody>();

            foreach (var c in Simulator.PlanetarySystemController.CelestialBodies)
            {
                if (c is AsteroidBelt)
                    continue;

                var text = new Text(c.Name, "Pixelite")
                {
                    SizeX = 3,
                    VisualPriority = visualPriority,
                    Alpha = 0
                }.CenterIt();

                Texts.Add(text, c);
            }

            VisualEffectsIds = new List<int>();
        }


        public void Draw()
        {
            foreach (var kvp in Texts)
            {
                kvp.Key.Position = kvp.Value.Position + new Vector3(0, kvp.Value.Circle.Radius + 10, 0);
                Simulator.Scene.Add(kvp.Key);
            }


            foreach (var player in Inputs.ConnectedPlayers)
            {
                CelestialBody c = Simulator.GetSelectedCelestialBody(player);

                if (c != null)
                {
                    foreach (var kvp in Texts)
                        if (kvp.Key.Data == c.Name)
                        {
                            Simulator.Scene.Add(kvp.Key);
                            break;
                        }
                }
            }
        }


        public void Show()
        {
            ClearActiveEffects();

            foreach (var kvp in Texts)
                VisualEffectsIds.Add(Simulator.Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.Fade(kvp.Key.Alpha, 100, 500 + Main.Random.Next(0, 500), 500), EffectTerminated));
        }


        public void Hide()
        {
            ClearActiveEffects();

            foreach (var kvp in Texts)
                VisualEffectsIds.Add(Simulator.Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.Fade(kvp.Key.Alpha, 0, 0, 500), EffectTerminated));
        }


        private void ClearActiveEffects()
        {
            Simulator.Scene.VisualEffects.CancelEffect(VisualEffectsIds);

            VisualEffectsIds.Clear();
        }


        private void EffectTerminated(int id)
        {
            VisualEffectsIds.Remove(id);
        }
    }
}
