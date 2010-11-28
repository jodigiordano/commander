//=====================================================================
//
// Gestionnaire des steerings
//
//=====================================================================

//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;

//namespace EndOfCivilizations
//{
//    public class GestionnaireSteerings : GestionnaireEffets
//    {
//        //=====================================================================
//        // Logique
//        //=====================================================================

//        public override void Update(GameTime gameTime)
//        {
//            List<IIntelligent> objets = new List<IIntelligent>();

//            for (int i = 0; i < Effets.Count; i++)
//            {
//                if (!objets.Contains((IIntelligent)Effets[i].Objet))
//                    objets.Add((IIntelligent)Effets[i].Objet);
//            }

//            base.Update(gameTime);

//            for (int i = 0; i < objets.Count; i++)
//            {
//                objets[i].IA.ApplySteeringForce(objets[i].Steering, (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100.0f);
//                objets[i].Steering = Vector3.Zero;
//            }
//        }
//    }
//}
