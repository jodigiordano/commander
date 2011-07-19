namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CommanderAnimation
    {
        private Image Background;
        private Text Commander;
        private Scene Scene;
        private double TimeBeforeIn;

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
                Blend = TypeBlend.Add
            };

            Commander = new Text("Commander", "Pixelite")
            {
                Color = Color.Black,
                SizeX = 12,
                VisualPriority = VisualPriorities.Default.IntroCommanderText
            };

            TimeBeforeIn = IntroCutscene.Timing["CommanderIn"];

            PrepareLetters();

            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn, 3000));
            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 13000, 3000));

            foreach (var kvp in Letters)
            {
                kvp.Key.Alpha = 0;

                Scene.PhysicalEffects.Add(kvp.Key, Core.Physics.PhysicalEffects.Move(kvp.Value, TimeBeforeIn + 3000, 10000));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeIn + Main.Random.Next(3000, 5000), 1500));
                Scene.VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn + 11000, 3000));
            }
        }


        public void Update()
        {

        }


        public void Draw()
        {
            Scene.BeginForeground();
            Scene.Add(Background);

            foreach (var l in Letters)
                Scene.Add(l.Key);

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
