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


namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    public static class EffetsPredefinis
    {
        private static Random random = new Random();

        public static EffetTaille tailleMaintenant(float taille)
        {
            EffetTaille et = new EffetTaille();
            et.Progression = AbstractEffet.TypeProgression.Maintenant;
            et.Duree = 500;
            et.Trajet =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(taille, taille)
                }, new double[]
                {
                    0,
                    500
                });

            return et;
        }

        public static EffetTaille tailleOriginaleMaintenant()
        {
            EffetTaille et = new EffetTaille();
            et.Progression = AbstractEffet.TypeProgression.Maintenant;
            et.Duree = 500;
            et.Trajet =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 1)
                }, new double[]
                {
                    0,
                    500
                });

            return et;
        }

        public static List<AbstractEffet> dessinerPartieMaintenant(Rectangle partieVisible, Vector2 nouvelleOrigine)
        {
            List<AbstractEffet> effets = new List<AbstractEffet>();

            EffetDessinerPartie eDp = new EffetDessinerPartie();
            eDp.DessinerPartie = true;
            eDp.PartieVisible = partieVisible;
            eDp.Progression = AbstractEffet.TypeProgression.Maintenant;
            eDp.Duree = 500;
            effets.Add(eDp);

            EffetRecentrer eR = new EffetRecentrer();
            eR.OrigineFin = nouvelleOrigine;
            eR.Duree = 500;
            effets.Add(eR);

            return effets;
        }

        public static EffetFadeCouleur fadeInFrom0(int alphaFinal, double delai, double duree)
        {
            EffetFadeCouleur e = new EffetFadeCouleur();

            e.Trajet =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(alphaFinal / 255.0f / 2.0f, alphaFinal / 255.0f / 2.0f),
                    new Vector2(alphaFinal / 255.0f, alphaFinal / 255.0f)
                }, new double[]
                {
                    0,
                    duree / 2.0f,
                    duree
                });

            e.Progression = AbstractEffet.TypeProgression.Lineaire;
            e.Delai = delai;
            e.Duree = duree;

            return e;
        }

        public static EffetFadeCouleur fadeOutTo0(int alphaDepart, double delai, double duree)
        {
            EffetFadeCouleur e = new EffetFadeCouleur();

            e.Trajet =
                new Trajet2D(new Vector2[]
                {
                    new Vector2(alphaDepart / 255.0f, alphaDepart / 255.0f),
                    new Vector2(alphaDepart / 255.0f / 2.0f, alphaDepart / 255.0f / 2.0f),
                    new Vector2(0, 0)
                }, new double[]
                {
                    0,
                    duree / 2.0f,
                    duree
                });

            e.Delai = delai;
            e.Duree = duree;

            return e;
        }

        public static EffetRotationCamera rotationCamera(float rotationRadians, double delai, double duree)
        {
            EffetRotationCamera e = new EffetRotationCamera();
            e.Delai = delai;
            e.Duree = duree;
            e.Progression = AbstractEffet.TypeProgression.Lineaire;
            e.Rotation = rotationRadians;

            return e;
        }
    }
}
