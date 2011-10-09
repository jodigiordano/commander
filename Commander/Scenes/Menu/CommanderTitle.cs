namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CommanderTitle
    {
        private Text Commander;
        private Image Filter;
        private Translator PressStart;
        private Scene Scene;

        private List<int> VisualEffectsIds;
        private List<int> PhysicalEffectsIds;


        public CommanderTitle(Scene scene, Vector3 position, double visualPriority)
        {
            Scene = scene;

            Commander = new Text("Commander", @"Pixelite", position)
            {
                SizeX = 12,
                VisualPriority = visualPriority,
                Alpha = 0
            }.CenterIt();

            Filter = new Image("PixelBlanc")
            {
                Size = new Vector2(1800, 200),
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
            //
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
                            new Vector2(-1920, -85),
                            new Vector2(-1000, -85),
                            new Vector2(-740, -85)
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
                (Preferences.Target == Core.Utilities.Setting.Xbox360) ? "Press a button to start your engine" : "Click a mouse button to start your engine",
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
    }
}
