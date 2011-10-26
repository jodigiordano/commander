namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpPanel : VerticalPanel
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
        }


        private void DoVisitWebsiteClicked(PanelWidget widget)
        {
#if WINDOWS
            System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif
        }
    }
}
