namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextTypeWriter
    {
        private string Raw;
        private double Frequency;
        private bool StopAtEndOfSentence;
        private Vector2 CanvasSize;
        private Scene Scene;
        private Vector3 StartingPosition;
        private float Size;
        private double PauseTime;

        private int CharWidth;
        private int NbCharPerLine;
        private int NbCharWritten;
        private int NbCharWrittenLine;
        private double Counter;
        private bool PlaySfx;
        private List<string> Sfxs;

        public Text Text { get; private set; }


        public TextTypeWriter(
            string text,
            Color color,
            Vector3 startingPosition,
            string police,
            float size,
            Vector2 canvasSize,
            double frequency,
            bool StopAtEndOfSentence,
            double pauseTime,
            bool playSfx,
            List<string> sfxs,
            Scene scene)
        {
            this.CanvasSize = canvasSize;
            this.Frequency = frequency;
            this.StopAtEndOfSentence = StopAtEndOfSentence;
            this.Scene = scene;
            this.StartingPosition = startingPosition;
            this.PauseTime = pauseTime;
            this.Size = size;
            this.PlaySfx = playSfx;
            this.Sfxs = sfxs;

            Text iv = new Text("a", police, Color.White, Vector3.Zero);
            iv.SizeX = this.Size;
            CharWidth = (int) iv.TextSize.X;

            NbCharPerLine = (int) (canvasSize.X / CharWidth);

            Raw = "";

            string[] sentences = text.Split(new string[] { "\n\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sentence in sentences)
            {
                string[] words = sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                NbCharWrittenLine = 0;

                foreach (var word in words)
                {
                    if (word.Length > NbCharPerLine - NbCharWrittenLine)
                    {
                        Raw += "\n\n";
                        NbCharWrittenLine = 0;
                    }

                    Raw += word + " ";
                    NbCharWrittenLine += word.Length + 1;
                }

                Raw += "\n\n\n";
            }


            Counter = 0;

            Text = new Text("", police, color, StartingPosition);
            Text.SizeX = this.Size;
        }


        public bool Finished
        {
            get { return NbCharWritten >= Raw.Length; }
            set
            {
                NbCharWritten = Raw.Length;
                Text.Data = Raw;
            }
        }


        //public void Show()
        //{
        //    Scene.Add(Text);
        //}


        //public void Hide()
        //{
        //    Scene.Remove(Text);
        //}


        public void Update(GameTime gameTime)
        {
            Counter -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Counter <= 0 && NbCharWritten < Raw.Length)
            {
                Counter = (StopAtEndOfSentence && Raw[NbCharWritten] == '.') ? this.PauseTime : Frequency;

                Text.Data += Raw[NbCharWritten];

                NbCharWritten++;

                if (PlaySfx)
                    Audio.PlaySfx(@"Partie", Sfxs[Main.Random.Next(0, Sfxs.Count)]);
            }
        }


        public void Draw()
        {
            Scene.Add(Text);
        }
    }
}
