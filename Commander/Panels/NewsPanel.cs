namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewsPanel : VerticalPanel
    {
        private List<NewsWidget> AllNews;
        private PushButton Sync;
        private PushButton VisitWebsite;
        private Label LoadingInfos;

        private int MaxNewsDisplayed;
        private NewsType Type;


        public NewsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color, NewsType type, string title)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle(title);
            Type = type;
            DistanceBetweenTwoChoices = 30;

            Alpha = 0;

            AllNews = new List<NewsWidget>();

            var syncText = new Text("Reload", @"Pixelite") { SizeX = 2 };
            Sync = new PushButton(syncText, (int) syncText.AbsoluteSize.X + 20);
            Sync.ClickHandler = DoReloadClicked;

            var vwText = new Text("Visit website", @"Pixelite") { SizeX = 2 };
            VisitWebsite = new PushButton(vwText, (int) vwText.AbsoluteSize.X + 20);
            VisitWebsite.ClickHandler = DoVisitWebsiteClicked;

            AddTitleBarWidget(VisitWebsite);

            LoadingInfos = new Label(new Text(@"Pixelite") { SizeX = 4 });

            Main.NewsController.LoadingStarted += new NewsTypeHandler(DoLoadingStarted);
            Main.NewsController.LoadingDoneSuccessfully += new NewsTypeNewsHandler(DoLoadedSuccessfully);
            Main.NewsController.LoadingDoneWithError += new NewsTypeHandler(DoLoadedError);

            MaxNewsDisplayed = 3;
        }


        public void DoLoadingStarted(NewsType type)
        {
            if (type != this.Type)
                return;

            LoadingInfos.SetData("Loading news, please wait.");
            RemoveWidget("LoadingInfos");
            AddWidget("LoadingInfos", LoadingInfos);
        }


        public void DoLoadedSuccessfully(NewsType type, List<News> news)
        {
            if (type != this.Type)
                return;

            RemoveWidget("LoadingInfos");
            RemoveWidget("Sync");

            foreach (var n in AllNews)
                RemoveWidget(n.Name);

            AllNews.Clear();

            news.Sort();

            for (int i = 0; i < Math.Min(news.Count, MaxNewsDisplayed); i++)
            {
                var n = new NewsWidget(news[i], (int) Dimension.X - 50);

                AllNews.Add(n);
                AddWidget("news" + i, n);
            }
        }


        public void DoLoadedError(NewsType type)
        {
            if (type != this.Type)
                return;

            LoadingInfos.SetData("Loading error. Please retry.");
            RemoveWidget("Sync");
            AddWidget("Sync", Sync);
        }



        private void DoVisitWebsiteClicked(PanelWidget widget)
        {
#if WINDOWS
            System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif
        }


        private void DoReloadClicked(PanelWidget widget)
        {
            NotifyReloadAsked();
        }


        private void NotifyReloadAsked()
        {
            Main.NewsController.LoadNewsAsync(Type);
        }
    }
}
