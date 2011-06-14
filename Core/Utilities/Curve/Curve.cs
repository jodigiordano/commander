namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;


    public class Curve
    {
        public List<CurveKey> Keys;
        public CurveLoopType PostLoop;
        public CurveLoopType PreLoop;


        public Curve()
        {
            this.Keys = new List<CurveKey>();
        }


        public void ComputeTangents(CurveTangent tangentType)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                CurveKey k = Keys[i];

                if (i == 0 || i == Keys.Count - 1)
                    continue;

                var before = Keys[i - 1];
                var after = Keys[i + 1];

                k.TangentIn = ((after.Value - before.Value) * ((k.Position - before.Position) / (after.Position - before.Position)));
                k.TangentOut = ((after.Value - before.Value) * ((after.Position - k.Position) / (after.Position - before.Position)));
            }
        }


        public float Evaluate(float position)
        {
            CurveKey first = Keys[0];
            CurveKey last = Keys[Keys.Count - 1];

            if (position <= first.Position)
            {
                switch (this.PreLoop)
                {
                    case CurveLoopType.Constant:
                        return first.Value;

                    case CurveLoopType.Linear:
                        return first.Value - first.TangentIn * (first.Position - position);

                    case CurveLoopType.Cycle:
                        int cycle = GetNumberOfCycle(position);
                        float virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);

                    case CurveLoopType.CycleOffset:
                        cycle = GetNumberOfCycle(position);
                        virtualPos = position - (cycle * (last.Position - first.Position));
                        return (GetCurvePosition(virtualPos) + cycle * (last.Value - first.Value));

                    case CurveLoopType.Oscillate:
                        cycle = GetNumberOfCycle(position);
                        if (0 == cycle % 2f)
                            virtualPos = position - (cycle * (last.Position - first.Position));
                        else
                            virtualPos = last.Position - position + first.Position + (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);
                }
            }

            else if (position >= last.Position)
            {
                int cycle;
                switch (this.PostLoop)
                {
                    case CurveLoopType.Constant:
                        return last.Value;

                    case CurveLoopType.Linear:
                        return last.Value + first.TangentOut * (position - last.Position);

                    case CurveLoopType.Cycle:
                        cycle = GetNumberOfCycle(position);
                        float virtualPos = position - (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);

                    case CurveLoopType.CycleOffset:
                        cycle = GetNumberOfCycle(position);
                        virtualPos = position - (cycle * (last.Position - first.Position));
                        return (GetCurvePosition(virtualPos) + cycle * (last.Value - first.Value));

                    case CurveLoopType.Oscillate:
                        cycle = GetNumberOfCycle(position);
                        virtualPos = position - (cycle * (last.Position - first.Position));
                        if (0 == cycle % 2f)//if pair
                            virtualPos = position - (cycle * (last.Position - first.Position));
                        else
                            virtualPos = last.Position - position + first.Position + (cycle * (last.Position - first.Position));
                        return GetCurvePosition(virtualPos);
                }
            }

            return GetCurvePosition(position);
        }


        private int GetNumberOfCycle(float position)
        {
            float cycle = (position - Keys[0].Position) / (Keys[Keys.Count - 1].Position - Keys[0].Position);
            if (cycle < 0f)
                cycle--;
            return (int) cycle;
        }


        private float GetCurvePosition(float position)
        {
            int j = GetAssociatedKey(position);
            CurveKey prev = Keys[j - 1];
            CurveKey next = Keys[j];

            float t = (position - prev.Position) / (next.Position - prev.Position);
            float ts = t * t;
            float tss = ts * t;

            return (2 * tss - 3 * ts + 1f) * prev.Value + (tss - 2 * ts + t) * prev.TangentOut + (3 * ts - 2 * tss) * next.Value + (tss - ts) * next.TangentIn;
        }


        private int GetAssociatedKey(float position)
        {
            int start = 0;
            int end = Keys.Count - 1;
            int middle = 0;

            while (start < end)
            {
                middle = start + (end - start) / 2;

                if (Keys[middle].Position > position)
                    end = middle;
                else if (Keys[middle].Position <= position)
                    start = middle + 1;
            }

            return start;
        }
    }
}
