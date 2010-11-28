//=====================================================================
//
// Usine d'effets
// =================================
// utilité: créer facilement des effets complexes
// note: effet == transformation sur un IVisible
//
// Pour définir un effet :
// - Sous-classer Effet
// - Ajouter une méthode statique dans cette classe qui utilise le nouvel
//   effet
//
// Copyright © Jodi Giordano, Julien Théron, 2009
//=====================================================================

namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    static class EffetsPredefinis
    {
        private static Random random = new Random();

        public static EffetDeplacement deplacerMaintenant(Vector3 position)
        {
            EffetDeplacement e = new EffetDeplacement();

            e.PositionFin = position;
            e.Progression = AbstractEffet.TypeProgression.Maintenant;
            e.Duree = 500;

            return e;
        }

        public static EffetDeplacementTrajet deplacerTexteEnArcDroite(Vector2 positionDebut, Vector2 positionFin, double depart, double duree)
        {
            EffetDeplacementTrajet eDt = new EffetDeplacementTrajet();

            double[] temps = new double[] {
                0,
                (random.Next(10, 40) / 100.0) * duree,
                duree
            };

            Vector2[] positions = new Vector2[] {
                positionDebut,
                new Vector2(positionFin.X + 20, (positionDebut.Y + positionFin.Y) / 2f),
                positionFin
            };

            Trajet2D t = new Trajet2D(positions, temps);

            eDt.Trajet = t;
            eDt.Progression = AbstractEffet.TypeProgression.Lineaire;
            eDt.Delai = depart;
            eDt.Duree = duree;

            return eDt;
        }
    }
}
