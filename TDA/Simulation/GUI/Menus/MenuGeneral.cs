namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class MenuGeneral
    {
        public int Score;
        public int Cash;
        public int RemainingWaves;
        public double TimeNextWave;

        private Simulation Simulation;
        private IVisible WidgetCash;
        private IVisible WidgetRemainingWaves;
        private Vector3 Position;
        public MenuProchaineVague MenuNextWave;
        public Sablier SandGlass;
        public Cursor Cursor;

        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionNextWave
        {
            set
            {
                MenuNextWave = new MenuProchaineVague(Simulation, value, this.Position - new Vector3(150, 30, 0), Preferences.PrioriteGUIPanneauGeneral + 0.049f);
            }
        }

        public MenuGeneral(Simulation simulation, Vector3 position)
        {
            this.Simulation = simulation;
            this.Position = position;

            this.SandGlass = new Sablier(simulation.Main, simulation.Scene, 50000, this.Position, Preferences.PrioriteGUIPanneauGeneral + 0.05f);
            this.Cash = 0;


            WidgetCash = new IVisible
            (
                Cash + "M$",
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, 0, 0)
            );
            WidgetCash.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetCash.Taille = 3;

            WidgetRemainingWaves = new IVisible
            (
                RemainingWaves.ToString(),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, -40, 0)
            );
            WidgetRemainingWaves.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetRemainingWaves.Taille = 3;
        }


        public void Update(GameTime gameTime)
        {
            this.SandGlass.TempsRestant = this.TimeNextWave;
            this.SandGlass.Update(gameTime);
        }


        public void Draw()
        {
            WidgetCash.Texte = Cash + "M$";
            WidgetRemainingWaves.Texte = (RemainingWaves == -1) ? "Inf." : RemainingWaves.ToString();

            Simulation.Scene.ajouterScenable(WidgetCash);
            Simulation.Scene.ajouterScenable(WidgetRemainingWaves);

            this.SandGlass.Draw(null);
            this.MenuNextWave.Draw(null);
        }
    }
}
