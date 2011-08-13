namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;

    
    class ContextualMenuLayout
    {
        public int Id;
        public PhysicalRectangle Rectangle;


        public int BubbleTailId;
        public Vector2 BubblePositionCorrection;
        public bool SubstractSizeX;
        public bool SubstractSizeY;


        public ContextualMenuLayout(int id, int bubbleTailId, Vector2 bubblePositionCorrection)
        {
            Id = id;
            Rectangle = new PhysicalRectangle();
            BubbleTailId = bubbleTailId;
            BubblePositionCorrection = bubblePositionCorrection;
            SubstractSizeX = false;
            SubstractSizeY = false;
        }
    }
}
