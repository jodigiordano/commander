namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewHorizontalSlider
    {
        public int Value;
        public int Min;
        public int Max;
        public int Increment;

        private Scene Scene;
        private Image DecrementRep;
        private Image IncrementRep;
        private Text ValueText;
        private Vector3 Position;
        private Circle DecrementCircle;
        private Circle IncrementCircle;


        public NewHorizontalSlider(Scene scene, Vector3 position, int min, int max, int startingValue, int increment, float visualPriority)
        {
            Scene = scene;
            Position = position;

            Value = startingValue;
            Min = min;
            Max = max;
            Increment = increment;

            DecrementRep = new Image("Gauche", Position - new Vector3(50, 0, 0));
            DecrementRep.VisualPriority = visualPriority;

            IncrementRep = new Image("Droite", Position + new Vector3(50, 0, 0));
            IncrementRep.VisualPriority = visualPriority;

            ValueText = new Text(Value.ToString(), "Pixelite", Color.White, Position);
            ValueText.SizeX = 2;
            ValueText.VisualPriority = visualPriority;
            ValueText.Origin = ValueText.Center;

            DecrementCircle = new Circle(DecrementRep.Position, 16);
            IncrementCircle = new Circle(IncrementRep.Position, 16);
        }


        public void DoClick(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, DecrementCircle) && Value > Min)
                Value = Math.Max(Min, Value - Increment);
            else if (Physics.CircleCicleCollision(circle, IncrementCircle) && Value < Max)
                Value = Math.Min(Max, Value + Increment);
        }


        public void Draw()
        {
            ValueText.Data = Value.ToString();

            Scene.Add(DecrementRep);
            Scene.Add(IncrementRep);
            Scene.Add(ValueText);
        }
    }
}
