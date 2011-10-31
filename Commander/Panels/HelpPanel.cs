namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpPanel : SlideshowPanel
    {
        private PushButton VisitWebsite;


        public HelpPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("How to play");

            var vwText = new Text("Visit website", @"Pixelite") { SizeX = 2 };
            VisitWebsite = new PushButton(vwText, (int) vwText.AbsoluteSize.X + 20);
            VisitWebsite.ClickHandler = DoVisitWebsiteClicked;

            AddTitleBarWidget(VisitWebsite);

            Alpha = 0;
            Slider.SpaceForValue = 400;

            AddWidget("general", CreateGeneralSubPanel());
            AddWidget("turrets", CreateTurretsSubPanel());
            AddWidget("asteroids", CreateAsteroidsSubPanel());
            AddWidget("score", CreateScoreSubPanel());

            Slider.AddAlias(0, "General");
            Slider.AddAlias(1, "Turrets");
            Slider.AddAlias(2, "Asteroids");
            Slider.AddAlias(3, "Score");
        }


        private void DoVisitWebsiteClicked(PanelWidget widget)
        {
#if WINDOWS
            System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif
        }


        private VerticalPanel CreateGeneralSubPanel()
        {
            var v = CreateEmptySubPanel();

            v.AddWidget("1", CreateMessage("Commander is a mix of tower defense, geometry wars and awesomeness."));
            v.AddWidget("2", CreateMessage("In each level, you must protect a planetary system from evil aliens that throw asteroids at it."));
            v.AddWidget("3", CreateMessage("To do so, you must install turrets around the planets. Those turrets will automatically fire at the asteroids when they are in range."));
            v.AddWidget("4", CreateMessage("You can also fire at those asteroids with your spaceship."));

            return v;
        }


        private VerticalPanel CreateTurretsSubPanel()
        {
            var v = CreateEmptySubPanel();

            v.AddWidget("1", CreateMessage("You can buy, install, upgrade and sell turrets."));
            v.AddWidget("2", CreateMessage("Buying a turret cost money. You gain money by destroying asteroids. Spend wisely!"));
            v.AddWidget("3", CreateMessage("When you install a turret, it must not overlap with previously installed ones and must not be too close to the center of the planet."));
            v.AddWidget("4", CreateMessage("Upgrading a turret boost its range, fire rate and bullet damage."));
            v.AddWidget("5", CreateMessage("Each turret is unique. Refer to the contextual help when buing a turret for more information."));

            return v;
        }


        private VerticalPanel CreateAsteroidsSubPanel()
        {
            var v = CreateEmptySubPanel();

            v.AddWidget("1", CreateMessage("Asteroids are thrown in waves by the evil aliens."));
            v.AddWidget("2", CreateMessage("They follow a path and must not reach its end!"));
            v.AddWidget("3", CreateMessage("Some asteroids are faster than others or have more or less health. Some have special characteristics that you will discover while playing."));
            v.AddWidget("4", CreateMessage("Each wave is usually stronger than the previous one."));
            v.AddWidget("5", CreateMessage("The number of remaining waves, the time until the next wave and it's composition can be found on the upper game bar."));
            v.AddWidget("6", CreateMessage("You can call the next wave earlier by clicking on the alien spaceship at the start of the path."));

            return v;
        }


        private VerticalPanel CreateScoreSubPanel()
        {
            var v = CreateEmptySubPanel();

            v.AddWidget("1", CreateMessage("Your score in each level depends on the time taken to finish it, your remaining cash and your remaining lives."));
            v.AddWidget("2", CreateMessage("How good you get in a level is translated into a 3 stars rating."));
            v.AddWidget("3", CreateMessage("So to get a better score (and get 3 stars): call the waves earlier, don't spend too much money and keep all your lives."));

            return v;
        }


        private Label CreateMessage(string message)
        {
            return new Label(new Text(message, "Pixelite") { SizeX = 2, Alpha = 0 }.CompartmentalizeIt((int) Size.X - 60));
        }


        private VerticalPanel CreateEmptySubPanel()
        {
            var v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 30;
            v.Padding += new Vector2(30, 0);

            return v;
        }
    }
}
