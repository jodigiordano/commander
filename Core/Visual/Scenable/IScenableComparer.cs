namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;


    class IScenableComparer : IComparer<IScenable>
    {
        private static IScenableComparer instance;
        public static IScenableComparer Default
        {
            get
            {
                if (instance == null)
                    instance = new IScenableComparer();
                return instance;
            }
        }


        public int Compare(IScenable e1, IScenable e2)
        {
            if (e1.VisualPriority < e2.VisualPriority)
                return 1;

            if (e1.VisualPriority > e2.VisualPriority)
                return -1;

            if (e1.Id > e2.Id)
                return 1;

            if (e1.Id < e2.Id)
                return -1;

            return 0;
        }
    }
}
