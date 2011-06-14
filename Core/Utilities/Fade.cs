namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Fade
    {
        public static List<int> generateFades(Fade fade)
        {
            GameTime gameTime = new GameTime();

            List<int> fades = new List<int>();

            int unSur60 = (int)((1.0f / 60.0f) * 1000);

            while (!fade.Termine)
            {
                fades.Add(fade.suivant(gameTime));

                gameTime = new GameTime(
                    gameTime.TotalGameTime + new TimeSpan(0, 0, 0, 0, unSur60),
                    new TimeSpan(0, 0, 0, 0, unSur60));
            }

            fades.Add(fade.Final);

            return fades;
        }


        public static List<byte> generateByteFades(Fade fade)
        {
            List<int> intFades = generateFades(fade);
            List<byte> byteFades = new List<byte>();

            for (int i = 0; i < intFades.Count; i++)
            {
                byteFades.Add((byte)intFades[i]);
            }

            return byteFades;
        }


        public enum Type { IN, OUT, INOUT }

        protected int fadeActuel;
        protected Type type;

        protected double tempsFadeIn;
        protected double tempsFadeOut;
        protected bool fadeInTermine = false;
        protected bool fadeOutTermine = false;

        protected double tempsRestantFadeIn = 0;
        protected double tempsRestantFadeOut = 0;
        protected double stepFadeIn;
        protected double stepFadeOut;

        protected int fadeOutStartFrom;
        protected int fadeInStartFrom;

        protected int min = 0;
        protected int max = 255;

        public Fade(int fadeActuel, Type type, double tempsFadeIn, double tempsFadeOut) {
            this.fadeActuel = fadeActuel;
            this.type = type;
            this.tempsFadeIn = tempsFadeIn;
            tempsRestantFadeIn = this.tempsFadeIn;
            this.tempsFadeOut = tempsFadeOut;
            tempsRestantFadeOut = this.tempsFadeOut;

            switch (type)
            {
                case Type.OUT:
                    fadeInTermine = true;
                    this.fadeOutStartFrom = fadeActuel;
                    this.stepFadeOut = (fadeOutStartFrom - min) / this.tempsFadeOut;
                    break;

                case Type.IN:
                    fadeOutTermine = true;
                    fadeActuel = min;
                    fadeInStartFrom = fadeActuel;
                    this.stepFadeIn = (max - this.fadeInStartFrom) / this.tempsFadeIn;
                    break;

                case Type.INOUT:
                    fadeActuel = min;
                    this.fadeInStartFrom = fadeActuel;
                    this.fadeOutStartFrom = max;
                    this.stepFadeIn = (max - this.fadeInStartFrom) / this.tempsFadeIn;
                    this.stepFadeOut = (fadeOutStartFrom - min) / this.tempsFadeOut;
                    break;
            }
        }

        public Fade(int fadeActuel, Type type, double tempsFadeIn, int max, double tempsFadeOut, int min)
        {
            this.fadeActuel = fadeActuel;
            this.type = type;
            this.tempsFadeIn = tempsFadeIn;
            tempsRestantFadeIn = this.tempsFadeIn;
            this.tempsFadeOut = tempsFadeOut;
            tempsRestantFadeOut = this.tempsFadeOut;
            this.min = min;
            this.max = max;

            switch (type)
            {
                case Type.OUT:
                    fadeInTermine = true;
                    fadeOutStartFrom = fadeActuel;
                    stepFadeOut = (fadeOutStartFrom - min) / tempsFadeOut;
                    break;

                case Type.IN:
                    fadeOutTermine = true;
                    fadeInStartFrom = fadeActuel;
                    stepFadeIn = (max - fadeInStartFrom) / tempsFadeIn;
                    break;

                case Type.INOUT:
                    fadeInStartFrom = fadeActuel;
                    fadeOutStartFrom = max;
                    stepFadeIn = (max - fadeInStartFrom) / tempsFadeIn;
                    stepFadeOut = (fadeOutStartFrom - min) / tempsFadeOut;
                    break;
            }
        }

        public Fade()
        {
        }

        public double TempsFadeIn
        {
            set
            {
                // Incrémentation/décrémentation du temps restant si on venait
                // à modifier le tempsFadeIn en cours de route
                tempsRestantFadeIn += value - tempsFadeIn;
                
                tempsFadeIn = value;
                stepFadeIn = (max - fadeInStartFrom) / tempsFadeIn;
            }
        }

        public double TempsFadeOut
        {
            set
            {
                // Incrémentation/décrémentation du temps restant si on venait
                // à modifier le tempsFadeOut en cours de route
                tempsRestantFadeOut += value - tempsFadeOut;
                
                tempsFadeOut = value;
                stepFadeOut = (fadeOutStartFrom - min) / tempsFadeOut;
            }
        }

        public void reset()
        {
            switch(type)
            {
                case Type.OUT:
                    tempsRestantFadeOut = tempsFadeOut;
                    fadeOutTermine = false;
                    break;

                case Type.IN:
                    tempsRestantFadeIn = tempsFadeIn;
                    fadeInTermine = false;
                    break;


                case Type.INOUT:
                    tempsRestantFadeIn = tempsFadeIn;
                    fadeInTermine = false;
                    tempsRestantFadeOut = tempsFadeOut;
                    fadeOutTermine = false;
                    break;
                default: break;
            }
        }

        public Type TypeFade
        {
            get { return type; }
        }

        public double TempsTotal
        {
            get { return tempsFadeIn + tempsFadeOut; }
        }

        public int FadeInStartFrom
        {
            set
            {
                fadeInStartFrom = value;
                stepFadeIn = (max - value) / this.tempsFadeIn;
            }
        }


        public int Min
        {
            get { return min; }
            set
            {
                min = value;
                fadeActuel = (int) MathHelper.Clamp(fadeActuel, value, max); // utilité ? quand on set le min/max, on s'assure que le fade actuel est compris entre ces valeurs
                //stepFadeIn = (max - fadeInStartFrom) / tempsFadeIn;
                stepFadeOut = (fadeOutStartFrom - value) / tempsFadeOut;
            }
        }


        public int Max
        {
            get { return max; }
            set
            {
                max = value;
                fadeActuel = (int) MathHelper.Clamp(fadeActuel, value, max); // utilité ?
                stepFadeIn = (max - fadeInStartFrom) / tempsFadeIn;
                //stepFadeOut = (fadeOutStartFrom - value) / tempsFadeOut;
            }
        }


        public int suivant(GameTime gameTime)
        {
            if (!fadeInTermine)
                fadeIn(gameTime);

            else if (!fadeOutTermine)
                fadeOut(gameTime);

            return fadeActuel;
        }

         private void fadeIn(GameTime gameTime)
         {
             double tempsRelatif = tempsFadeIn - tempsRestantFadeIn;
             fadeActuel = (int)Math.Min(max, fadeInStartFrom + tempsRelatif * stepFadeIn);

             //if (tempsRestantFadeIn <= 0)
             //    fadeInTermine = true;

             tempsRestantFadeIn -= gameTime.ElapsedGameTime.TotalMilliseconds;

             fadeInTermine = (fadeActuel == max);
        }

         private void fadeOut(GameTime gameTime)
         {
             double tempsRelatif = tempsFadeOut - tempsRestantFadeOut;
             fadeActuel = (int)Math.Max(min, fadeOutStartFrom - tempsRelatif * stepFadeOut);

             //if (tempsRestantFadeOut <= 0)
             //    fadeOutTermine = true;

             tempsRestantFadeOut -= gameTime.ElapsedGameTime.TotalMilliseconds;

             fadeOutTermine = (fadeActuel == min);
        }

        public int Final
        {
            get
            {
                int final = 0;

                switch (type)
                {
                    case Type.OUT:
                    case Type.INOUT:
                        final = min;
                        break;

                    case Type.IN:
                        final = max;
                        break;

                    default:
                        break;
                }

                return final;
            }
        }


        public bool Termine {
            get { return fadeInTermine && fadeOutTermine; }
        }

        public int init()
        {
            return fadeActuel;
        }
    }
}
