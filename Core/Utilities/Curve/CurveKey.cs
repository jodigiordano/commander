namespace EphemereGames.Core.Utilities
{
    public class CurveKey
    {
        public CurveContinuity Continuity;
        public float Position;
        public float TangentIn;
        public float TangentOut;
        public float Value;


        public CurveKey()
            : this(0, 0, 0, 0, CurveContinuity.Smooth) { }


        public CurveKey(float position, float value)
            : this(position, value, 0, 0, CurveContinuity.Smooth)
        {

        }


        public CurveKey(float position, float value, float tangentIn, float tangentOut)
            : this(position, value, tangentIn, tangentOut, CurveContinuity.Smooth)
        {

        }


        public CurveKey(float position, float value, float tangentIn, float tangentOut, CurveContinuity continuity)
        {
            this.Position = position;
            this.Value = value;
            this.TangentIn = tangentIn;
            this.TangentOut = tangentOut;
            this.Continuity = continuity;
        }
    }
}
