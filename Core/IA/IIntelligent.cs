using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bnoerj.AI.Steering;

namespace EndOfCivilizations
{
    public interface IIntelligent
    {
        Vector3 Position     { get; set; }
        Vector3 Steering     { get; set; }
        SimpleVehicle IA     { get; set; }

        void Update(GameTime gameTime);
    }
}
