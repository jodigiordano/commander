namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NextWaveWidget : PanelWidget
    {
        private Dictionary<EnemyType, Image> EnemiesImages;
        private List<Image> Enemies;
        private Text Quantity;

        private int DistanceEnemiesX;
        private int DistanceQuantityX;

        private byte alpha;
        private Vector3 position;
        private PhysicalRectangle Rectangle;


        public NextWaveWidget(float size)
        {
            EnemiesImages = new Dictionary<EnemyType, Image>();

            foreach (var kvp in EnemiesFactory.ImagesEnemies)
            {
                Image im = new Image(kvp.Value)
                {
                    Origin = Vector2.Zero,
                    SizeX = size
                };

                EnemiesImages.Add(kvp.Key, im);
            }

            Enemies = new List<Image>();
            Quantity = new Text(@"Pixelite") { SizeX = size - 1 };

            DistanceEnemiesX = 10;
            DistanceQuantityX = 10;

            Rectangle = new PhysicalRectangle();
        }


        public WaveDescriptor Composition
        {
            set
            {
                Enemies.Clear();

                Quantity.Data = value.Enemies.Count <= 0 ? "" : value.Quantity.ToString();

                foreach (var enemy in value.Enemies)
                {
                    var image = EnemiesImages[enemy];

                    Enemies.Add(image);
                }

                Position = position;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;

                if (Enemies.Count <= 0)
                    return;

                for (int i = 0; i < Enemies.Count; i++)
                {
                    var e = Enemies[i];

                    e.Position = position + new Vector3((i * e.AbsoluteSize.X) + (i * DistanceEnemiesX), 5, 0);
                }

                var last = Enemies[Enemies.Count - 1];

                // Position after the last enemy and center text verticaly
                Quantity.Position = last.Position + new Vector3(last.AbsoluteSize.X + DistanceQuantityX, last.AbsoluteSize.Y / 2 - Quantity.AbsoluteSize.Y / 2, 0);
            }
        }


        public override double VisualPriority
        {
            get
            {
                return Quantity.Alpha;
            }

            set
            {
                foreach (var e in EnemiesImages.Values)
                    e.VisualPriority = value;

                Quantity.VisualPriority = value;
            }
        }


        public override void Draw()
        {
            foreach (var e in Enemies)
                Scene.Add(e);

            Scene.Add(Quantity);
        }


        public override Vector3 Dimension
        {
            get
            {
                return Enemies.Count == 0 ?
                    Vector3.Zero :
                    new Vector3(
                        (Quantity.Position.X + Quantity.AbsoluteSize.X) - Enemies[0].Position.X,
                        Enemies[0].AbsoluteSize.Y, 0);
            }

            set
            {
                throw new System.NotImplementedException();
            }
        }


        public override byte Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;

                foreach (var e in Enemies)
                    e.Alpha = value;

                Quantity.Alpha = value;
            }
        }


        protected override bool Click(Core.Physics.Circle circle)
        {
            return false;
        }


        protected override bool Hover(Circle circle)
        {
            SyncRectangle();

            return Physics.CircleRectangleCollision(circle, Rectangle);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = Core.Visual.VisualEffects.Fade(from, to, 0, length);

            foreach (var e in Enemies)
                Scene.VisualEffects.Add(e, effect);

            Scene.VisualEffects.Add(Quantity, effect);
        }


        private void SyncRectangle()
        {
            var dimension = Dimension;

            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;
            Rectangle.Width = (int) dimension.X;
            Rectangle.Height = (int) dimension.Y;
        }
    }
}
