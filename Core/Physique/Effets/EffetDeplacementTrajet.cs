//=====================================================================
//
//
//
//=====================================================================

namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    public class EffetDeplacementTrajet : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D Trajet { private get; set; }
        public float Rotation { get; private set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void LogicLinear()
        {
            Objet.Position = new Vector3(Trajet.position(ElaspedTime), Objet.Position.Z);
            Rotation = Trajet.rotation(ElaspedTime);
        }

        protected override void LogicAfter()
        {
            LogicLinear();
        }

        protected override void LogicNow()
        {
            Objet.Position = new Vector3(Trajet.position(Length), Objet.Position.Z);
            Rotation = Trajet.rotation(Length);
        }

        protected override void InitializeLogic()
        {
            Objet.Position = new Vector3(Trajet.positionDepart(), Objet.Position.Z);
            Rotation = Trajet.rotation(0);
        }
    }
}
