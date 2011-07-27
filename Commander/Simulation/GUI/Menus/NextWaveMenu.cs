namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NextWaveMenu
    {
        public Vector3 Position;
        public bool Visible;

        private Dictionary<EnemyType, EnemyDescriptor> EnnemisQtes;
        private Simulator Simulator;
        private Bubble Bubble;
        private Dictionary<EnemyType, Image> RepresentationsEnnemis;
        private Dictionary<EnemyType, Text> RepresentationsQtes;
        private Dictionary<EnemyType, Text> RepresentationsNiveaux;
        private float PrioriteAffichage;


        public NextWaveMenu(
            Simulator simulator,
            Dictionary<EnemyType, EnemyDescriptor> ennemisQtes,
            Vector3 positionInitiale,
            float prioriteAffichage)
        {
            Simulator = simulator;
            EnnemisQtes = ennemisQtes;
            Position = positionInitiale;
            PrioriteAffichage = prioriteAffichage;

            RepresentationsEnnemis = new Dictionary<EnemyType, Image>();
            RepresentationsQtes = new Dictionary<EnemyType, Text>();
            RepresentationsNiveaux = new Dictionary<EnemyType, Text>();

            foreach (var kvp in EnemiesFactory.ImagesEnemies)
            {
                Image im = new Image(kvp.Value)
                {
                    VisualPriority = this.PrioriteAffichage + 0.00001f,
                    SizeX = 4
                };

                RepresentationsEnnemis.Add(kvp.Key, im);

                Text t = new Text("Pixelite")
                {
                    VisualPriority = this.PrioriteAffichage,
                    SizeX = 2
                };

                RepresentationsQtes.Add(kvp.Key, t);

                t = new Text("Pixelite")
                {
                    VisualPriority = this.PrioriteAffichage,
                    SizeX = 1
                };

                RepresentationsNiveaux.Add(kvp.Key, t);
            }

            Bubble = new Bubble(Simulator.Scene, new PhysicalRectangle(), this.PrioriteAffichage + 0.0001f);
            Bubble.BlaPosition = 1;

            Visible = false;
        }


        public Color Color
        {
            set
            {
                Bubble.Color = value;
            }
        }


        public void Draw()
        {
            if (!Visible || EnnemisQtes.Count == 0)
                return;

            Bubble.Dimension.X = (int) this.Position.X;
            Bubble.Dimension.Y = (int) this.Position.Y;

            Bubble.Dimension.Width = 100;
            Bubble.Dimension.Height = 10;

            foreach (var ennemiQte in EnnemisQtes)
            {
                var image = RepresentationsEnnemis[ennemiQte.Key];
                var qty = RepresentationsQtes[ennemiQte.Key];
                var level = RepresentationsNiveaux[ennemiQte.Key];

                image.Position = this.Position + new Vector3
                (
                    25,
                    Bubble.Dimension.Height + image.AbsoluteSize.Y / 2.0f,
                    0
                );

                Bubble.Dimension.Height += (int) image.AbsoluteSize.Y + 10;

                level.Data = (ennemiQte.Value.LivesLevel).ToString();
                level.Origin = level.Center;
                level.Position = new Vector3(this.Position.X + 40, image.Position.Y, 0);

                qty.Data = ((int) ennemiQte.Value.CashValue).ToString();
                qty.Origin = qty.Center;
                qty.Position = new Vector3(this.Position.X + 80, image.Position.Y, 0);

                Simulator.Scene.Add(image);
                Simulator.Scene.Add(qty);
                Simulator.Scene.Add(level);
            }

            Bubble.Draw();
        }
    }
}
