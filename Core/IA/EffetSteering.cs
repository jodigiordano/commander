////=====================================================================
////
////
////
////=====================================================================

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Bnoerj.AI.Steering;

//namespace EndOfCivilizations
//{
//    public class EffetSteering : Effet
//    {

//        //=====================================================================
//        // Attributs
//        //=====================================================================

//        public enum TypeComportement
//        {
//            SePromener,
//            AtteindreEmplacementStatique,
//            AtteindreEmplacementDynamique,
//            SuivreChemin,
//            EviterObstacles
//        }

//        private IIntelligent ObjetIntelligent       { get; set; }
//        public TypeComportement Comportement        { get; set; }
//        public Vector3 emplacementStatique;
//        public IVisible EmplacementDynamique        { get; set; }
//        public Pathway Chemin                       { get; set; }
//        public List<SphericalObstacle> Obstacles    { get; set; }



//        //=====================================================================
//        // Logique
//        //=====================================================================

//        protected override void InitLogique()
//        {
//            ObjetIntelligent = (IIntelligent)Objet;
//            ObjetIntelligent.IA.Position = Objet.position;
//        }

//        protected override void LogiqueLineaire()
//        {
//            Vector3 steering = ObjetIntelligent.Steering;
//            Vector3 steerForX;

//            switch (Comportement)
//            {
//                case TypeComportement.SePromener:
//                    steerForX = ((IIntelligent)Objet).IA.SteerForWander((float)tempsUnTick / 100.0f, 3);
//                    ObjetIntelligent.Steering = new Vector3(
//                        steering.X + steerForX.X,
//                        steering.Y + steerForX.Y,
//                        steering.Z + steerForX.Z);
//                    break;

//                case TypeComportement.AtteindreEmplacementStatique:
//                    steerForX = ((IIntelligent)Objet).IA.SteerForArrival(emplacementStatique);
//                    ObjetIntelligent.Steering = new Vector3(
//                        steering.X + steerForX.X,
//                        steering.Y + steerForX.Y,
//                        steering.Z + steerForX.Z);
//                    break;

//                case TypeComportement.AtteindreEmplacementDynamique:
//                    steerForX = ((IIntelligent)Objet).IA.SteerForArrival(this.EmplacementDynamique.position);
//                    ObjetIntelligent.Steering = new Vector3(
//                        steering.X + steerForX.X,
//                        steering.Y + steerForX.Y,
//                        steering.Z + steerForX.Z);
//                    break;

//                case TypeComportement.SuivreChemin:
//                    steerForX = ((IIntelligent)Objet).IA.SteerToFollowPath(1, 3, Chemin);
//                    ObjetIntelligent.Steering = new Vector3(
//                        steering.X + steerForX.X,
//                        steering.Y + steerForX.Y,
//                        steering.Z + steerForX.Z);
//                    break;

//                case TypeComportement.EviterObstacles:
//                    steerForX = ((IIntelligent)Objet).IA.SteerToAvoidObstacles(2, Obstacles);
//                    ObjetIntelligent.Steering = new Vector3(
//                        steering.X + steerForX.X * 10000,
//                        steering.Y + steerForX.Y * 10000,
//                        steering.Z + steerForX.Z * 10000);
//                    break;
//            }
//        }

//        protected override void LogiqueApresDuree()
//        {
//            throw new Exception("Non supporté.");
//        }

//        protected override void LogiqueMaintenant()
//        {
//            throw new Exception("Non supporté.");
//        }
//    }
//}
