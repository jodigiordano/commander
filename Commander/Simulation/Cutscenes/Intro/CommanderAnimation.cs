namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CommanderAnimation
    {
        private Image Background;
        private Text Commander;
        private Translator SubTitle;
        private Scene Scene;
        private double TimeBeforeIn;
        private double Elapsed;

        private List<KeyValuePair<Text, Vector3>> Letters;


        public CommanderAnimation(Scene scene)
        {
            Scene = scene;

            Background = new Image("PixelBlanc")
            {
                Color = Color.White,
                Alpha = 0,
                Size = new Vector2(1280, 720),
                VisualPriority = VisualPriorities.Default.IntroCommanderBackground,
                Blend = BlendType.Add
            };

            Commander = new Text("Commander", "Pixelite")
            {
                Color = Color.Black,
                SizeX = 12,
                VisualPriority = VisualPriorities.Default.IntroCommanderText
            };

            SubTitle = new Translator(
                scene,
                new Vector3(0, 100, 0),
                "Alien", Colors.Default.NeutralDark,
                "Pixelite", Colors.Default.AlienBright,
                "Todo: Subtitle here",
                5, true, 4000, 250, VisualPriorities.Default.IntroCommanderText, false);
            SubTitle.CenterText = true;

            TimeBeforeIn = IntroCutscene.Timing["CommanderIn"];

            PrepareLetters();

            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn, 2000));
            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 13000, 2000));

            Scene.VisualEffects.Add(SubTitle.Translated, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + 5000, 3000));
            Scene.VisualEffects.Add(SubTitle.Translated, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 3000));

            Scene.VisualEffects.Add(SubTitle.ToTranslate, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + 5000, 3000));
            Scene.VisualEffects.Add(SubTitle.ToTranslate, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 3000));



            foreach (var kvp in Letters)
            {
                kvp.Key.Alpha = 0;

                Scene.PhysicalEffects.Add(kvp.Key, Core.Physics.PhysicalEffects.Move(kvp.Value, TimeBeforeIn + 2000, 10000));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + Main.Random.Next(2000, 3000), 1500));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 1000));
            }

            Elapsed = 0;
        }


        public void Update()
        {
            Elapsed += Preferences.TargetElapsedTimeMs;

            if (Elapsed >= TimeBeforeIn + 4000)
                SubTitle.Update();
        }


        public void Draw()
        {
            Scene.BeginForeground();
            Scene.Add(Background);

            foreach (var l in Letters)
                Scene.Add(l.Key);

            SubTitle.Draw();

            Scene.EndForeground();
        }


        private void PrepareLetters()
        {
            Letters = new List<KeyValuePair<Text,Vector3>>();

            var text = Commander.Data;
            var letterSizeX = Commander.TextSize.X / text.Length;
            var centerLetterIndex = text.Length / 2;

            Text t = null;
            Vector3 endPosition;

            for (int i = 0; i < centerLetterIndex; i++)
            {
                endPosition = Commander.Position - new Vector3((centerLetterIndex - i) * letterSizeX, 0, 0);

                t = Commander.Clone();
                t.Data = text[i].ToString();
                t.Position = endPosition - new Vector3((centerLetterIndex - i) * letterSizeX / 2, 0, 0);
                t.CenterIt();

                Letters.Add(new KeyValuePair<Text,Vector3>(t, endPosition));
            }

            if (text.Length % 2 != 0)
            {
                t = Commander.Clone();
                t.Data = text[text.Length / 2].ToString();
                t.Position = Commander.Position;
                t.CenterIt();

                Letters.Add(new KeyValuePair<Text, Vector3>(t, Commander.Position));
            }

            for (int i = 1; i <= centerLetterIndex; i++)
            {
                endPosition = Commander.Position + new Vector3(i * letterSizeX, 0, 0);

                t = Commander.Clone();
                t.Data = text[centerLetterIndex + i].ToString();
                t.Position = endPosition + new Vector3(i * letterSizeX / 2, 0, 0);
                t.CenterIt();

                Letters.Add(new KeyValuePair<Text,Vector3>(t, endPosition));
            }
        }
    }
}
