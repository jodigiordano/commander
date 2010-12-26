//=====================================================================
//
// Gestion des collisions
//
//=====================================================================

namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Collisions
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public enum SensLigne
        {
            SensMontre,
            ContreSensMontre,
            Ligne
        }


        //=====================================================================
        // Logique
        //=====================================================================

        /// <summary>
        /// Collision ligne <--> ligne 2D
        /// </summary>
        /// <param name="l1p1">Début ligne 1</param>
        /// <param name="l1p2">Fin ligne 1</param>
        /// <param name="l2p1">Début ligne 2</param>
        /// <param name="l2p2">Fin ligne 2</param>
        /// <returns>Est-ce qu'il y a une collision entre les deux lignes</returns>
        public static bool collisionLigneLigne(ref Vector2 l1p1, ref Vector2 l1p2, ref Vector2 l2p1, ref Vector2 l2p2)
        {
            SensLigne test1a, test1b, test2a, test2b;


            test1a = getSensLigne(ref l1p1, ref l1p2, ref l2p1);
            test1b = getSensLigne(ref l1p1, ref l1p2, ref l2p2);

            if (test1a != test1b)
            {

                test2a = getSensLigne(ref l2p1, ref l2p2, ref l1p1);
                test2b = getSensLigne(ref l2p1, ref l2p2, ref l1p2);

                if (test2a != test2b)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Collision ligne <--> rectangle 2D
        /// </summary>
        /// <param name="pointDebut">Ligne début</param>
        /// <param name="pointFin">Ligne fin</param>
        /// <param name="rectangle">Rectangle</param>
        /// <returns>Est-ce qu'il y a une collision entre la ligne et le rectangle</returns>
        public static bool collisionLigneRectangle(ref Vector2 pointDebut, ref Vector2 pointFin, RectanglePhysique rectangle)
        {
            Vector2 v1 = new Vector2(rectangle.X, rectangle.Y);
            Vector2 v2 = new Vector2(rectangle.X, rectangle.Y + rectangle.Height);
            Vector2 v3 = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
            Vector2 v4 = new Vector2(rectangle.X + rectangle.Width, rectangle.Y);

            if (
             collisionLigneLigne(ref pointDebut, ref pointFin, ref v1, ref v2) ||
             collisionLigneLigne(ref pointDebut, ref pointFin, ref v1, ref v4) ||
             collisionLigneLigne(ref pointDebut, ref pointFin, ref v4, ref v3) ||
             collisionLigneLigne(ref pointDebut, ref pointFin, ref v2, ref v3))
                return true;

            return false;
        }


        /// <summary>
        /// Collision ligne <--> rectangle 2D
        /// </summary>
        /// <param name="ligne">Ligne</param>
        /// <param name="rectangle">Rectangle</param>
        /// <returns>vrai s'il y a une collision entre la ligne et le rectangle</returns>
        public static bool collisionLigneRectangle(Ligne ligne, RectanglePhysique rectangle)
        {
            Vector2 debut = ligne.DebutV2;
            Vector2 fin = ligne.FinV2;

            return collisionLigneRectangle(ref debut, ref fin, rectangle);
        }


        /// <summary>
        /// Collision triangle <--> rectangle 2D
        /// </summary>
        /// <param name="triangle">Triangle</param>
        /// <param name="rectangle">Rectangle</param>
        /// <returns>Est-ce qu'il y a une collision entre le triangle et le rectangle</returns>
        public static bool collisionTriangleRectangle(Triangle triangle, RectanglePhysique rectangle)
        {
            Vector2 s1 = triangle.Sommet1;
            Vector2 s2 = triangle.Sommet2;
            Vector2 s3 = triangle.Sommet3;

            if (collisionLigneRectangle(ref s1, ref s2, rectangle) ||
                collisionLigneRectangle(ref s2, ref s3, rectangle) ||
                collisionLigneRectangle(ref s3, ref s1, rectangle))
                return true;

            return false;
        }

        /// <summary>
        /// Collision Point <--> Rectangle 2D
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="rectangle">Rectangle</param>
        /// <returns>Est-ce que le Point est contenu (bords inclus) dans le Rectangle</returns>
        public static bool collisionPointRectangle(ref Vector2 point, RectanglePhysique rectangle)
        {
            return (point.X >= rectangle.Left && point.X <= rectangle.Right &&
                    point.Y >= rectangle.Top && point.Y <= rectangle.Bottom);
        }


        /// <summary>
        /// Collision Cercle <--> Rectangle
        /// </summary>
        /// <param name="cercle">Cercle</param>
        /// <param name="rectangle">Rectangle</param>
        /// <returns>Est-ce qu'il y a une collision entre le cercle et le rectangle</returns>
        public static bool collisionCercleRectangle(Cercle cercle, RectanglePhysique rectangle)
        {
            return cercle.Intersects(rectangle);
        }


        /// <summary>
        /// Collision Rectangle <--> Rectangle
        /// </summary>
        /// <param name="rectangle1">Rectangle 1</param>
        /// <param name="rectangle2">Rectangle 2</param>
        /// <returns>S'il y a une collision entre les deux rectangles</returns>
        public static bool collisionRectangleRectangle(RectanglePhysique rectangle1, RectanglePhysique rectangle2)
        {
            return rectangle1.Intersects(rectangle2);
        }


        /// <summary>
        /// Collision Cercle <--> Cercle
        /// </summary>
        /// <param name="cercle1">Cercle 1</param>
        /// <param name="cercle2">Cercle 2</param>
        /// <returns>S'il y a une collision entre les deux cercles</returns>
        public static bool collisionCercleCercle(Cercle cercle1, Cercle cercle2)
        {
            return cercle1.Intersects(cercle2);
        }


        /// <summary>
        /// Collision par pixels
        /// </summary>
        /// <param name="objet1">Objet 1</param>
        /// <param name="objet2">Objet 2</param>
        /// <returns>Est-ce qu'il y a une collision entre l'objet 1 et l'objet 2</returns>
        public static bool collisionPixels(ref Matrix Transformee1, Texture2D Texture1, ref Color[] TextureData1, ref Matrix Transformee2, Texture2D Texture2, ref Color[] TextureData2) {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = Transformee1 * Matrix.Invert(Transformee2);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < Texture1.Height; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < Texture1.Width; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < Texture2.Width &&
                        0 <= yB && yB < Texture2.Height)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = TextureData1[xA + yA * Texture1.Width];
                        Color colorB = TextureData2[xB + yB * Texture2.Width];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB.X = posInB.X + stepX.X;
                    posInB.Y = posInB.Y + stepX.Y;
                }

                // Move to the next row
                yPosInB.X = yPosInB.X + stepY.X;
                yPosInB.Y = yPosInB.Y + stepY.Y;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Transformation d'un rectangle
        /// </summary>
        /// <param name="transform">Matrice de transformation</param>
        /// <param name="rectangle">Rectangle à transformer</param>
        /// <example>Rotation => retourne le rectangle entourant le "rectangle" tourné</example>
        public static void rectangleTransforme(ref Matrix transform, RectanglePhysique rectangle)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            rectangle.X = (int)min.X;
            rectangle.Y = (int)min.Y;
            rectangle.Width = (int)(max.X - min.X);
            rectangle.Height = (int)(max.Y - min.Y);
        }


        //=====================================================================
        // Helpers
        //=====================================================================

        /// <summary>
        /// Détermine le sens d'une ligne
        /// </summary>
        /// <param name="pt1">Point 1 de la ligne</param>
        /// <param name="pt2">Point 2 de la ligne</param>
        /// <param name="pt3">Point 3 de la ligne</param>
        /// <returns>Sens de la ligne</returns>
        private static SensLigne getSensLigne(ref Vector2 pt1, ref Vector2 pt2, ref Vector2 pt3)
        {
            float tester =
                (pt2.X - pt1.X) * (pt3.Y - pt1.Y) -
                (pt3.X - pt1.X) * (pt2.Y - pt1.Y);

            if (tester > 0)
                return SensLigne.ContreSensMontre;
            
            if (tester < 0)
                return SensLigne.SensMontre;

            return SensLigne.Ligne;
        }


        /// <summary>
        /// Collision Point <--> Cercle
        /// </summary>
        /// <param name="point">Un point</param>
        /// <param name="cercle">Un cercle</param>
        /// <returns>vrai s'il y a une collision entre le point et le cercle</returns>
        public static bool collisionPointCercle(ref Vector3 point, Cercle cercle)
        {
            return cercle.Intersects(ref point);
        }
    }
}
