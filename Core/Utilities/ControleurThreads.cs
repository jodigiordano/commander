namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Xna.Framework;

    public class ControleurThreads
    {
        private volatile GameTime GameTime;

        private AutoResetEvent PrincipalDebute;
        private AutoResetEvent PrincipalTermine;

        private AutoResetEvent UpdateSpecifiqueDebute;
        private AutoResetEvent UpdateSpecifiqueTermine;

        private AutoResetEvent UpdateVisuelDebute;
        private AutoResetEvent UpdateVisuelTermine;

        private AutoResetEvent UpdateAutreDebute;
        private AutoResetEvent UpdateAutreTermine;


        public ControleurThreads()
        {
            PrincipalDebute = new AutoResetEvent(false);
            PrincipalTermine = new AutoResetEvent(false);

            UpdateSpecifiqueDebute = new AutoResetEvent(false);
            UpdateSpecifiqueTermine = new AutoResetEvent(false);

            UpdateVisuelDebute = new AutoResetEvent(false);
            UpdateVisuelTermine = new AutoResetEvent(false);

            UpdateAutreDebute = new AutoResetEvent(false);
            UpdateAutreTermine = new AutoResetEvent(false);

            //reset the values
            InitializeThisShit();
        }


        public void InitializeThisShit()
        {
            PrincipalDebute.Reset(); // Etat == Non-signale
            PrincipalTermine.Reset();

            UpdateSpecifiqueDebute.Reset();
            UpdateSpecifiqueTermine.Reset();

            UpdateVisuelDebute.Reset();
            UpdateVisuelTermine.Reset();

            UpdateAutreDebute.Reset();
            UpdateAutreTermine.Reset();
        }


        public void FinalizeThisShit()
        {
            PrincipalDebute.Close(); // Ressources liberees
            PrincipalTermine.Close();

            UpdateSpecifiqueDebute.Close();
            UpdateSpecifiqueTermine.Close();

            UpdateVisuelDebute.Close();
            UpdateVisuelTermine.Close();

            UpdateAutreDebute.Close();
            UpdateAutreTermine.Close();
        }

        public void DebuterPrincipal(out GameTime gameTime)
        {
            PrincipalDebute.WaitOne(); // Attente du signal de depart

            Thread.MemoryBarrier(); // utile ?

            gameTime = this.GameTime;
        }

        public void DebuterUpdateSpecifique(out GameTime gameTime)
        {
            UpdateSpecifiqueDebute.WaitOne();

            Thread.MemoryBarrier();

            gameTime = this.GameTime;
        }

        public void DebuterUpdateVisuel(out GameTime gameTime)
        {
            UpdateVisuelDebute.WaitOne();

            Thread.MemoryBarrier();

            gameTime = this.GameTime;
        }

        public void DebuterUpdateAutre(out GameTime gameTime)
        {
            UpdateAutreDebute.WaitOne();

            Thread.MemoryBarrier();

            gameTime = this.GameTime;
        }


        public void TerminerPrincipal()
        {
            Thread.MemoryBarrier(); // utile ?

            PrincipalTermine.Set();  // evenement signale
        }

        public void TerminerUpdateSpecifique()
        {
            Thread.MemoryBarrier(); // utile ?

            UpdateSpecifiqueTermine.Set();  // evenement signale
        }

        public void TerminerUpdateVisuel()
        {
            Thread.MemoryBarrier(); // utile ?

            UpdateVisuelTermine.Set();  // evenement signale
        }

        public void TerminerUpdateAutre()
        {
            Thread.MemoryBarrier(); // utile ?

            UpdateAutreTermine.Set();  // evenement signale
        }

        public void AVosMarquesPretsPartez(GameTime gameTime)
        {
            GameTime = gameTime;

            PrincipalDebute.Set();
            //UpdateSpecifiqueDebute.Set();
            UpdateVisuelDebute.Set();
            //UpdateAutreDebute.Set();
        }

        public void BonCestQuiLeRetardataire()
        {
            PrincipalTermine.WaitOne();
            //UpdateSpecifiqueTermine.WaitOne();
            UpdateVisuelTermine.WaitOne();
            //UpdateAutreTermine.WaitOne();
        }
    }
}
