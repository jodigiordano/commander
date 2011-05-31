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
        private Dictionary<PowerUpType, Turret> ImagesTurretsPowerUps;
        
        private Bulle PowerUpPriceBubble;
        private Text PowerUpPriceTitleAndCost;
        private Text PowerUpDescription;
        private Color ColorPowerUpAvailable;
        private Color ColorPowerUpNotAvailable;

        private Simulation Simulation;
        private float VisualPriority;
        private Vector3 Position;
        private float TextSize;
        private float ImageSize;
        private Vector3 DistanceBetweenTwoChoices;
        private Vector3 PowerUpsLayout;

        public List<Turret> Turrets;


        public MenuPowerUps(Simulation simulation, Vector3 position, float visualPriority)
        {
            Simulation = simulation;
            VisualPriority = visualPriority;
            Position = position;
            TextSize = 2;
            ImageSize = 2;
            DistanceBetweenTwoChoices = new Vector3(30, 30, 0);
            PowerUpsLayout = new Vector3(4, 4, 0);

            PowerUpPriceTitleAndCost = new Text("Pixelite")
            {
                SizeX = TextSize,
                VisualPriority = VisualPriority + 0.001f
            };

            PowerUpDescription = new Text("Pixelite")
            {
                SizeX = TextSize - 1,
                VisualPriority = VisualPriority + 0.001f
            };

            PowerUpPriceBubble = new Bulle(Simulation, new Rectangle(0, 0, 100, 30), VisualPriority + 0.002f)
            {
                PositionBla = 3
            };

            ColorPowerUpAvailable = Color.White;
            ColorPowerUpNotAvailable = Color.Red;

            ImagesPowerUpsBuy = new Dictionary<PowerUpType, Image>();
            ImagesPlaceHolders = new Dictionary<PowerUpType, Image>();
            ImagesTurretsPowerUps = new Dictionary<PowerUpType, Turret>();

            HumanBattleship = new HumanBattleship(Simulation, this.Position - new Vector3(300, 75, 0), this.VisualPriority + 0.005f);
        }


        public void Initialize()
        {
            Dictionary<PowerUpType, PowerUp> availablePowerUps = Simulation.PowerUpsFactory.Availables;

            var index = 0;
            foreach (var p in availablePowerUps.Values)
            {
                if (p.Category == PowerUpCategory.Turret)
                    InitializeTurretsPowerUps(p);
                else
                {
                    InitializeImagesPowerUps(index, p);
                    index++;
                }
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

            foreach (var turret in ImagesTurretsPowerUps.Values)
                Simulation.Scene.Effets.Add(turret, Core.Physique.PredefinedEffects.Arrival(
                    turret.Position + new Vector3(300, 75, 0),
                    1000,
                    2000));
        }


        public void Update()
        {
            foreach (var turret in ImagesTurretsPowerUps.Values)
                turret.Update();
        }


        public void Draw()
        {
            if (Simulation.PowerUpsFactory.Availables.Count == 0)
                return;

            // draw the power-ups
            foreach (var kvp in AvailablePowerUpsToBuy)
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[kvp.Key];
             
                if (p.Category == PowerUpCategory.Turret)
                    ImagesTurretsPowerUps[p.Type].Draw();
                else
                {
                    Simulation.Scene.ajouterScenable(ImagesPowerUpsBuy[p.Type]);
                    Simulation.Scene.ajouterScenable(ImagesPlaceHolders[p.Type]);
                }
            }

            
            // draw the description / price of the power-up
            if (PowerUpToBuy != PowerUpType.None)
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[PowerUpToBuy];

                Vector3 position = (p.Category == PowerUpCategory.Turret) ?
                    ImagesTurretsPowerUps[p.Type].Position :
                    ImagesPowerUpsBuy[p.Type].Position;

                PowerUpPriceTitleAndCost.Data = p.BuyTitle + " (" + p.BuyPrice + " M$)";
                PowerUpPriceTitleAndCost.Position = position - new Vector3(0, 60, 0);
                PowerUpPriceTitleAndCost.Color = AvailablePowerUpsToBuy[p.Type] ? ColorPowerUpAvailable : ColorPowerUpNotAvailable;

                PowerUpDescription.Data = p.BuyDescription;
                PowerUpDescription.Position = position - new Vector3(0, 40, 0);
                PowerUpDescription.Color = AvailablePowerUpsToBuy[p.Type] ? ColorPowerUpAvailable : ColorPowerUpNotAvailable;

                PowerUpPriceBubble.Dimension.X = (int) position.X;
                PowerUpPriceBubble.Dimension.Y = (int) position.Y - 60;

                Simulation.Scene.ajouterScenable(PowerUpPriceTitleAndCost);
                Simulation.Scene.ajouterScenable(PowerUpDescription);

                PowerUpPriceBubble.Dimension.Width = (int) MathHelper.Max(PowerUpPriceTitleAndCost.TextSize.X, PowerUpDescription.TextSize.X) + 10;
                PowerUpPriceBubble.Draw(null);
            }

            // draw the battleship
            HumanBattleship.Draw();
        }


        private void InitializeImagesPowerUps(int index, PowerUp p)
        {
            Vector3 gridPosition = new Vector3(index % (int) PowerUpsLayout.X, index / (int) PowerUpsLayout.Y, 0);
            Vector3 position = this.Position + gridPosition * DistanceBetweenTwoChoices - new Vector3(300, 75, 0);

            p.BuyPosition = this.Position + gridPosition * DistanceBetweenTwoChoices;

            Image image = new Image(p.BuyImage)
            {
                VisualPriority = this.VisualPriority + 0.003f,
                SizeX = ImageSize,
                Position = position
            };
            ImagesPowerUpsBuy.Add(p.Type, image);

            Image placeHolder = new Image("powerUpPlaceHolder")
            {
                VisualPriority = this.VisualPriority + 0.004f,
                SizeX = ImageSize,
                Position = position
            };
            ImagesPlaceHolders.Add(p.Type, placeHolder);
        }


        private void InitializeTurretsPowerUps(PowerUp p)
        {
            if (p.Type == PowerUpType.RailGun)
            {
                RailGunTurret rgt = new RailGunTurret(Simulation)
                {
                    Position = HumanBattleship.Position + new Vector3(120, 30, 0),
                    VisualPriority = this.VisualPriority + 0.003f,
                    PlayerControlled = true,
                    UpdatePosition = false,
                    Watcher = false,
                    TimeLastBullet = double.MaxValue
                };

                p.BuyPosition = rgt.Position + new Vector3(300, 75, 0);
                HumanBattleship.RailGun = rgt;
                Turrets.Add(rgt);

                ImagesTurretsPowerUps.Add(p.Type, rgt);
            }
        }
    }
}
