namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CreditsPanel : SlideshowPanel
    {
        private PushButton VisitWebsite;


        public CreditsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Credits");

            var vwText = new Text("Visit website", @"Pixelite") { SizeX = 2 };
            VisitWebsite = new PushButton(vwText, (int) vwText.AbsoluteSize.X + 20);
            VisitWebsite.ClickHandler = DoVisitWebsiteClicked;

            AddTitleBarWidget(VisitWebsite);

            Slider.SpaceForValue = 400;

            Alpha = 0;

            AddWidget("general", CreateGeneralPanel());
            AddWidget("associations", CreateAssociationsPanel());
            AddWidget("assets", CreateLibrairiesAndAssetsPanel());
            AddWidget("specialthanks", CreateSpecialThanksPanel());

            Slider.AddAlias(0, "General");
            Slider.AddAlias(1, "Associations");
            Slider.AddAlias(2, "Librairies and assets");
            Slider.AddAlias(3, "Special Thanks");
        }


        private void DoVisitWebsiteClicked(PanelWidget widget)
        {
#if WINDOWS
            System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif
        }


        private VerticalPanel CreateGeneralPanel()
        {
            VerticalPanel v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 15;
            v.CenterWidgets = true;

            var Separator1 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            var Jodi = new ColoredLabel(new ColoredText(new List<string>() { "A game by ", "Jodi Giordano" }, new Color[] { Colors.Default.NeutralBright, Colors.Default.AlienBright }, "Pixelite", Vector3.Zero) { SizeX = 4, Alpha = 0 });
            var Website = new Label(new Text("www.ephemeregames.com", "Pixelite") { SizeX = 3, Alpha = 0 });

            var Separator2 = new VerticalSeparatorWidget() { MaxAlpha = 0 };
            var Separator3 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            var SoundDesign1 = new Label(new Text("Music by Frederic Chopin.\n\nAdditional music and sound\n\ndesign by the awesome:", "Pixelite") { SizeX = 2, Alpha = 0 });
            var SoundDesign2 = new ColoredLabel(new ColoredText(new List<string>() { "FX Dupas", " (fxdupas.com)." }, new Color[] { Colors.Default.AlienBright, Colors.Default.NeutralBright }, "Pixelite", Vector3.Zero) { SizeX = 2, Alpha = 0 });
            
            v.AddWidget("Seperator", Separator1);
            v.AddWidget("Jodi", Jodi);
            v.AddWidget("Website", Website);
            v.AddWidget("Seperator2", Separator2);
            v.AddWidget("Seperator3", Separator3);
            v.AddWidget("Sound1", SoundDesign1);
            v.AddWidget("Sound2", SoundDesign2);

            return v;
        }


        private VerticalPanel CreateAssociationsPanel()
        {
            VerticalPanel v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 15;
            v.CenterWidgets = true;

            var Separator1 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            var Tag = new Label(new Text("TAG (tag.hexagram.ca)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var Latece = new Label(new Text("LATECE (latece.uqam.ca)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var AGEEI = new Label(new Text("AGEEI (ageei.org)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var MRGS = new Label(new Text("MRGS (montrealindies.com)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var UGAM = new Label(new Text("UGAM (ageei.org/groupes/ugam)", "Pixelite") { SizeX = 2, Alpha = 0 });

            v.AddWidget("Seperator", Separator1);
            v.AddWidget("Tag", Tag);
            v.AddWidget("Latece", Latece);
            v.AddWidget("AGEEI", AGEEI);
            v.AddWidget("MRGS", MRGS);
            v.AddWidget("UGAM", UGAM);

            return v;
        }


        private VerticalPanel CreateLibrairiesAndAssetsPanel()
        {
            VerticalPanel v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 15;
            v.CenterWidgets = true;

            var Separator1 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            var Mercury = new Label(new Text("Mercury Particle Engine (mpe.codeplex.com)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var EasyStorage = new Label(new Text("EasyStorage (easystorage.codeplex.com)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var Backgrounds = new Label(new Text("NASA/STScI for some of the backgrounds", "Pixelite") { SizeX = 2, Alpha = 0 });
            var Xbox360Buttons = new Label(new Text("Xbox Button Pack by Jeff Jenkins (sinnix.net)", "Pixelite") { SizeX = 2, Alpha = 0 });
            var Font = new Label(new Text("Pixelite font by CK (freakyfonts.de)", "Pixelite") { SizeX = 2, Alpha = 0 });

            v.AddWidget("Seperator", Separator1);
            v.AddWidget("Backgrounds", Backgrounds);
            v.AddWidget("Mercury", Mercury);
            v.AddWidget("EasyStorage", EasyStorage);
            v.AddWidget("Xbox360Buttons", Xbox360Buttons);
            v.AddWidget("Font", Font);

            return v;
        }


        private VerticalPanel CreateSpecialThanksPanel()
        {
            VerticalPanel v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            v.OnlyShowWidgets = true;
            v.DistanceBetweenTwoChoices = 15;
            v.CenterWidgets = true;

            var Separator1 = new VerticalSeparatorWidget() { MaxAlpha = 0 };

            var All = new Label(new Text("In no special order: My family, Evil Franck,\n\nCatherine Levesque, Julien Theron,\n\nTristan Labelle, Alexis Laferriere,\n\nNilovna Bascunan-Vasquez, Sandrine Gautier-Ethier,\n\nDesura, Gamersgate, Tyler Steele,\n\nStephanie Bouchard, Yasmine Charif,\n\nWassim Jendoubi, Jean-Sebastien Gelinas,\n\nYousra Ben Fadhel, Nidhal Daghrir,\n\nFrancois Pomerleau, Saleem Dabbous,\n\nJason Della Rocca, David Sears, Maxime Doucet,\n\nMicheal McIntyre, Ethan Larson,\n\nTiego Francois-Brosseau, Paul Williams,\n\nBehrouz Bayat, Shawn Bell.", "Pixelite") { SizeX = 2, Alpha = 0 });

            v.AddWidget("Seperator1", Separator1);
            v.AddWidget("All", All);

            return v;
        }


        private VerticalPanel CreateTestersPanel()
        {
            VerticalPanel v = new VerticalPanel(Scene, Vector3.Zero, Size, VisualPriority, Color);

            return v;
        }
    }
}
