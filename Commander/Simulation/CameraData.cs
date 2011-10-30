namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CameraData
    {
        public float MaxZoomIn;
        public float MaxZoomOut;
        public float MaxDelta;
        public float Zoom;
        public float ZoomPerc;
        public bool ZoomChanged;
        public bool ZoomChangedOverride;
        public bool ManualZoom;


        public CameraData()
        {
            MaxZoomIn = 0;
            MaxZoomOut = 0;
            ZoomPerc = 0;
            ZoomChanged = false;
            ZoomChangedOverride = false;
            ManualZoom = false;
        }


        public void Update()
        {
            float currentZoom = ZoomPerc;
            float newZoom = Zoom / MaxZoomIn;

            float delta = newZoom - currentZoom;

            ZoomChanged = ZoomChangedOverride || delta != 0;
            ZoomChangedOverride = false;

            ZoomPerc = MathHelper.Clamp(ZoomPerc + delta, 0, 1);
        }
    }
}
