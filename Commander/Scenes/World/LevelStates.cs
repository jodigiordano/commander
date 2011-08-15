namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LevelStates
    {
        public Dictionary<int, CelestialBody> CelestialBodies;
        public WorldDescriptor Descriptor;
        
        private Dictionary<int, bool> LevelUnlockedStates;
        private Dictionary<int, Text> LockedTexts;
        private Text GamePausedText;

        private Scene Scene;


        public LevelStates(Scene scene)
        {
            Scene = scene;
            LevelUnlockedStates = new Dictionary<int, bool>();
            LockedTexts = new Dictionary<int, Text>();
        }


        public void Initialize()
        {
            foreach (var level in Descriptor.Levels)
            {
                LevelUnlockedStates.Add(level.Key, false);
                LockedTexts.Add(level.Key, new Text("Locked", "Pixelite")
                {
                    SizeX = 2,
                    Alpha = 100,
                    VisualPriority = VisualPriorities.Default.LevelLocked
                }.CenterIt());
            }

            GamePausedText = new Text("Paused", "Pixelite")
            {
                SizeX = 2,
                Color = Colors.Default.GamePaused,
                Alpha = 200,
                VisualPriority = VisualPriorities.Default.LevelPaused
            }.CenterIt();
        }

        
        public byte GetLevelNumberAlpha(int id)
        {
            return (byte) (LevelUnlockedStates[id] ? 200 : 0);
        }


        public void Sync()
        {
            // Unlock conditions
            foreach (var level in Descriptor.Levels)
            {
                bool unlocked = true;

                foreach (var other in level.Value)
                    if (!Main.PlayerSaveGame.Progress.ContainsKey(other) || Main.PlayerSaveGame.Progress[other] <= 0)
                    {
                        unlocked = false;
                        break;
                    }

                LevelUnlockedStates[level.Key] = unlocked;
                CelestialBodies[level.Key].CanSelect = unlocked;
            }
        }


        public void Draw()
        {
            foreach (var kvp in LevelUnlockedStates)
            {
                var cb = CelestialBodies[kvp.Key];
                var lockedImg = LockedTexts[kvp.Key];

                if (kvp.Value) //unlocked
                {
                    cb.Image.Alpha = (byte) 255;
                }

                else
                {
                    cb.Image.Alpha = (byte) 100;
                    lockedImg.Position = cb.Position - new Vector3(0, cb.Circle.Radius + 20, 0);
                    Scene.Add(lockedImg);
                }
            }

            if (Main.GamePausedToWorld)
            {
                var cb = GetPausedGameCelestialBody();
                GamePausedText.Position = cb.Position + new Vector3(0, cb.Circle.Radius + 10, 0);
                Scene.Add(GamePausedText);
            }
        }


        private CelestialBody GetPausedGameCelestialBody()
        {
            return CelestialBodies[Main.GameInProgress.Level.Infos.Id];
        }
    }
}
