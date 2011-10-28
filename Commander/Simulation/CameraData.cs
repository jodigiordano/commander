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


        public CameraData()
        {
            MaxZoomIn = 0;
            MaxZoomOut = 0;
            ZoomPerc = 0;
            ZoomChanged = false;
        }


        public void Update()
        {
            float currentZoom = ZoomPerc;
            float newZoom = Zoom / MaxZoomIn;

            float delta = newZoom - currentZoom;

            ZoomChanged = delta != 0;

            ZoomPerc = MathHelper.Clamp(ZoomPerc + delta, 0, 1);
        }
    }
}
