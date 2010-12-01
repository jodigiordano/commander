namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Core.Physique;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class GridWorld
    {
        private List<int>[,] Grille;
        private RectanglePhysique Terrain;
        private Vector2 TerrainCoinHautGauche;
        private int TailleCellule;
        private int NbLignes, NbColonnes;
        private Scene Scene;

        public GridWorld(Scene Scene, RectanglePhysique terrain, int tailleCellule)
        {
            this.Terrain = terrain;
            this.TailleCellule = tailleCellule;
            this.Scene = Scene;

            TerrainCoinHautGauche = new Vector2(Terrain.Left, Terrain.Top);

            NbLignes = terrain.Height / TailleCellule;
            NbColonnes = terrain.Width / TailleCellule;

            Grille = new List<int>[NbLignes, NbColonnes];

            for (int i = 0; i < NbLignes; i++)
                for (int j = 0; j < NbColonnes; j++)
                    Grille[i, j] = new List<int>();
        }

        public void ajouter(int indice, Ennemi objet)
        {
            if (!Terrain.Includes(objet.Position))
                return;

            switch (objet.Forme)
            {
                case Forme.Rectangle:
                    Vector2 coinHautGauche = new Vector2(objet.Rectangle.Left, objet.Rectangle.Top);
                    Vector2 coinHautDroite = new Vector2(objet.Rectangle.Right, objet.Rectangle.Top);
                    Vector2 coinBasGauche = new Vector2(objet.Rectangle.Left, objet.Rectangle.Bottom);
                    Vector2 coinBasDroite = new Vector2(objet.Rectangle.Right, objet.Rectangle.Bottom);

                    Vector2.Subtract(ref coinHautGauche, ref TerrainCoinHautGauche, out coinHautGauche);
                    Vector2.Subtract(ref coinHautDroite, ref TerrainCoinHautGauche, out coinHautDroite);
                    Vector2.Subtract(ref coinBasGauche, ref TerrainCoinHautGauche, out coinBasGauche);
                    Vector2.Subtract(ref coinBasDroite, ref TerrainCoinHautGauche, out coinBasDroite);

                    Vector2.Divide(ref coinHautGauche, TailleCellule, out coinHautGauche);
                    Vector2.Divide(ref coinHautDroite, TailleCellule, out coinHautDroite);
                    Vector2.Divide(ref coinBasGauche, TailleCellule, out coinBasGauche);
                    Vector2.Divide(ref coinBasDroite, TailleCellule, out coinBasDroite);

                    int x_1, y_1, x_2, y_2, x_3, y_3, x_4, y_4;
                    x_1 = (int)coinHautGauche.X;
                    y_1 = (int)coinHautGauche.Y;

                    x_2 = (int)coinHautDroite.X;
                    y_2 = (int)coinHautDroite.Y;

                    x_3 = (int)coinBasGauche.X;
                    y_3 = (int)coinBasGauche.Y;

                    x_4 = (int)coinBasDroite.X;
                    y_4 = (int)coinBasDroite.Y;

                    if (coinHautGauche.X >= 0 && coinHautGauche.Y >= 0 &&
                        coinHautGauche.X < NbColonnes && coinHautGauche.Y < NbLignes)
                        Grille[y_1, x_1].Add(indice);

                    if (coinHautDroite.X >= 0 && coinHautDroite.Y >= 0 &&
                        coinHautDroite.X < NbColonnes && coinHautDroite.Y < NbLignes && !(x_2 == x_1 && y_2 == y_1))
                        Grille[y_2, x_2].Add(indice);

                    if (coinBasGauche.X >= 0 && coinBasGauche.Y >= 0 &&
                        coinBasGauche.X < NbColonnes && coinBasGauche.Y < NbLignes && !(x_3 == x_1 && y_3 == y_1) && !(x_3 == x_2 && y_3 == y_2))
                        Grille[y_3, x_3].Add(indice);

                    if (coinBasDroite.X >= 0 && coinBasDroite.Y >= 0 &&
                        coinBasDroite.X < NbColonnes && coinBasDroite.Y < NbLignes && !(x_4 == x_3 && y_4 == y_3) && !(x_4 == x_2 && y_4 == y_2) && !(x_4 == x_1 && y_4 == y_1))
                        Grille[y_4, x_4].Add(indice);
                    break;
            }
        }

        public IEnumerable<int> getItems(RectanglePhysique rectangle /*, ref List<int> resultats*/)
        {
            Vector2 coinHautGauche = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref coinHautGauche, ref TerrainCoinHautGauche, out coinHautGauche);
            Vector2.Divide(ref coinHautGauche, TailleCellule, out coinHautGauche);

            int indiceXInferieur = (int)coinHautGauche.X;
            int indiceYInferieur = (int)coinHautGauche.Y;
            int indiceXSuperieur = indiceXInferieur + rectangle.Width / TailleCellule;
            int indiceYSuperieur = indiceYInferieur + rectangle.Width / TailleCellule;

            //Scene.ajouterScenable(new RectangleVisuel(rectangle.RectanglePrimitif, Color.Blue));

            for (int i = indiceXInferieur; i <= indiceXSuperieur; i++)
                for (int j = indiceYInferieur; j <= indiceYSuperieur; j++)
                    if (i >= 0 && i < NbColonnes && j >= 0 && j < NbLignes)
                        for (int k = 0; k < Grille[j, i].Count; k++)
                            yield return Grille[j, i][k];
                        //resultats.AddRange(Grille[j, i]);
        }

        public IEnumerable<int> getItems(Rectangle rectangle/*, ref List<int> resultats*/)
        {
            Vector2 coinHautGauche = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref coinHautGauche, ref TerrainCoinHautGauche, out coinHautGauche);
            Vector2.Divide(ref coinHautGauche, TailleCellule, out coinHautGauche);

            int indiceXInferieur = (int)coinHautGauche.X;
            int indiceYInferieur = (int)coinHautGauche.Y;
            int indiceXSuperieur = indiceXInferieur + rectangle.Width / TailleCellule;
            int indiceYSuperieur = indiceYInferieur + rectangle.Width / TailleCellule;

            //Scene.ajouterScenable(new RectangleVisuel(rectangle, Color.Blue));

            for (int i = indiceXInferieur; i <= indiceXSuperieur; i++)
                for (int j = indiceYInferieur; j <= indiceYSuperieur; j++)
                    if (i >= 0 && i < NbColonnes && j >= 0 && j < NbLignes)
                        for (int k = 0; k < Grille[j, i].Count; k++)
                            yield return Grille[j, i][k];
                        //resultats.AddRange(Grille[j, i]);
        }


        public IEnumerable<int> getItems(Ligne ligne/*, ref List<int> resultats*/)
        {
            Vector2 debut = ligne.DebutV2;
            Vector2 direction = ligne.FinV2 - debut;
            Vector2 directionMultipliee;
            float Longueur = ligne.Longueur;

            direction.Normalize();

            int x_precedent = int.MaxValue;
            int y_precedent = int.MaxValue;

            for (int i = 0; i < Longueur; i+= TailleCellule / 2)
            {
                Vector2.Multiply(ref direction, i, out directionMultipliee);

                Vector2 position;
                Vector2.Add(ref debut, ref directionMultipliee, out position);
                Vector2.Subtract(ref position, ref TerrainCoinHautGauche, out position);
                Vector2.Divide(ref position, TailleCellule, out position);

                int x_1 = (int)position.X;
                int y_1 = (int)position.Y;

                if (!(x_1 == x_precedent && y_1 == y_precedent) &&
                    x_1 >= 0 && x_1 < NbColonnes && y_1 >= 0 && y_1 < NbLignes)
                        for (int k = 0; k < Grille[y_1, x_1].Count; k++)
                            yield return Grille[y_1, x_1][k];
                    //resultats.AddRange(Grille[y_1, x_1]);
            }
        }


        public void vider()
        {
            for (int i = 0; i < NbLignes; i++)
                for (int j = 0; j < NbColonnes; j++)
                    Grille[i, j].Clear();
        }

        public void Draw()
        {
            //for (int i = 0; i < NbLignes; i++)
            //    Scene.ajouterScenable(new LigneVisuel(TerrainCoinHautGauche + new Vector2(0, i * TailleCellule), new Vector2(Terrain.Right, Terrain.Top) + new Vector2(0, i * TailleCellule), Color.Azure));

            //for (int i = 0; i < NbColonnes; i++)
            //    Scene.ajouterScenable(new LigneVisuel(TerrainCoinHautGauche + new Vector2(i * TailleCellule, 0), new Vector2(Terrain.Left, Terrain.Bottom) + new Vector2(i * TailleCellule, 0), Color.Azure));

            //for (int i = 0; i < NbLignes; i++)
            //    for (int j = 0; j < NbColonnes; j++)
            //        for (int k = 0; k < Grille[i, j].Count; k++)
            //        {
            //            Ennemi objet = Grille[i, j][k];

            //            Rectangle r = objet.Rectangle.RectanglePrimitif;
            //            r.X = (int) (TerrainCoinHautGauche.X + j * TailleCellule);
            //            r.Y = (int)(TerrainCoinHautGauche.Y + i * TailleCellule);

            //            Scene.ajouterScenable(new RectangleVisuel(r, Color.Red));
            //        }
        }
    }
}
