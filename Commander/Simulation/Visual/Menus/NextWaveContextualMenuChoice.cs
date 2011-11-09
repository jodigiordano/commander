namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NextWaveContextualMenuChoice : ContextualMenuChoice
    {
        private Dictionary<EnemyType, Image> EnemiesImages;
        private List<Image> Enemies;
        private ColoredText Quantity;

        private int DistanceEnemiesX;
        private int DistanceQuantityX;


        public NextWaveContextualMenuChoice(string name)
            : base(name)
        {
            EnemiesImages = new Dictionary<EnemyType, Image>();

            foreach (var kvp in EnemiesFactory.ImagesEnemies)
            {
                Image im = new Image(kvp.Value)
                {
                    Origin = Vector2.Zero,
                    SizeX = 4
                };

                EnemiesImages.Add(kvp.Key, im);
            }

            Enemies = new List<Image>();
            Quantity = new ColoredText(new List<string>() {"Next wave: ", ""}, new Color[] { Color.White, Color.White }, @"Pixelite", Vector3.Zero) { SizeX = 2 };

            DistanceEnemiesX = 10;
            DistanceQuantityX = 10;
        }


        public WaveDescriptor Composition
        {
            set
            {
                Enemies.Clear();

                Quantity.SetData(1, value.Quantity + " ");

                foreach (var enemy in value.Enemies)
                {
                    var image = EnemiesImages[enemy];

                    Enemies.Add(image);
                }

                NotifyDataChanged();
            }
        }


        public override Vector3 Position
        {
            set
            {
                Quantity.Position = value;

                for (int i = 0; i < Enemies.Count; i++)
                {
                    var e = Enemies[i];

                    e.Position = Quantity.Position + new Vector3((Quantity.AbsoluteSize.X + DistanceQuantityX) + (i * e.AbsoluteSize.X) + (i * DistanceEnemiesX), 0, 0);
                }
            }
        }


        public Color Color
        {
            set
            {
                Quantity.SetColor(1, value);
            }
        }


        public override Vector2 Size
        {
            get
            {
                return Enemies.Count == 0 ?
                    Vector2.Zero :
                    new Vector2(
                        Quantity.Position.X + Quantity.AbsoluteSize.X - Enemies[0].Position.X,
                        Enemies[0].AbsoluteSize.Y);
            }
        }


        public override double VisualPriority
        {
            set
            {
                foreach (var e in Enemies)
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


        public override void Fade(FadeColorEffect effect)
        {
            foreach (var e in Enemies)
                Scene.VisualEffects.Add(e, effect);

            Scene.VisualEffects.Add(Quantity, effect);
        }
    }
}
