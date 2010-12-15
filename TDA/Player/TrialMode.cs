namespace TDA
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    class TrialMode
    {
        public bool EndOfDemo;

        private Main Main;


        public TrialMode(Main main)
        {
            Main = main;
            EndOfDemo = false;
        }


        public void Update(GameTime gameTime)
        {
            if (Active)
                verifyEndOfDemo();
        }


        public bool Active
        {
            get
            {
                return (Preferences.Target == Setting.WindowsDemo) ? true :
                       (Preferences.Target == Setting.WindowsFull) ? false : Guide.IsTrialMode;
            }
        }


        private void verifyEndOfDemo()
        {
            int level = 0;

            switch (Preferences.Target)
            {
                case Setting.WindowsDemo:
                    EndOfDemo = Main.SaveGame.Progress.TryGetValue(0, out level) && level > 0 &&
                                Main.SaveGame.Progress.TryGetValue(1, out level) && level > 0 &&
                                Main.SaveGame.Progress.TryGetValue(2, out level) && level > 0;
                    break;

                case Setting.WindowsFull:
                    EndOfDemo = false;
                    break;

                case Setting.Xbox360:
                    int nbTableauxTermines = 0;

                    for (int i = 0; i < 9; i++)
                    {
                        if (Main.SaveGame.Progress.TryGetValue(i, out level) && level > 0)
                            nbTableauxTermines++;
                    }

                    int nbPartiesJouees = 0;

                    for (int i = 0; i < 9; i++)
                    {
                        Main.SaveGame.Progress.TryGetValue(i, out level);

                        nbPartiesJouees += Math.Abs(level);
                    }

                    EndOfDemo = (nbTableauxTermines >= 3) || (nbPartiesJouees > 15);

                    break;
            }
        }
    }
}
