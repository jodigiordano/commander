namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class ContextualMenusCollisions
    {
        public List<ContextualMenu> Menus;

        private List<List<PhysicalRectangle>> PossibleLayouts;


        public ContextualMenusCollisions()
        {
            Menus = new List<ContextualMenu>();
            PossibleLayouts = new List<List<PhysicalRectangle>>();
        }


        public void Sync()
        {
            if (Menus.Count == 0)
                return;

            PossibleLayouts = new List<List<PhysicalRectangle>>(); //to delete
            PossibleLayouts.Clear();

            // Find the possible layouts for a menu (always 4)
            foreach (var menu in Menus)
                PossibleLayouts.Add(menu.GetPossibleLayouts());

            PossibleLayout lessCollisions;

            if (Menus.Count == 1)
            {
                lessCollisions = FindBestLayout1(PossibleLayouts);
                Menus[0].Layout = lessCollisions.P1;
            }

            else if (Menus.Count == 2)
            {
                lessCollisions = FindBestLayout2(PossibleLayouts);
                Menus[0].Layout = lessCollisions.P1;
                Menus[1].Layout = lessCollisions.P2;
            }

            else if (Menus.Count == 3)
            {
                lessCollisions = FindBestLayout3(PossibleLayouts);
                Menus[0].Layout = lessCollisions.P1;
                Menus[1].Layout = lessCollisions.P2;
                Menus[2].Layout = lessCollisions.P3;
            }

            else if (Menus.Count == 4)
            {
                lessCollisions = FindBestLayout4(PossibleLayouts);
                Menus[0].Layout = lessCollisions.P1;
                Menus[1].Layout = lessCollisions.P2;
                Menus[2].Layout = lessCollisions.P3;
                Menus[3].Layout = lessCollisions.P4;
            }
        }


        private struct PossibleLayout
        {
            public int P1;
            public int P2;
            public int P3;
            public int P4;
            public int NbCollisions;
        }


        private PossibleLayout FindBestLayout1(List<List<PhysicalRectangle>> layouts)
        {
            PossibleLayout bestLayout = new PossibleLayout() { NbCollisions = int.MaxValue };

            for (int i = 0; i < layouts[0].Count; i++)
            {
                if (layouts[0][i] == null)
                    continue;

                bestLayout.P1 = i;
                break;
            }

            return bestLayout;
        }


        private PossibleLayout FindBestLayout2(List<List<PhysicalRectangle>> layouts)
        {
            PossibleLayout bestLayout = new PossibleLayout() { NbCollisions = int.MaxValue };

            for (int i = 0; i < layouts[0].Count; i++)
            {
                if (layouts[0][i] == null)
                    continue;

                for (int j = 0; j < layouts[1].Count; j++)
                {
                    if (layouts[1][j] == null)
                        continue;

                    int nbCollisions = 0;

                    if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[1][j]))
                        nbCollisions++;

                    if (nbCollisions < bestLayout.NbCollisions)
                    {
                        bestLayout.P1 = i;
                        bestLayout.P2 = j;
                        bestLayout.NbCollisions = nbCollisions;
                    }

                    if (bestLayout.NbCollisions == 0)
                        return bestLayout;
                }
            }

            return bestLayout;
        }


        private PossibleLayout FindBestLayout3(List<List<PhysicalRectangle>> layouts)
        {
            PossibleLayout bestLayout = new PossibleLayout() { NbCollisions = int.MaxValue };

            for (int i = 0; i < layouts[0].Count; i++)
            {
                if (layouts[0][i] == null)
                    continue;

                for (int j = 0; j < layouts[1].Count; j++)
                {
                    if (layouts[1][j] == null)
                        continue;

                    for (int k = 0; k < layouts[2].Count; k++)
                    {
                        if (layouts[2][k] == null)
                            continue;

                        int nbCollisions = 0;

                        if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[1][j]))
                            nbCollisions++;

                        if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[2][k]))
                            nbCollisions++;

                        if (Physics.RectangleRectangleCollision(layouts[1][j], layouts[2][k]))
                            nbCollisions++;

                        if (nbCollisions < bestLayout.NbCollisions)
                        {
                            bestLayout.P1 = i;
                            bestLayout.P2 = j;
                            bestLayout.P3 = k;
                            bestLayout.NbCollisions = nbCollisions;
                        }

                        if (bestLayout.NbCollisions == 0)
                            return bestLayout;
                    }
                }
            }

            return bestLayout;
        }


        private PossibleLayout FindBestLayout4(List<List<PhysicalRectangle>> layouts)
        {
            PossibleLayout bestLayout = new PossibleLayout() { NbCollisions = int.MaxValue };

            for (int i = 0; i < layouts[0].Count; i++)
            {
                if (layouts[0][i] == null)
                    continue;

                for (int j = 0; j < layouts[1].Count; j++)
                {
                    if (layouts[1][j] == null)
                        continue;

                    for (int k = 0; k < layouts[2].Count; k++)
                    {
                        if (layouts[2][k] == null)
                            continue;

                        for (int l = 0; l < layouts[3].Count; l++)
                        {
                            if (layouts[3][l] == null)
                                continue;

                            int nbCollisions = 0;

                            if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[1][j]))
                                nbCollisions++;

                            if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[2][k]))
                                nbCollisions++;

                            if (Physics.RectangleRectangleCollision(layouts[0][i], layouts[3][l]))
                                nbCollisions++;

                            if (Physics.RectangleRectangleCollision(layouts[1][j], layouts[2][k]))
                                nbCollisions++;

                            if (Physics.RectangleRectangleCollision(layouts[1][j], layouts[3][l]))
                                nbCollisions++;

                            if (Physics.RectangleRectangleCollision(layouts[2][k], layouts[3][l]))
                                nbCollisions++;

                            if (nbCollisions < bestLayout.NbCollisions)
                            {
                                bestLayout.P1 = i;
                                bestLayout.P2 = j;
                                bestLayout.P3 = k;
                                bestLayout.P4 = l;
                                bestLayout.NbCollisions = nbCollisions;
                            }

                            if (bestLayout.NbCollisions == 0)
                                return bestLayout;
                        }
                    }
                }
            }

            return bestLayout;
        }
    }
}
