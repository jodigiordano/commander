namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PowerUpLastSolution : PowerUp
    {
        public float ZoneImpactDestruction  { get; private set; }
        public int AttackPoints             { get; private set; }
        public bool GoAhead                 { get; private set; }
        public SimPlayerSelection Selection;
        public CelestialBody CelestialBody;

        private bool terminated;


        public PowerUpLastSolution(Simulator simulator)
            : base(simulator)
        {
            ZoneImpactDestruction = 200;
            AttackPoints = int.MaxValue / 2;
            Type = PowerUpType.FinalSolution;
            Category = PowerUpCategory.Other;
            PayOnActivation = false;
            PayOnUse = true;
            BuyImage = "Destruction";
            UsePrice = 500;
            BuyTitle = "The final solution (" + UsePrice + "M$)";
            BuyDescription = "Create a massive explosion by destroying a planet.";
            NeedInput = true;
            Crosshair = "crosshairDestruction";
            Position = Vector3.Zero;
            GoAhead = false;
            SfxIn = "sfxLastSolutionIn";
            SfxOut = "sfxLastSolutionOut";
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || terminated; }
        }


        public override void DoInputPressed()
        {
            if (Selection.CelestialBody == null)
                return;

            CelestialBody = Selection.CelestialBody;

            terminated = true;
            GoAhead = true;
        }


        public override void DoInputCanceled()
        {
            terminated = true;
            GoAhead = false;
        }


        public override void Start()
        {
            terminated = false;
        }
    }
}
