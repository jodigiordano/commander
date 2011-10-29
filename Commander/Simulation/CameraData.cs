namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CameraData
    {
        public float MaxZoomIn;
        public float MaxZoomOut;
        public float Zoom;
        public float ZoomPerc;
        public bool ZoomChanged;
        public bool ZoomChangedOverride;


        public CameraData()
        {
            MaxZoomIn = 0;
            MaxZoomOut = 0;
            ZoomPerc = 0;
            ZoomChanged = false;
            ZoomChangedOverride = false;
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
