﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PowerUpsMenu
    {
        public PowerUpsBattleship HumanBattleship;

        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public PowerUpType PowerUpToBuy;

        public Vector3 Position;

        private Dictionary<PowerUpType, Image> ImagesPowerUpsBuy;
        private Dictionary<PowerUpType, Image> ImagesPlaceHolders;
        private Dictionary<PowerUpType, Turret> ImagesTurretsPowerUps;
        
        private Bubble PowerUpPriceBubble;
        private Text PowerUpPriceTitleAndCost;
        private Text PowerUpDescription;
        private Color ColorPowerUpAvailable;
        private Color ColorPowerUpNotAvailable;

        private Simulator Simulator;
        private double VisualPriority;
        private float TextSize;
        private float ImageSize;
        private Vector3 DistanceBetweenTwoChoices;
        private Vector3 PowerUpsLayout;
        private bool HumanBattleshipHasArrived;

        public List<Turret> Turrets;


        public PowerUpsMenu(Simulator simulator, Vector3 position, double visualPriority)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            Position = position;
            TextSize = 2;
            ImageSize = 2;
            DistanceBetweenTwoChoices = new Vector3(30, 30, 0);
            PowerUpsLayout = new Vector3(4, 4, 0);

            PowerUpPriceTitleAndCost = new Text(@"Pixelite")
            {
                SizeX = TextSize,
                VisualPriority = VisualPriority + 0.001f
            };

            PowerUpDescription = new Text(@"Pixelite")
            {
                SizeX = TextSize - 1,
                VisualPriority = VisualPriority + 0.001f
            };

            PowerUpPriceBubble = new Bubble(Simulator.Scene, new PhysicalRectangle(0, 0, 100, 30), VisualPriority + 0.002)
            {
                BlaPosition = 3
            };

            ColorPowerUpAvailable = Color.White;
            ColorPowerUpNotAvailable = Color.Red;

            ImagesPowerUpsBuy = new Dictionary<PowerUpType, Image>(PowerUpTypeComparer.Default);
            ImagesPlaceHolders = new Dictionary<PowerUpType, Image>(PowerUpTypeComparer.Default);
            ImagesTurretsPowerUps = new Dictionary<PowerUpType, Turret>(PowerUpTypeComparer.Default);

            HumanBattleship = new PowerUpsBattleship(Simulator, VisualPriority + 0.005f);
        }


        public void Initialize()
        {
            Dictionary<PowerUpType, PowerUp> availablePowerUps = Simulator.PowerUpsFactory.Availables;

            ImagesPowerUpsBuy.Clear();
            ImagesPlaceHolders.Clear();
            ImagesTurretsPowerUps.Clear();

            HumanBattleship.Position = Position - new Vector3(300, 75, 0);
            HumanBattleshipHasArrived = false;
            HumanBattleship.Initialize();

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


            Simulator.Scene.PhysicalEffects.Add(HumanBattleship, Core.Physics.PhysicalEffects.Arrival(
                this.Position + new Vector3(50, 25, 0),
                1000,
                2000), HumanBattleshipArrived);

            foreach (var image in ImagesPowerUpsBuy)
                Simulator.Scene.PhysicalEffects.Add(image.Value, Core.Physics.PhysicalEffects.Arrival(
                    image.Value.Position + new Vector3(300, 75, 0),
                    1000,
                    2000));

            foreach (var image in ImagesPlaceHolders)
                Simulator.Scene.PhysicalEffects.Add(image.Value, Core.Physics.PhysicalEffects.Arrival(
                    image.Value.Position + new Vector3(300, 75, 0),
                    1000,
                    2000));

            foreach (var turret in ImagesTurretsPowerUps.Values)
                Simulator.Scene.PhysicalEffects.Add(turret, Core.Physics.PhysicalEffects.Arrival(
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
            if (AvailablePowerUps.Count == 0)
                return;

            // draw the power-ups
            foreach (var kvp in AvailablePowerUps)
            {
                PowerUp p = Simulator.PowerUpsFactory.Availables[kvp.Key];
             
                if (p.Category == PowerUpCategory.Turret)
                    ImagesTurretsPowerUps[p.Type].Draw();
                else
                {
                    ImagesPlaceHolders[p.Type].Color = AvailablePowerUps[p.Type] ? ColorPowerUpAvailable : ColorPowerUpNotAvailable;

                    Simulator.Scene.Add(ImagesPowerUpsBuy[p.Type]);
                    Simulator.Scene.Add(ImagesPlaceHolders[p.Type]);
                }
            }

            
            // draw the description / price of the power-up
            if (HumanBattleshipHasArrived && PowerUpToBuy != PowerUpType.None)
            {
                PowerUp p = Simulator.PowerUpsFactory.Availables[PowerUpToBuy];

                Vector3 position = (p.Category == PowerUpCategory.Turret) ?
                    ImagesTurretsPowerUps[p.Type].Position :
                    ImagesPowerUpsBuy[p.Type].Position;

                PowerUpPriceTitleAndCost.Data = p.BuyTitle;
                PowerUpPriceTitleAndCost.Position = position - new Vector3(0, 60, 0);
                PowerUpPriceTitleAndCost.Color = AvailablePowerUps[p.Type] ? ColorPowerUpAvailable : ColorPowerUpNotAvailable;

                PowerUpDescription.Data = p.BuyDescription;
                PowerUpDescription.Position = position - new Vector3(0, 40, 0);
                PowerUpDescription.Color = AvailablePowerUps[p.Type] ? ColorPowerUpAvailable : ColorPowerUpNotAvailable;

                PowerUpPriceBubble.Dimension.X = (int) position.X;
                PowerUpPriceBubble.Dimension.Y = (int) position.Y - 60;

                Simulator.Scene.Add(PowerUpPriceTitleAndCost);
                Simulator.Scene.Add(PowerUpDescription);

                PowerUpPriceBubble.Dimension.Width = (int) MathHelper.Max(PowerUpPriceTitleAndCost.AbsoluteSize.X, PowerUpDescription.AbsoluteSize.X) + 10;
                PowerUpPriceBubble.Draw();
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
                RailGunTurret rgt = new RailGunTurret(Simulator)
                {
                    Position = HumanBattleship.Position + new Vector3(130, 30, 0),
                    VisualPriority = this.VisualPriority + 0.004f,
                    PlayerControlled = true,
                    UpdatePosition = false,
                    Watcher = false,
                    TimeLastBullet = double.MaxValue,
                    RangeAlpha = 0,
                    ShowRange = false,
                    ShowForm = false
                };

                p.BuyPosition = rgt.Position + new Vector3(300, 75, 0);
                HumanBattleship.RailGun = rgt;
                Turrets.Add(rgt);

                ImagesTurretsPowerUps.Add(p.Type, rgt);
            }
        }


        private void HumanBattleshipArrived(int id)
        {
            HumanBattleshipHasArrived = true;
        }
    }
}
