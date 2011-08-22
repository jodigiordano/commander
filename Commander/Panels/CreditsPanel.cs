namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CreditsPanel : VerticalPanel
    {
        private Label Jodi;
        private Label Website;
        private Label Tag;
        private Label Mercury;
        private Label EasyStorage;
        private Label Backgrounds;
        private Label SpecialThanks;
        private VerticalSeparatorWidget Seperator1;
        private VerticalSeparatorWidget Seperator2;


        public CreditsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Credits");
            DistanceBetweenTwoChoices = 15;
            CenterWidgets = true;

            Alpha = 0;

            Seperator1 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            Jodi = new Label(new Text("A game by Jodi Giordano", @"Pixelite") { SizeX = 4, Alpha = 0 }.CenterIt());
            Website = new Label(new Text("ephemeregames.com", @"Pixelite") { SizeX = 3, Alpha = 0 }.CenterIt());

            Seperator2 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            SpecialThanks = new Label(new Text("Special thanks to:", @"Pixelite") { SizeX = 3, Alpha = 0 }.CenterIt());
            Tag = new Label(new Text("TAG (tag.hexagram.ca)", @"Pixelite") { SizeX = 2, Alpha = 0 }.CenterIt());
            Mercury = new Label(new Text("Mercury Particle Engine (mpe.codeplex.com)", @"Pixelite") { SizeX = 2, Alpha = 0 }.CenterIt());
            EasyStorage = new Label(new Text("EasyStorage (easystorage.codeplex.com)", @"Pixelite") { SizeX = 2, Alpha = 0 }.CenterIt());
            Backgrounds = new Label(new Text("NASA/STScI for some of the backgrounds", @"Pixelite") { SizeX = 2, Alpha = 0 }.CenterIt());

            AddWidget("Seperator", Seperator1);
            AddWidget("Jodi", Jodi);
            AddWidget("Website", Website);
            AddWidget("Seperator", Seperator2);
            AddWidget("ST", SpecialThanks);
            AddWidget("Tag", Tag);
            AddWidget("Backgrounds", Backgrounds);
            AddWidget("Mercury", Mercury);
            AddWidget("EasyStorage", EasyStorage);
        }
    }
}
