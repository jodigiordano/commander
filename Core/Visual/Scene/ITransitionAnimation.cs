namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public interface ITransitionAnimation
    {
        Scene Scene { set; }
        bool IsFinished { get; }

        void Initialize(TransitionType type);
        void Update(GameTime gameTime);
        void Draw();
    }
}
