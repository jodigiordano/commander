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
            switch (Preferences.Target)
            {
                case Setting.WindowsDemo:
                    EndOfDemo = (Main.SaveGame.Progression[0] > 0 &&
                       Main.SaveGame.Progression[1] > 0 &&
                       Main.SaveGame.Progression[2] > 0);
                    break;

                case Setting.WindowsFull:
                    EndOfDemo = false;
                    break;

                case Setting.Xbox360:
                    int nbTableauxTermines = 0;

                    for (int i = 0; i < 9; i++)
                        if (Main.SaveGame.Progression[i] > 0)
                            nbTableauxTermines++;

                    int nbPartiesJouees = 0;

                    for (int i = 0; i < 9; i++)
                        nbPartiesJouees += Math.Abs(Main.SaveGame.Progression[i]);

                    EndOfDemo = (nbTableauxTermines >= 3) || (nbPartiesJouees > 15);

                    break;
            }
        }
    }
}
