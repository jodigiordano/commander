namespace EphemereGames.Commander.Simulation
{
    abstract class EditorCelestialBodyCommand : EditorCommand
    {
        public CelestialBody CelestialBody;


        public EditorCelestialBodyCommand(SimPlayer owner, CelestialBody celestialBody)
            : base(owner)
        {
            CelestialBody = celestialBody;
        }
    }


    class EditorCelestialBodySpeedCommand : EditorCelestialBodyCommand
    {
        public float Speed;


        public EditorCelestialBodySpeedCommand(SimPlayer owner, float speed)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            Speed = speed;
        }
    }


    class EditorCelestialBodyShowPathCommand : EditorCelestialBodyCommand
    {
        public bool ShowPath;


        public EditorCelestialBodyShowPathCommand(SimPlayer owner, bool showPath)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            ShowPath = showPath;
        }
    }


    class EditorCelestialBodyMoveCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyMoveCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            Owner.ActualSelection.EditingState = EditorEditingState.MovingCB; 
        }
    }


    class EditorCelestialBodyRotateCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyRotateCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            Owner.ActualSelection.EditingState = EditorEditingState.RotatingCB;
        }
    }


    class EditorCelestialBodyTrajectoryCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyTrajectoryCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            Owner.ActualSelection.EditingState = EditorEditingState.TrajectoryCB;
        }
    }


    class EditorCelestialBodyRemoveCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyRemoveCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {

        }
    }


    class EditorCelestialBodyPushFirstCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyPushFirstCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {

        }
    }


    class EditorCelestialBodyPushLastCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyPushLastCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {

        }
    }


    class EditorCelestialBodyRemoveFromPathCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyRemoveFromPathCommand(SimPlayer owner)
            : base(owner, owner.ActualSelection.CelestialBody)
        {

        }
    }


    class EditorCelestialBodySizeCommand : EditorCelestialBodyCommand
    {
        public Size Size;

        public EditorCelestialBodySizeCommand(SimPlayer owner, Size size)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            Size = size;
        }
    }


    class EditorCelestialBodyChangeAssetCommand : EditorCelestialBodyCommand
    {
        public string AssetName;

        public EditorCelestialBodyChangeAssetCommand(SimPlayer owner, CelestialBody cb, string assetName)
            : base(owner, cb)
        {
            AssetName = assetName;
        }
    }


    class EditorCelestialBodyHasMoonsCommand : EditorCelestialBodyCommand
    {
        public bool HasMoons;

        public EditorCelestialBodyHasMoonsCommand(SimPlayer owner, CelestialBody cb, bool hasMoons)
            : base(owner, owner.ActualSelection.CelestialBody)
        {
            HasMoons = hasMoons;
        }
    }


    class EditorCelestialBodyFollowPathCommand : EditorCelestialBodyCommand
    {
        public bool FollowPath;

        public EditorCelestialBodyFollowPathCommand(SimPlayer owner, CelestialBody cb, bool followPath)
            : base(owner, cb)
        {
            FollowPath = followPath;
        }
    }


    class EditorCelestialBodyCanSelectCommand : EditorCelestialBodyCommand
    {
        public bool CanSelect;

        public EditorCelestialBodyCanSelectCommand(SimPlayer owner, CelestialBody cb, bool canSelect)
            : base(owner, cb)
        {
            CanSelect = canSelect;
        }
    }


    class EditorCelestialBodyStraightLineCommand : EditorCelestialBodyCommand
    {
        public bool StraightLine;

        public EditorCelestialBodyStraightLineCommand(SimPlayer owner, CelestialBody cb, bool straightLine)
            : base(owner, cb)
        {
            StraightLine = straightLine;
        }
    }


    class EditorCelestialBodyInvincibleCommand : EditorCelestialBodyCommand
    {
        public bool Invincible;

        public EditorCelestialBodyInvincibleCommand(SimPlayer owner, CelestialBody cb, bool invincible)
            : base(owner, cb)
        {
            Invincible = invincible;
        }
    }


    class EditorCelestialBodyAddCommand : EditorCelestialBodyCommand
    {

        public EditorCelestialBodyAddCommand(SimPlayer owner, CelestialBody celestialBody)
            : base(owner, celestialBody)
        {

        }
    }
}
