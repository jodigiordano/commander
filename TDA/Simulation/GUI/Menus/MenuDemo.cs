namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuDemo : MenuAbstract
    {
        public CorpsCeleste CelestialBody;
        public DescripteurScenario Scenario;
        
        private float PrioriteAffichage;


        public MenuDemo(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            PrioriteAffichage = prioriteAffichage;
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (CelestialBody == null)
                    return Vector2.Zero;

                return new Vector2(190, 65);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position - new Vector3(0, CelestialBody.Cercle.Rayon / 4, 0);
            }
        }


        public override void Draw()
        {
            if (CelestialBody == null || Scenario == null)
                return;

            base.Draw();

            //Bulle.Draw(null);
        }
    }
}
