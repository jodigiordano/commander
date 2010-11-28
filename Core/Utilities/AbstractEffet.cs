namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public abstract class AbstractEffet : ICloneable
    {
        /// <summary>
        /// Type de progression de l'effet
        /// </summary>
        public enum TypeProgression
        {
            Lineaire,
            Maintenant,
            ApresDuree
        }

        //=====================================================================
        // Attributs
        //=====================================================================

        [ContentSerializer(Optional = true)]
        public double Duree { get; set; }                   // Durée de l'effet

        [ContentSerializer(Optional = true)]
        public TypeProgression Progression { get; set; }    // Façon dont est appliqué l'effet

        [ContentSerializer(Optional = true)]
        public double Delai { get; set; }                   // Temps mort avant de partir l'effet

        [ContentSerializerIgnore]
        public bool Termine { get; set; }                   // Est-ce que l'effet est terminé

        [ContentSerializerIgnore]
        public object objet { get; set; }                   // Objet sur lequel s'applique l'effet

        private double tempsRestantDebut;                   // Temps à attendre avant que l'effet débute (en tenant compte du temps mort)
        private double tempsRestantFin;                     // Temps à attendre avant que l'effet s'arrête
        protected double tempsRelatif;
        protected double tempsUnTick;
        private bool initialisation;


        //=====================================================================
        // Constructeur
        //=====================================================================

        /// <summary>
        /// Constructeur d'un effet
        /// </summary>
        public AbstractEffet()
        {
            Progression = TypeProgression.Lineaire;
            Delai = 0;
            Termine = false;
            Duree = 0;
            initialisation = true;

            init();
        }


        //=====================================================================
        // Logique
        //=====================================================================

        /// <summary>
        /// Prochaine application de l'effet
        /// </summary>
        /// <remarks>
        /// Important: lorsqu'on redéfini suivant(), ne pas oublier de
        /// faire un appel à base.suivant(gameTime) pour que l'effet soit
        /// initialisé
        /// </remarks>
        /// <param name="gameTime">Temps</param>
        public void Update(GameTime gameTime)
        {
            if (initialisation)
            {
                init();
                InitLogique();

                initialisation = false;
            }

            // détermine si l'effet est terminé
            Termine = (tempsRestantFin <= 0 || gameTime == null);

            if (gameTime != null)
            {
                if (tempsRestantDebut > 0)
                {
                    tempsRestantDebut -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    return;
                }
                else
                {
                    tempsRelatif = Duree - tempsRestantFin;
                    tempsRestantFin -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                tempsUnTick = gameTime.ElapsedGameTime.TotalMilliseconds;
            }


            if (!Termine && Progression == TypeProgression.Maintenant)
                LogiqueMaintenant();

            else if (!Termine && Progression == TypeProgression.Lineaire)
                LogiqueLineaire();

            else if (Termine && Progression == TypeProgression.ApresDuree)
                LogiqueApresDuree();

            // terminer l'effet
            if (Termine)
                LogiqueTermine();
        }


        /// <summary>
        /// Logiques à sous-classer
        /// </summary>
        protected virtual void InitLogique() { }
        protected virtual void LogiqueLineaire() { }
        protected virtual void LogiqueApresDuree() { }
        protected virtual void LogiqueMaintenant() { }


        /// <summary>
        /// Initialisation de l'effet
        /// </summary>
        public void init()
        {
            tempsRestantFin = Duree;
            tempsRestantDebut = Delai;
            Termine = false;
            initialisation = true;
        }

        /// <summary>
        /// Logique effectuée lorsque l'effet est terminé, peut importe le type de progression
        /// </summary>
        protected virtual void LogiqueTermine()
        {

        }


        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}

