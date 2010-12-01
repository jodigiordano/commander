namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class MenuGeneral : DrawableGameComponent
    {
        public int Pointage;
        public int ReserveUnites;
        public int VaguesRestantes;
        public double TempsProchaineVague;

        private Simulation Simulation;
        private Scene Scene;
        private IVisible WidgetReserveUnites;
        private IVisible WidgetVaguesRestantes;
        private Vector3 Position;
        private MenuProchaineVague MenuProchaineVague;
        public Sablier Sablier;
        public Curseur Curseur;

        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionProchaineVague
        {
            set
            {
                MenuProchaineVague = new MenuProchaineVague(Simulation, value, this.Position - new Vector3(150, 30, 0), Preferences.PrioriteGUIPanneauGeneral + 0.049f);
            }
        }

        public MenuGeneral(Simulation simulation, Vector3 position)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Scene = simulation.Scene;
            this.Position = position;

            this.Sablier = new Sablier(simulation.Main, simulation.Scene, 50000, this.Position, Preferences.PrioriteGUIPanneauGeneral + 0.05f);
            this.ReserveUnites = 0;


            WidgetReserveUnites = new IVisible
            (
                ReserveUnites + "M$",
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, 0, 0),
                Scene
            );
            WidgetReserveUnites.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetReserveUnites.Taille = 3;

            WidgetVaguesRestantes = new IVisible
            (
                VaguesRestantes.ToString(),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, -40, 0),
                Scene
            );
            WidgetVaguesRestantes.PrioriteAffichage = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetVaguesRestantes.Taille = 3;
        }

        public override void Update(GameTime gameTime)
        {
            this.Sablier.TempsRestant = this.TempsProchaineVague;
            this.Sablier.Update(gameTime);

            MenuProchaineVague.Visible = Curseur.Actif && Core.Physique.Facade.collisionCercleRectangle(Curseur.Cercle, Sablier.Rectangle);
        }

        public override void Draw(GameTime gameTime)
        {
            WidgetReserveUnites.Texte = ReserveUnites + "M$";
            WidgetVaguesRestantes.Texte = (VaguesRestantes == -1) ? "Inf." : VaguesRestantes.ToString();

            Scene.ajouterScenable(WidgetReserveUnites);
            Scene.ajouterScenable(WidgetVaguesRestantes);

            this.Sablier.Draw(gameTime);
            this.MenuProchaineVague.Draw(null);
        }
    }
}
