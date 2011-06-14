namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class AnimationCommodore : Animation
    {
        private Image Bubble;
        private Image TheLieutenant;
        private TextTypeWriter TypeWriter;
        private EffectsController GE;


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
                "Pixelite",
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

            GE = new EffectsController();

            MovePathEffect edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Path2D
            (new List<Vector2> { new Vector2(-300, 500), new Vector2(-300, 275), new Vector2(-300, 275), new Vector2(-300, 700) },
             new List<double> { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(TheLieutenant, edt);

            edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Path2D
            (new List<Vector2> { new Vector2(-100, 300), new Vector2(-100, -100), new Vector2(-100, -100), new Vector2(-100, 500) },
             new List<double> { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(Bubble, edt);

            edt = new MovePathEffect();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Path2D
            (new List<Vector2> { new Vector2(-75, 325), new Vector2(-75, -75), new Vector2(-75, -75), new Vector2(-75, 500) },
             new List<double> { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(TypeWriter.Text, edt);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TypeWriter.Update(gameTime);

            GE.Update(gameTime);
        }


        public void FadeIn()
        {
            GE.Add(Bubble, Core.Visual.PredefinedEffects.FadeInFrom0(255, 0, 250));
            GE.Add(TheLieutenant, Core.Visual.PredefinedEffects.FadeInFrom0(255, 0, 250));
            GE.Add(TypeWriter.Text, Core.Visual.PredefinedEffects.FadeInFrom0(255, 0, 250));
        }


        public void FadeOut()
        {
            GE.Add(Bubble, Core.Visual.PredefinedEffects.FadeOutTo0(255, 0, 250));
            GE.Add(TheLieutenant, Core.Visual.PredefinedEffects.FadeOutTo0(255, 0, 250));
            GE.Add(TypeWriter.Text, Core.Visual.PredefinedEffects.FadeOutTo0(255, 0, 250));
        }


        //protected override void Show()
        //{
        //    Scene.Add(Bubble);
        //    Scene.Add(TheLieutenant);

        //    TypeWriter.Show();
        //}


        //protected override void Hide()
        //{
        //    Scene.Remove(Bubble);
        //    Scene.Remove(TheLieutenant);

        //    TypeWriter.Hide();
        //}


        public override void Draw()
        {
            Scene.Add(Bubble);
            Scene.Add(TheLieutenant);

            TypeWriter.Draw();
        }
    }
}
