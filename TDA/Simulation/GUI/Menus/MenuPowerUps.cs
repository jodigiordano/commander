namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuPowerUps
    {
        public HumanBattleship HumanBattleship;

        public Dictionary<PowerUpType, bool> AvailablePowerUpsToBuy;
        public PowerUpType PowerUpToBuy;

        private Dictionary<PowerUpType, Image> ImagesPowerUpsBuy;
        private Dictionary<PowerUpType, Image> ImagesPlaceHolders;
        
        private Bulle PowerUpPriceBubble;
        private Text PowerUpPriceText;

        private Simulation Simulation;
        private float VisualPriority;
        private Vector3 Position;
        private float TextSize;
        private float ImageSize;
        private Vector3 DistanceBetweenTwoChoices;
        private Vector3 PowerUpsLayout;


        public MenuPowerUps(Simulation simulation, Vector3 position, float visualPriority)
        {
            Simulation = simulation;
            VisualPriority = visualPriority;
            Position = position;
            TextSize = 2;
            ImageSize = 2;
            DistanceBetweenTwoChoices = new Vector3(30, 30, 0);
            PowerUpsLayout = new Vector3(4, 4, 0);

            PowerUpPriceText = new Text("Pixelite")
            {
                SizeX = TextSize
            };

            PowerUpPriceBubble = new Bulle(Simulation, new Rectangle(0, 0, 100, 30), VisualPriority)
            {
                PositionBla = 3
            };

            ImagesPowerUpsBuy = new Dictionary<PowerUpType, Image>();
            ImagesPlaceHolders = new Dictionary<PowerUpType, Image>();

            HumanBattleship = new HumanBattleship(Simulation, this.Position - new Vector3(300, 75, 0), this.VisualPriority + 0.0002f);
        }


        public void Initialize()
        {
            Dictionary<PowerUpType, PowerUp> availablePowerUps = Simulation.PowerUpsFactory.Availables;

            var index = 0;
            foreach (var p in availablePowerUps.Values)
            {
                Vector3 gridPosition = new Vector3(index % (int) PowerUpsLayout.X, index / (int) PowerUpsLayout.Y, 0);
                Vector3 position = this.Position + gridPosition * DistanceBetweenTwoChoices - new Vector3(300, 75, 0);

                p.BuyPosition = this.Position + gridPosition * DistanceBetweenTwoChoices;

                Image image = new Image(p.BuyImage)
                {
                    VisualPriority = this.VisualPriority,
                    SizeX = ImageSize,
                    Position = position
                };
                ImagesPowerUpsBuy.Add(p.Type, image);

                Image placeHolder = new Image("powerUpPlaceHolder")
                {
                    VisualPriority = this.VisualPriority + 0.0001f,
                    SizeX = ImageSize,
                    Position = position
                };
                ImagesPlaceHolders.Add(p.Type, placeHolder);

                index++;
            }


            Simulation.Scene.Effets.Add(HumanBattleship, Core.Physique.PredefinedEffects.Arrival(
                this.Position + new Vector3(50, 25, 0),
                1000,
                2000));

            foreach (var image in ImagesPowerUpsBuy)
                Simulation.Scene.Effets.Add(image.Value, Core.Physique.PredefinedEffects.Arrival(
                    image.Value.Position + new Vector3(300, 75, 0),
                    1000,
                    2000));

            foreach (var image in ImagesPlaceHolders)
                Simulation.Scene.Effets.Add(image.Value, Core.Physique.PredefinedEffects.Arrival(
                    image.Value.Position + new Vector3(300, 75, 0),
                    1000,
                    2000));
        }


        public void Draw()
        {
            if (Simulation.PowerUpsFactory.Availables.Count == 0)
                return;

            foreach (var kvp in AvailablePowerUpsToBuy)
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[kvp.Key];
                
                Simulation.Scene.ajouterScenable(ImagesPowerUpsBuy[p.Type]);
                Simulation.Scene.ajouterScenable(ImagesPlaceHolders[p.Type]);                
            }

            if (PowerUpToBuy != PowerUpType.None)
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[PowerUpToBuy];

                PowerUpPriceText.Data = p.BuyPrice + " M$";
                PowerUpPriceText.Position = ImagesPowerUpsBuy[p.Type].Position - new Vector3(0, 60, 0);

                PowerUpPriceBubble.Dimension.X = (int) ImagesPowerUpsBuy[p.Type].Position.X;
                PowerUpPriceBubble.Dimension.Y = (int) ImagesPowerUpsBuy[p.Type].Position.Y - 60;

                Simulation.Scene.ajouterScenable(PowerUpPriceText);

                PowerUpPriceBubble.Draw(null);
            }

            HumanBattleship.Draw();
        }
    }
}
