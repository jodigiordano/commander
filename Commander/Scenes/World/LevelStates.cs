﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LevelStates : IVisual
    {
        public Dictionary<int, CelestialBody> CelestialBodies;
        public Dictionary<string, LevelDescriptor> LevelsDescriptors;
        public WorldDescriptor Descriptor;
        
        private Dictionary<int, bool> LevelUnlockedStates;
        private Dictionary<int, Text> LockedTexts;
        private Text GamePausedText;
        private Dictionary<int, Text> LevelsNumbers;

        private Scene Scene;

        private byte alpha;
        private byte LockedMaxAlpha;
        private byte GamePausedMaxAlpha;
        private byte NumbersMaxAlpha;

        public bool AllLevelsUnlockedOverride;


        public LevelStates(Scene scene)
        {
            Scene = scene;
            LevelUnlockedStates = new Dictionary<int, bool>();
            LockedTexts = new Dictionary<int, Text>();
            LevelsNumbers = new Dictionary<int, Text>();

            LockedMaxAlpha = 100;
            GamePausedMaxAlpha = 200;
            alpha = 255;
            NumbersMaxAlpha = 200;

            AllLevelsUnlockedOverride = false;
        }


        public void Initialize()
        {
            // Level numbers
            foreach (var level in Descriptor.Levels)
            {
                var cb = CelestialBodies[level.Key];

                LevelsNumbers.Add(level.Key, new Text(LevelsDescriptors[cb.Name].Infos.Mission, @"Pixelite")
                {
                    SizeX = 3,
                    VisualPriority = cb.VisualPriority + 0.00001,
                    Alpha = 150
                }.CenterIt());
            }

            // Lock state
            foreach (var level in Descriptor.Levels)
            {
                LevelUnlockedStates.Add(level.Key, false);
                LockedTexts.Add(level.Key, new Text("Locked", @"Pixelite")
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
            foreach (var level in Descriptor.Levels)
            {
                bool unlocked = Descriptor.Id < 2; //tmp Alpha (instead of true)

                if (!AllLevelsUnlockedOverride)
                {
                    foreach (var other in level.Value)
                        if (!Main.SaveGameController.IsLevelUnlocked(other))
                        {
                            unlocked = false;
                            break;
                        }
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

            // Draw level numbers
            foreach (var kvp in LevelsNumbers)
            {
                var cb = CelestialBodies[kvp.Key];

                kvp.Value.Position = cb.Position - new Vector3(0, cb.Circle.Radius + 20, 0);
                kvp.Value.Alpha = (byte) (GetLevelNumberAlpha(kvp.Key));
                Scene.Add(kvp.Value);
            }
        }


        private CelestialBody GetPausedGameCelestialBody()
        {
            return CelestialBodies[Main.GameInProgress.Level.Infos.Id];
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
    }
}
