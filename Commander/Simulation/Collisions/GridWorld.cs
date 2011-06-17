namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class GridWorld
    {
        private List<List<List<int>>> Grid;
        private RectanglePhysique Space;
        private Vector2 SpaceUpperLeft;
        private int CellSize;
        private int NbRows;
        private int NbColumns;


        public GridWorld(RectanglePhysique space, int cellSize)
        {
            this.Space = space;
            this.CellSize = cellSize;

            SpaceUpperLeft = new Vector2(Space.Left, Space.Top);

            NbRows = Space.Height / CellSize;
            NbColumns = Space.Width / CellSize;

            Grid = new List<List<List<int>>>();

            for (int i = 0; i < NbRows; i++)
            {
                Grid.Add(new List<List<int>>());

                for (int j = 0; j < NbColumns; j++)
                    Grid[i].Add(new List<int>());
            }
        }


        public void Add(int id, IObjetPhysique obj)
        {
            if (!Space.Includes(obj.Position))
                return;

            switch (obj.Shape)
            {
                case Shape.Rectangle:
                    AddRectangle(id, obj.Rectangle.Left, obj.Rectangle.Right, obj.Rectangle.Top, obj.Rectangle.Bottom);
                    break;
                case Shape.Circle:
                    AddRectangle(id, obj.Circle.Rectangle.Left, obj.Circle.Rectangle.Right, obj.Circle.Rectangle.Top, obj.Circle.Rectangle.Bottom);
                    break;
            }
        }

        private void AddRectangle(int id, float left, float right, float top, float bottom)
        {
            Vector2 upperLeft = new Vector2(left, top);
            Vector2 upperRight = new Vector2(right, top);
            Vector2 lowerLeft = new Vector2(left, bottom);
            Vector2 lowerRight = new Vector2(right, bottom);

            Vector2.Subtract(ref upperLeft, ref SpaceUpperLeft, out upperLeft);
            Vector2.Subtract(ref upperRight, ref SpaceUpperLeft, out upperRight);
            Vector2.Subtract(ref lowerLeft, ref SpaceUpperLeft, out lowerLeft);
            Vector2.Subtract(ref lowerRight, ref SpaceUpperLeft, out lowerRight);

            Vector2.Divide(ref upperLeft, CellSize, out upperLeft);
            Vector2.Divide(ref upperRight, CellSize, out upperRight);
            Vector2.Divide(ref lowerLeft, CellSize, out lowerLeft);
            Vector2.Divide(ref lowerRight, CellSize, out lowerRight);

            int x_1, y_1, x_2, y_2, x_3, y_3, x_4, y_4;
            x_1 = (int) upperLeft.X;
            y_1 = (int) upperLeft.Y;

            x_2 = (int) upperRight.X;
            y_2 = (int) upperRight.Y;

            x_3 = (int) lowerLeft.X;
            y_3 = (int) lowerLeft.Y;

            x_4 = (int) lowerRight.X;
            y_4 = (int) lowerRight.Y;

            if (upperLeft.X >= 0 && upperLeft.Y >= 0 &&
                upperLeft.X < NbColumns && upperLeft.Y < NbRows)
                Grid[y_1][x_1].Add(id);

            if (upperRight.X >= 0 && upperRight.Y >= 0 &&
                upperRight.X < NbColumns && upperRight.Y < NbRows && !(x_2 == x_1 && y_2 == y_1))
                Grid[y_2][x_2].Add(id);

            if (lowerLeft.X >= 0 && lowerLeft.Y >= 0 &&
                lowerLeft.X < NbColumns && lowerLeft.Y < NbRows && !(x_3 == x_1 && y_3 == y_1) && !(x_3 == x_2 && y_3 == y_2))
                Grid[y_3][x_3].Add(id);

            if (lowerRight.X >= 0 && lowerRight.Y >= 0 &&
                lowerRight.X < NbColumns && lowerRight.Y < NbRows && !(x_4 == x_3 && y_4 == y_3) && !(x_4 == x_2 && y_4 == y_2) && !(x_4 == x_1 && y_4 == y_1))
                Grid[y_4][x_4].Add(id);
        }


        public delegate bool IntegerHandler(int i);

        public void GetItems(RectanglePhysique rectangle, IntegerHandler callback)
        {
            Vector2 upperLeft = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref upperLeft, ref SpaceUpperLeft, out upperLeft);
            Vector2.Divide(ref upperLeft, CellSize, out upperLeft);

            int lowerXIndex = (int) upperLeft.X;
            int lowerYIndex = (int) upperLeft.Y;
            int upperXIndex = lowerXIndex + rectangle.Width / CellSize;
            int upperYIndex = lowerYIndex + rectangle.Width / CellSize;

            bool wantsNext = true;

            for (int i = lowerXIndex; i <= upperXIndex; i++)
                for (int j = lowerYIndex; j <= upperYIndex; j++)
                    if (i >= 0 && i < NbColumns && j >= 0 && j < NbRows)
                        for (int k = 0; k < Grid[j][i].Count; k++)
                        {
                            if (!wantsNext)
                                return;

                            wantsNext = callback(Grid[j][i][k]);
                        }
        }


        public void GetItems(Rectangle rectangle, IntegerHandler callback)
        {
            Vector2 upperLeft = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref upperLeft, ref SpaceUpperLeft, out upperLeft);
            Vector2.Divide(ref upperLeft, CellSize, out upperLeft);

            int lowerXIndex = (int) upperLeft.X;
            int lowerYIndex = (int) upperLeft.Y;
            int upperXIndex = lowerXIndex + rectangle.Width / CellSize;
            int upperYIndex = lowerYIndex + rectangle.Width / CellSize;

            bool wantsNext = true;

            for (int i = lowerXIndex; i <= upperXIndex; i++)
                for (int j = lowerYIndex; j <= upperYIndex; j++)
                    if (i >= 0 && i < NbColumns && j >= 0 && j < NbRows)
                        for (int k = 0; k < Grid[j][i].Count; k++)
                        {
                            if (!wantsNext)
                                return;

                            wantsNext = callback(Grid[j][i][k]);
                        }
        }


        public void GetItems(Line line, IntegerHandler callback)
        {
            Vector2 start = line.DebutV2;
            Vector2 direction = line.FinV2 - start;
            Vector2 multipliedDirection;
            float length = line.Longueur;

            direction.Normalize();

            bool wantsNext = true;

            for (int i = 0; i < length; i += CellSize / 2)
            {
                Vector2.Multiply(ref direction, i, out multipliedDirection);

                Vector2 position;
                Vector2.Add(ref start, ref multipliedDirection, out position);
                Vector2.Subtract(ref position, ref SpaceUpperLeft, out position);
                Vector2.Divide(ref position, CellSize, out position);

                int x_1 = (int) position.X;
                int y_1 = (int) position.Y;

                if (x_1 >= 0 && x_1 < NbColumns && y_1 >= 0 && y_1 < NbRows)
                    for (int k = 0; k < Grid[y_1][x_1].Count; k++)
                    {
                        if (!wantsNext)
                            return;

                        wantsNext = callback(Grid[y_1][x_1][k]);
                    }
            }
        }


        public IEnumerable<int> GetItems(RectanglePhysique rectangle)
        {
            Vector2 upperLeft = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref upperLeft, ref SpaceUpperLeft, out upperLeft);
            Vector2.Divide(ref upperLeft, CellSize, out upperLeft);

            int lowerXIndex = (int)upperLeft.X;
            int lowerYIndex = (int)upperLeft.Y;
            int upperXIndex = lowerXIndex + rectangle.Width / CellSize;
            int upperYIndex = lowerYIndex + rectangle.Width / CellSize;

            for (int i = lowerXIndex; i <= upperXIndex; i++)
                for (int j = lowerYIndex; j <= upperYIndex; j++)
                    if (i >= 0 && i < NbColumns && j >= 0 && j < NbRows)
                        for (int k = 0; k < Grid[j][i].Count; k++)
                            yield return Grid[j][i][k];
        }


        public IEnumerable<int> GetItems(Rectangle rectangle)
        {
            Vector2 upperLeft = new Vector2(rectangle.Left, rectangle.Top);
            Vector2.Subtract(ref upperLeft, ref SpaceUpperLeft, out upperLeft);
            Vector2.Divide(ref upperLeft, CellSize, out upperLeft);

            int lowerXIndex = (int)upperLeft.X;
            int lowerYIndex = (int)upperLeft.Y;
            int upperXIndex = lowerXIndex + rectangle.Width / CellSize;
            int upperYIndex = lowerYIndex + rectangle.Width / CellSize;

            for (int i = lowerXIndex; i <= upperXIndex; i++)
                for (int j = lowerYIndex; j <= upperYIndex; j++)
                    if (i >= 0 && i < NbColumns && j >= 0 && j < NbRows)
                        for (int k = 0; k < Grid[j][i].Count; k++)
                            yield return Grid[j][i][k];
        }


        public IEnumerable<int> GetItems(Line line)
        {
            Vector2 start = line.DebutV2;
            Vector2 direction = line.FinV2 - start;
            Vector2 multipliedDirection;
            float length = line.Longueur;

            direction.Normalize();

            for (int i = 0; i < length; i+= CellSize / 2)
            {
                Vector2.Multiply(ref direction, i, out multipliedDirection);

                Vector2 position;
                Vector2.Add(ref start, ref multipliedDirection, out position);
                Vector2.Subtract(ref position, ref SpaceUpperLeft, out position);
                Vector2.Divide(ref position, CellSize, out position);

                int x_1 = (int)position.X;
                int y_1 = (int)position.Y;

                if (x_1 >= 0 && x_1 < NbColumns && y_1 >= 0 && y_1 < NbRows)
                        for (int k = 0; k < Grid[y_1][x_1].Count; k++)
                            yield return Grid[y_1][x_1][k];
            }
        }


        public void Clear()
        {
            for (int i = 0; i < NbRows; i++)
                for (int j = 0; j < NbColumns; j++)
                    Grid[i][j].Clear();
        }
    }
}
