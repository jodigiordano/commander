namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EnemiesWidget : PanelWidget
    {
        public Enemy SelectedEnemy;
        public int SelectedCount;

        private Dictionary<Enemy, ImageCheckBox> CheckBoxes;
        private ImageCheckBox FirstCheckBox;
        private ImageCheckBox LastCheckBox;
        private List<Enemy> Enemies;

        private float DistanceBetweenTwoChoices;
        private int Width;
        private int NbColumns;

        private Vector3 position;
        private double visualPriority;


        public EnemiesWidget(List<Enemy> enemies, int width, int nbColumns)
        {
            Enemies = enemies;
            CheckBoxes = new Dictionary<Enemy, ImageCheckBox>();

            foreach (var e in enemies)
                CheckBoxes.Add(e, new ImageCheckBox(e.Name));

            FirstCheckBox = CheckBoxes[Enemies[0]];
            LastCheckBox = CheckBoxes[Enemies[Enemies.Count - 1]];

            Width = width;
            NbColumns = nbColumns;
            DistanceBetweenTwoChoices = 20;
            SelectedCount = 0;
        }


        public override Core.Visual.Scene Scene
        {
            get { return base.Scene; }
            set
            {
                base.Scene = value;

                foreach (var w in CheckBoxes.Values)
                    w.Scene = value;
            }
        }


        public override double VisualPriority
        {
            get
            {
                return visualPriority;
            }

            set
            {
                visualPriority = value;

                foreach (var w in CheckBoxes.Values)
                    w.VisualPriority = value;
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

                Vector3 upperLeft = value;
                float initialX = upperLeft.X;
                float distanceColumn = Width / NbColumns;

                int columnCounter = 0;

                foreach (var w in CheckBoxes.Values)
                {
                    if (columnCounter != 0)
                        upperLeft.X += distanceColumn;

                    w.Position = upperLeft;

                    columnCounter++;

                    if (columnCounter >= NbColumns)
                        columnCounter = 0;

                    if (columnCounter == 0)
                    {
                        upperLeft.X = initialX;
                        upperLeft.Y += w.Dimension.Y + DistanceBetweenTwoChoices;
                    }
                }
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return
                    LastCheckBox.Position +
                    LastCheckBox.Dimension -
                    FirstCheckBox.Position;
            }

            set { }
        }


        protected override bool Click(Core.Physics.Circle circle)
        {
            foreach (var w in CheckBoxes)
                if (w.Value.DoClick(circle))
                {
                    SelectedCount += w.Value.Checked ? 1 : -1;
                    SelectedEnemy = w.Key;
                    return true;
                }

            return false;
        }


        public override void Draw()
        {
            foreach (var w in CheckBoxes.Values)
                w.Draw();
        }


        public override void Fade(int from, int to, double length)
        {
            foreach (var w in CheckBoxes.Values)
                w.Fade(from, to, length);
        }


        public List<EnemyType> GetEnemies()
        {
            List<EnemyType> enemies = new List<EnemyType>();

            foreach (var e in CheckBoxes)
                if (e.Value.Checked)
                    enemies.Add(e.Key.Type);

            return enemies;
        }
    }
}
