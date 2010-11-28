namespace TDA
{
    using System;
    using System.Collections.Generic;


    class JoueurCommun
    {
        public int Pointage;
        public int ReserveUnites;
        public int PointsDeVie;


        public JoueurCommun(int reserveUnites)
        {
            this.Pointage = 0;
            this.ReserveUnites = reserveUnites;
        }


        public JoueurCommun()
        {
            this.Pointage = 0;
            this.ReserveUnites = 0;
            this.PointsDeVie = 0;
        }
    }
}