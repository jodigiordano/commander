namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LevelStates : IVisual
    {
        private Dictionary<int, bool> LevelUnlockedStates;
        private Dictionary<int, Text> LockedTexts;
        private Text GamePausedText;
        private Dictionary<int, Text> LevelsNumbers;

        private WorldScene Scene;

        private byte alpha;
        private byte LockedMaxAlpha;
        private byte GamePausedMaxAlpha;
        private byte NumbersMaxAlpha;

        public bool AllLevelsUnlockedOverride;

        private int VisualEffectsId;


        public LevelStates(WorldScene scene)
        {
            Scene = scene;
            LevelUnlockedStates = new Dictionary<int, bool>();
            LockedTexts = new Dictionary<int, Text>();
            LevelsNumbers = new Dictionary<int, Text>();

            LockedMaxAlpha = 100;
            GamePausedMaxAlpha = 200;
            alpha = (byte) (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 0 : 255);
            NumbersMaxAlpha = 200;

            AllLevelsUnlockedOverride = false;
        }


        public void Initialize()
        {
            LevelUnlockedStates.Clear();
            LockedTexts.Clear();
            LevelsNumbers.Clear();

            // Level numbers
            foreach (var level in Scene.World.Descriptor.Levels)
            {
                var cb = Scene.LeveltoCB[level];

                LevelsNumbers.Add(level, new Text(Scene.World.GetLevelStringId(level), @"Pixelite")
                {
                    SizeX = 3,
                    VisualPriority = cb.VisualPriority + 0.00001,
                    Alpha = 150
                }.CenterIt());
            }

            // Lock state
            foreach (var level in Scene.World.Descriptor.Levels)
            {
                LevelUnlockedStates.Add(level, false);
                LockedTexts.Add(level, new Text("Locked", @"Pixelite")
                {
                    SizeX = 2,
                    Alpha = LockedMaxAlpha,
                    VisualPriority = VisualPriorities.Default.LevelLocked
                }.CenterIt());
            }

            GamePausedText = new Text("Paused", @"Pixelite")
            {
                SizeX = 2,
                Color = Colors.Default.GamePaused,
                Alpha = GamePausedMaxAlpha,
                VisualPriority = VisualPriorities.Default.LevelPaused
            }.CenterIt();
        }


        public byte Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;

                foreach (var txt in LockedTexts.Values)
                    txt.Alpha = Math.Min(value, LockedMaxAlpha);

                GamePausedText.Alpha = Math.Min(value, LockedMaxAlpha);
            }
        }


        public void Sync()
        {
            for (int i = Scene.World.Descriptor.Levels.Count - 1; i > -1; i--)
            {
                var level = Scene.World.Descriptor.Levels[i];

                bool unlocked = true;

                if (i != 0 && !AllLevelsUnlockedOverride)
                    unlocked = Scene.World.IsLevelUnlocked(Scene.World.Descriptor.Levels[i - 1]);

                LevelUnlockedStates[level] = unlocked;
                Scene.LeveltoCB[level].CanSelect = unlocked;
            }
        }


        public void Draw()
        {
            foreach (var kvp in LevelUnlockedStates)
            {
                var cb = Scene.LeveltoCB[kvp.Key];
                var lockedImg = LockedTexts[kvp.Key];

                if (kvp.Value) //unlocked
                {
                    cb.Alpha = (byte) 255;
                }

                else
                {
                    cb.Alpha = (byte) 100;
                    lockedImg.Position = cb.Position - new Vector3(0, cb.Circle.Radius + 20, 0);
                    Scene.Add(lockedImg);
                }
            }

            if (Scene.GamePausedToWorld)
            {
                var cb = GetPausedGameCelestialBody();
                GamePausedText.Position = cb.Position + new Vector3(0, cb.Circle.Radius + 10, 0);
                Scene.Add(GamePausedText);
            }

            // Draw level numbers
            foreach (var kvp in LevelsNumbers)
            {
                var cb = Scene.LeveltoCB[kvp.Key];

                kvp.Value.Position = cb.Position - new Vector3(0, cb.Circle.Radius + 20, 0);
                kvp.Value.Alpha = (byte) (GetLevelNumberAlpha(kvp.Key));
                Scene.Add(kvp.Value);
            }
        }


        private CelestialBody GetPausedGameCelestialBody()
        {
            return Scene.LeveltoCB[Main.CurrentGame.Level.Infos.Id];
        }


        private byte GetLevelNumberAlpha(int id)
        {
            return (byte) Math.Min(alpha, (LevelUnlockedStates[id] ? NumbersMaxAlpha : 0));
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public void Show()
        {
            ClearActiveEffect();

            VisualEffectsId = Scene.VisualEffects.Add(this, Core.Visual.VisualEffects.Fade(Alpha, 255, 500, 500));
        }


        public void Hide()
        {
            ClearActiveEffect();

            VisualEffectsId = Scene.VisualEffects.Add(this, Core.Visual.VisualEffects.Fade(Alpha, 0, 500, 500));
        }


        private void ClearActiveEffect()
        {
            Scene.VisualEffects.CancelEffect(VisualEffectsId);
        }
    }
}
