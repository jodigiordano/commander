namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CommanderTitle : IVisual
    {
        private Text Commander;
        private Image Filter;
        private Translator PressStart;
        private CommanderScene Scene;

        private List<int> VisualEffectsIds;
        private List<int> PhysicalEffectsIds;

        private byte alpha;


        public CommanderTitle(CommanderScene scene, Vector3 position, double visualPriority)
        {
            Scene = scene;

            alpha = 0;

            Commander = new Text("Commander", @"Pixelite", position)
            {
                SizeX = 12,
                VisualPriority = visualPriority,
                Alpha = 0
            }.CenterIt();

            Filter = new Image("PixelBlanc")
            {
                Color = Color.Transparent,
                VisualPriority = visualPriority + 0.00001,
                Origin = Vector2.Zero
            };

            VisualEffectsIds = new List<int>();
            PhysicalEffectsIds = new List<int>();

            InitPressStart();
        }


        public void Initialize()
        {
            Filter.Size = new Vector2(Scene.CameraView.Width * 1.5f, 200);
        }


        public byte Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;

                PressStart.Alpha = value;
                Commander.Alpha = value;
                Filter.Alpha = Math.Min(value, (byte) 100);
            }
        }


        public void Update()
        {
            if (PressStart.Alpha > 0)
                PressStart.Update();
        }


        public void Draw()
        {
            Scene.Add(Commander);
            Scene.Add(Filter);
            PressStart.Draw();
        }


        public void Show()
        {
            ClearActiveEffects();

            InitPressStart();

            VisualEffectsIds.Add(Scene.VisualEffects.Add(PressStart, EphemereGames.Core.Visual.VisualEffects.Fade(PressStart.Alpha, 255, 500, 1000)));
            VisualEffectsIds.Add(Scene.VisualEffects.Add(Commander, Core.Visual.VisualEffects.Fade(Commander.Alpha, 255, 0, 1000)));
            VisualEffectsIds.Add(Scene.VisualEffects.Add(Filter, Core.Visual.VisualEffects.Fade(Filter.Alpha, 100, 0, 500)));

            MovePathEffect mpe = new MovePathEffect()
            {
                StartAt = 0,
                PointAt = false,
                Delay = 0,
                Length = 10000,
                Progress = Core.Utilities.Effect<IPhysical>.ProgressType.Linear,
                InnerPath = new Path2D(new List<Vector2>()
                        {
                            new Vector2(Scene.CameraView.Left - Filter.AbsoluteSize.X, -85),
                            new Vector2(Scene.CameraView.Left - Filter.AbsoluteSize.X / 5, -85),
                            new Vector2(Scene.CameraView.Left, -85)
                        }, new List<double>()
                        {
                            0,
                            600,
                            1200
                        })
            };

            PhysicalEffectsIds.Add(Scene.PhysicalEffects.Add(Filter, mpe));
        }


        public void Hide()
        {
            ClearActiveEffects();

            VisualEffectsIds.Add(Scene.VisualEffects.Add(PressStart, EphemereGames.Core.Visual.VisualEffects.Fade(PressStart.Alpha, 0, 0, 1000)));
            VisualEffectsIds.Add(Scene.VisualEffects.Add(Commander, Core.Visual.VisualEffects.FadeOutTo0(Commander.Alpha, 0, 1000)));
            VisualEffectsIds.Add(Scene.VisualEffects.Add(Filter, Core.Visual.VisualEffects.FadeOutTo0(Filter.Alpha, 0, 1000)));
        }


        private void InitPressStart()
        {
            PressStart = new Translator(
                Scene, new Vector3(0, 65, 0),
                "Alien", Colors.Default.AlienBright,
                @"Pixelite", Colors.Default.NeutralBright,
                (Preferences.Target == Setting.Xbox360 || Preferences.Target == Setting.ArcadeRoyale) ?
                "Press a button to start your engine" :
                "Click a mouse button to start your engine",
                3, true, 3000, 250, Commander.VisualPriority, false)
                {
                    CenterText = true
                };
        }


        private void ClearActiveEffects()
        {
            Scene.VisualEffects.CancelEffect(VisualEffectsIds);
            Scene.PhysicalEffects.CancelEffect(PhysicalEffectsIds);

            VisualEffectsIds.Clear();
            PhysicalEffectsIds.Clear();
        }


        Rectangle IVisual.VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Color IVisual.Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
