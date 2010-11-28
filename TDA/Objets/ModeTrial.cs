namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    class ModeTrial : GameComponent
    {
        public bool Actif       { get; private set; }
        public bool FinDemo     { get; private set; }

        private Main Main;

        public ModeTrial(Main main)
            : base(main)
        {
            Main = main;

            FinDemo = false;

            verifierModeTrial();
        }

        public override void Update(GameTime gameTime)
        {
            verifierModeTrial();

            if (Actif)
                verifierFinDemo();
        }

        private void verifierModeTrial()
        {
#if WINDOWS && TRIAL
            Actif = true;
#elif WINDOWS
            Actif = false;
#else
            Actif = Guide.IsTrialMode;
#endif
        }

        private void verifierFinDemo()
        {
#if WINDOWS
            FinDemo = (Main.Sauvegarde.Progression[0] > 0 &&
                       Main.Sauvegarde.Progression[1] > 0 &&
                       Main.Sauvegarde.Progression[2] > 0);
#else
            // verifier que 3 tableaux sont termines
            int nbTableauxTermines = 0;

            for (int i = 0; i < 9; i++)
                if (Main.Sauvegarde.Progression[i] > 0)
                    nbTableauxTermines++;

            // verifier que 20 parties ont ete joues
            int nbPartiesJouees = 0;

            for (int i = 0; i < 9; i++)
                nbPartiesJouees += Math.Abs(Main.Sauvegarde.Progression[i]);

            FinDemo = (nbTableauxTermines >= 3) || (nbPartiesJouees > 15);
#endif
        }
    }
}
