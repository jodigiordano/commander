namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    public class PlanetCBDescriptor : CustomizableBodyDescriptor
    {
        public string Image;
        public bool HasMoons;
        public bool IsALevel;

        [XmlIgnore]
        public List<TurretDescriptor> StartingTurrets;


        public PlanetCBDescriptor()
        {
            Image = "";
            StartingTurrets = new List<TurretDescriptor>();
            HasMoons = true;
            IsALevel = true;
        }


        public void AddTurret(TurretType type, int level, Vector3 position, bool visible, bool canSell, bool canUpgrade, bool canSelect)
        {
            StartingTurrets.Add(CreateTurret(type, level, position, visible, canSell, canUpgrade, canSelect));
        }


        public void AddTurret(TurretType type, int level, Vector3 position)
        {
            AddTurret(type, level, position, true, true, true, true);
        }


        public void AddTurret(TurretType type, int level, Vector3 position, bool visible, bool canSelect)
        {
            AddTurret(type, level, position, visible, true, true, canSelect);
        }


        public TurretDescriptor CreateTurret(TurretType type, int level, Vector3 position, bool visible, bool canSell, bool canUpgrade, bool canSelect)
        {
            return new TurretDescriptor()
            {
                Type = type,
                Level = level,
                Position = position,
                Visible = visible,
                CanSell = canSell,
                CanUpgrade = canUpgrade,
                CanSelect = canSelect
            };
        }


        public override CelestialBody GenerateSimulatorObject(double visualPriority)
        {
            return InitializeIt(new Planet(Name, Image, visualPriority)
            {
                HasMoons = HasMoons,
                IsALevel = IsALevel
            });
        }
    }


    public class PinkHoleCBDescriptor : CustomizableBodyDescriptor
    {
        public override CelestialBody GenerateSimulatorObject(double visualPriority)
        {
            return InitializeIt(new PinkHole(Name, visualPriority));
        }
    }


    public class AsteroidBeltCBDescriptor : CelestialBodyDescriptor
    {
        public List<string> Images;


        public AsteroidBeltCBDescriptor()
        {
            Images = new List<string>();
        }


        public override CelestialBody GenerateSimulatorObject(double visualPriority)
        {
            return new AsteroidBelt(Name, Images, visualPriority);
        }
    }


    [XmlInclude(typeof(PlanetCBDescriptor))]
    [XmlInclude(typeof(PinkHoleCBDescriptor))]
    public abstract class CustomizableBodyDescriptor : CelestialBodyDescriptor
    {
        public bool HasGravitationalTurret;
        public Size Size;
        public float Speed;
        public int PathPriority;
        public Vector3 Path;
        public Vector3 Position;
        public float Rotation;
        public bool CanSelect;
        public bool Invincible;
        public bool FollowPath;
        public bool StraightLine;

        private Matrix RotationMatrix;


        public CustomizableBodyDescriptor()
        {
            HasGravitationalTurret = false;
            Speed = 0;
            PathPriority = -1;
            Path = Vector3.Zero;
            Position = Vector3.Zero;
            CanSelect = true;
            Invincible = false;
            Size = Size.Normal;
            FollowPath = false;
            StraightLine = false;
            Rotation = 0;
            RotationMatrix = new Matrix();
        }


        public PhysicalRectangle GetBoundaries()
        {
            Matrix.CreateRotationZ(Rotation, out RotationMatrix);

            Vector3 position1 = Position;
            Vector3 position2 = Position;
            Vector3 position3 = Position;
            Vector3 position4 = Position;

            CelestialBodySteeringBehavior.Move(Speed, Speed * 0.00f, ref Path, ref Position, ref RotationMatrix, ref position1);
            CelestialBodySteeringBehavior.Move(Speed, Speed * 0.25f, ref Path, ref Position, ref RotationMatrix, ref position2);
            CelestialBodySteeringBehavior.Move(Speed, Speed * 0.50f, ref Path, ref Position, ref RotationMatrix, ref position3);
            CelestialBodySteeringBehavior.Move(Speed, Speed * 0.75f, ref Path, ref Position, ref RotationMatrix, ref position4);

            var left = Math.Min(position1.X, position2.X);
            left = Math.Min(left, position3.X);
            left = Math.Min(left, position4.X);

            var right = Math.Max(position1.X, position2.X);
            right = Math.Max(right, position3.X);
            right = Math.Max(right, position4.X);

            var top = Math.Min(position1.Y, position2.Y);
            top = Math.Min(top, position3.Y);
            top = Math.Min(top, position4.Y);

            var bottom = Math.Max(position1.Y, position2.Y);
            bottom = Math.Max(bottom, position3.Y);
            bottom = Math.Max(bottom, position4.Y);

            return new PhysicalRectangle(
                (int) left,
                (int) top,
                (int) (Math.Abs(right - left)),
                (int) (Math.Abs(bottom - top)));
        }


        public CelestialBody InitializeIt(CelestialBody cb)
        {
            cb.Size = Size;
            cb.Speed = Speed;
            cb.PathPriority = PathPriority;
            cb.SteeringBehavior.Path = Path;
            cb.SteeringBehavior.BasePosition = Position;
            cb.SteeringBehavior.PathRotation = Rotation;
            cb.CanSelect = CanSelect;
            cb.Invincible = Invincible;
            cb.FollowPath = FollowPath;
            cb.StraightLine = StraightLine;

            return cb;
        }
    }


    [XmlInclude(typeof(CustomizableBodyDescriptor))]
    [XmlInclude(typeof(AsteroidBeltCBDescriptor))]
    public abstract class CelestialBodyDescriptor
    {
        public string Name;


        public CelestialBodyDescriptor()
        {
            Name = "Undefined";
        }


        public abstract CelestialBody GenerateSimulatorObject(double visualPriority);
    }
}
