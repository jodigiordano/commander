namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ColoredTextContextualMenuChoice : ContextualMenuChoice
    {
        private ColoredText Label;


        public ColoredTextContextualMenuChoice(string name, ColoredText label)
            : base(name)
        {
            Label = label;
        }


        public void SetData(List<string> data)
        {
            bool dataChanged = Label.Data.Count != data.Count;
            
            if (!dataChanged)
                for (int i = 0; i < Label.Data.Count; i ++)
                    if (Label.Data[i] != data[i])
                    {
                        dataChanged = true;
                        break;
                    }

            Label.Data = data;

            if (dataChanged)
                NotifyDataChanged();
        }


        public void SetColor(int index, Color color)
        {
            Label.SetColor(index, color);
        }


        public override Vector3 Position
        {
            set { Label.Position = value; }
        }


        public override Vector2 Size
        {
            get { return Label.AbsoluteSize; }
        }


        public override double VisualPriority
        {
            set { Label.VisualPriority = value; }
        }


        public override void Draw()
        {
            Scene.Add(Label);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Label, effect);
        }
    }
}
