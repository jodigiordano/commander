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
                Size = Preferences.BackBuffer,
                VisualPriority = VisualPriorities.Cutscenes.IntroCommanderBackground,
                Blend = BlendType.Add
            };

            Commander = new Text("Commander", @"Pixelite")
            {
                Color = Color.White,
                SizeX = 16,
                VisualPriority = VisualPriorities.Cutscenes.IntroCommanderText
            };

            SubTitle = new Translator(
                scene,
                new Vector3(0, 100, 0),
                "Alien", Colors.Default.NeutralDark,
                @"Pixelite", Colors.Default.AlienBright,
                "Todo: Subtitle here",
                5, true, 4000, 250, VisualPriorities.Cutscenes.IntroCommanderText, false);
            SubTitle.CenterText = true;

            TimeBeforeIn = IntroCutscene.Timing["CommanderIn"];

            PrepareLetters();

            //Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn, 2000));
            //Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 2000));

            foreach (var kvp in Letters)
            {
                kvp.Key.Alpha = 0;

                Scene.PhysicalEffects.Add(kvp.Key, Core.Physics.PhysicalEffects.Move(kvp.Value, TimeBeforeIn + 2000, 10000));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + Main.Random.Next(2000, 3000), 1500));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 1000));
            }

            Scene.VisualEffects.Add(SubTitle, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + 5000, 3000));
            Scene.VisualEffects.Add(SubTitle, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 1000));

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
            //Scene.BeginForeground();
            //Scene.Add(Background);

            foreach (var l in Letters)
                Scene.Add(l.Key);

            //SubTitle.Draw();

            //Scene.EndForeground();
        }


        private void PrepareLetters()
        {
            Letters = new List<KeyValuePair<Text,Vector3>>();

            var text = Commander.Data;
            var letterSizeX = Commander.AbsoluteSize.X / text.Length;
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
                t.Blend = BlendType.Add;

                Letters.Add(new KeyValuePair<Text,Vector3>(t, endPosition));
            }

            if (text.Length % 2 != 0)
            {
                t = Commander.Clone();
                t.Data = text[text.Length / 2].ToString();
                t.Position = Commander.Position;
                t.CenterIt();
                t.Blend = BlendType.Add;

                Letters.Add(new KeyValuePair<Text, Vector3>(t, Commander.Position));
            }

            for (int i = 1; i <= centerLetterIndex; i++)
            {
                endPosition = Commander.Position + new Vector3(i * letterSizeX, 0, 0);

                t = Commander.Clone();
                t.Data = text[centerLetterIndex + i].ToString();
                t.Position = endPosition + new Vector3(i * letterSizeX / 2, 0, 0);
                t.CenterIt();
                t.Blend = BlendType.Add;

                Letters.Add(new KeyValuePair<Text,Vector3>(t, endPosition));
            }
        }
    }
}
