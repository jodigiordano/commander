namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class AnimationCommodore : Animation
    {
        private Image Bubble;
        private Image TheLieutenant;
        private TextTypeWriter TypeWriter;
        private EffectsController<IPhysical> GPE;
        private EffectsController<IVisual> GVE;


        public AnimationCommodore(string text, double time, double visualPriority)
            : base(time, visualPriority)
        {
            TheLieutenant = new Image("lieutenant", new Vector3(-300, 500, 0))
            {
                SizeX = 6,
                VisualPriority = Preferences.PrioriteGUIMenuPrincipal
            };

            Bubble = new Image("bulle", new Vector3(-100, 300, 0))
            {
                SizeX = 8,
                VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f,
                Origin = Vector2.Zero
            };

            TypeWriter = new TextTypeWriter
            (
                text,
                Color.Black,
                new Vector3(20, 280, 0),
                @"Pixelite",
                2.0f,
                new Vector2(600, 500),
                50,
                true,
                1000,
                true,
                new List<string>()
                {
                    "sfxLieutenantParle1",
                    "sfxLieutenantParle2",
                    "sfxLieutenantParle3",
                    "sfxLieutenantParle4"
                },
                Scene
            );
            TypeWriter.Text.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            GPE = new EffectsController<IPhysical>();
            GVE = new EffectsController<IVisual>();

            MovePathEffect edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Duration;
            edt.Progress = Effect<IPhysical>.ProgressType.Linear;
            edt.InnerPath = new Path2D
            (new List<Vector2> { new Vector2(-300, 500), new Vector2(-300, 275), new Vector2(-300, 275), new Vector2(-300, 700) },
             new List<double> { 0, 1000, this.Duration - 1000, this.Duration });

            GPE.Add(TheLieutenant, edt);

            edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Duration;
            edt.Progress = Effect<IPhysical>.ProgressType.Linear;
            edt.InnerPath = new Path2D
            (new List<Vector2> { new Vector2(-100, 300), new Vector2(-100, -100), new Vector2(-100, -100), new Vector2(-100, 500) },
             new List<double> { 0, 1000, this.Duration - 1000, this.Duration });

            GPE.Add(Bubble, edt);

            edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Duration;
            edt.Progress = Effect<IPhysical>.ProgressType.Linear;
            edt.InnerPath = new Path2D
            (new List<Vector2> { new Vector2(-75, 325), new Vector2(-75, -75), new Vector2(-75, -75), new Vector2(-75, 500) },
             new List<double> { 0, 1000, this.Duration - 1000, this.Duration });

            GPE.Add(TypeWriter.Text, edt);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TypeWriter.Update();

            GPE.Update((float) gameTime.ElapsedGameTime.TotalMilliseconds);
            GVE.Update((float) gameTime.ElapsedGameTime.TotalMilliseconds);
        }


        public void FadeIn()
        {
            GVE.Add(Bubble, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 250));
            GVE.Add(TheLieutenant, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 250));
            GVE.Add(TypeWriter.Text, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 250));
        }


        public void FadeOut()
        {
            GVE.Add(Bubble, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 250));
            GVE.Add(TheLieutenant, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 250));
            GVE.Add(TypeWriter.Text, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 250));
        }


        public override void Draw()
        {
            Scene.Add(Bubble);
            Scene.Add(TheLieutenant);

            TypeWriter.Draw();
        }
    }
}
