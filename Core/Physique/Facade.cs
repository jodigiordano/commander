//=============================================================================
//
// Point d'entrée dans la librairie
//
//=============================================================================

namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    
    public static class Facade
    {
        public static void Initialize() {}

        public static bool collisionPixels(ref Matrix transformee1, Texture2D texture1, ref Color[] couleurs1, ref Matrix transformee2, Texture2D texture2, ref Color[] couleurs2)
        {
            return Collisions.collisionPixels(ref transformee1, texture1, ref couleurs1, ref transformee2, texture2, ref couleurs2);
        }

        public static bool collisionPointRectangle(ref Vector2 point, RectanglePhysique rectangle)
        {
            return Collisions.collisionPointRectangle(ref point, rectangle);
        }

        public static bool collisionCercleRectangle(Cercle cercle, RectanglePhysique rectangle)
        {
            return Collisions.collisionCercleRectangle(cercle, rectangle);
        }

        public static bool collisionRectangleRectangle(RectanglePhysique rectangle, RectanglePhysique rectangle_2)
        {
            return Collisions.collisionRectangleRectangle(rectangle, rectangle_2);
        }

        public static bool collisionTriangleRectangle(Triangle triangle, RectanglePhysique rectangle)
        {
            return Collisions.collisionTriangleRectangle(triangle, rectangle);
        }

        public static bool collisionLigneRectangle(Ligne ligne, RectanglePhysique rectangle)
        {
            return Collisions.collisionLigneRectangle(ligne, rectangle);
        }

        public static bool collisionPointCercle(ref Vector3 point, Cercle cercle)
        {
            return Collisions.collisionPointCercle(ref point, cercle);
        }

        public static bool collisionCercleCercle(Cercle cercle1, Cercle cercle2)
        {
            return Collisions.collisionCercleCercle(cercle1, cercle2);
        }
    }
}
