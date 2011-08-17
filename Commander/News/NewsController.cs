namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;


    class NewsController
    {
        public event NewsTypeHandler LoadingStarted;
        public event NewsTypeHandler LoadingDoneWithError;
        public event NewsTypeNewsHandler LoadingDoneSuccessfully;

        private enum NewsLoadingState
        {
            NotLoaded,
            Loading,
            Error,
            Loaded
        };


        private Dictionary<NewsType, List<News>> News;
        private Dictionary<NewsType, NewsLoadingState> States;


        public NewsController()
        {
            News = new Dictionary<NewsType, List<News>>(NewsTypeComparer.Default);
            States = new Dictionary<NewsType, NewsLoadingState>(NewsTypeComparer.Default);

            News.Add(NewsType.DLC, new List<News>());
            News.Add(NewsType.General, new List<News>());
            News.Add(NewsType.Updates, new List<News>());

            States.Add(NewsType.DLC, NewsLoadingState.NotLoaded);
            States.Add(NewsType.General, NewsLoadingState.NotLoaded);
            States.Add(NewsType.Updates, NewsLoadingState.NotLoaded);
        }


        public void Initialize()
        {
            
        }


        public void LoadNewsAsync(NewsType type)
        {
            if (States[type] == NewsLoadingState.Loading)
                return;

            if (States[type] == NewsLoadingState.Loaded)
            {
                NotifyLoadingDoneSuccessfully(type);
                return;
            }

            States[type] = NewsLoadingState.Loading;
            NotifyLoadingStarted(type);

            switch (type)
            {
                case NewsType.General: ParallelTasks.Parallel.Start(new Action(LoadGeneralNews)); break;
                case NewsType.Updates: ParallelTasks.Parallel.Start(new Action(LoadUpdateNews)); break;
                case NewsType.DLC: ParallelTasks.Parallel.Start(new Action(LoadDLCNews)); break;
            }
        }


        public void Update()
        {

        }


        private void LoadGeneralNews()
        {
            SynchronizeNews(NewsType.General);
        }


        private void LoadUpdateNews()
        {
            SynchronizeNews(NewsType.Updates);
        }


        private void LoadDLCNews()
        {
            SynchronizeNews(NewsType.DLC);
        }


        //private void GeneralNewsLoaded()
        //{
        //    if (States[NewsType.General] == NewsLoadingState.Loaded)
        //        NotifyLoadingDoneSuccessfully(NewsType.General);
        //    else
        //        NotifyLoadingDoneWithError(NewsType.General);
        //}


        //private void UpdateNewsLoaded()
        //{
        //    if (States[NewsType.Updates] == NewsLoadingState.Loaded)
        //        NotifyLoadingDoneSuccessfully(NewsType.Updates);
        //    else
        //        NotifyLoadingDoneWithError(NewsType.Updates);
        //}


        //private void DLCNewsLoaded()
        //{
        //    if (States[NewsType.DLC] == NewsLoadingState.Loaded)
        //        NotifyLoadingDoneSuccessfully(NewsType.DLC);
        //    else
        //        NotifyLoadingDoneWithError(NewsType.DLC);
        //}


        private void SynchronizeNews(NewsType type)
        {
            bool ok = true;

            try
            {
                XDocument reader = XDocument.Load(Preferences.WebsiteURL + GetRelativeURL(type));

                News[type] =
                  (from item in reader.Descendants("item")
                   select new News
                   {
                       Title = System.Net.WebUtility.HtmlDecode(item.Element("title").Value),
                       Description = System.Net.WebUtility.HtmlDecode(item.Element("description").Value),
                       Date = DateTime.Parse(System.Net.WebUtility.HtmlDecode(item.Element("pubDate").Value))
                   }).ToList();
            }

            catch
            {
                ok = false;
            }

            States[type] = ok ? NewsLoadingState.Loaded : NewsLoadingState.Error;

            if (States[type] == NewsLoadingState.Loaded)
                NotifyLoadingDoneSuccessfully(type);
            else
                NotifyLoadingDoneWithError(type);
        }


        private string GetRelativeURL(NewsType type)
        {
            string result = null;

            switch (type)
            {
                case NewsType.General: result = Preferences.GeneralNewsURL; break;
                case NewsType.Updates: result = Preferences.UpdatesNewsURL; break;
                case NewsType.DLC: result = Preferences.DLCNewsURL; break;
            }

            return result;
        }


        private void NotifyLoadingStarted(NewsType type)
        {
            if (LoadingStarted != null)
                LoadingStarted(type);
        }


        private void NotifyLoadingDoneWithError(NewsType type)
        {
            if (LoadingDoneWithError != null)
                LoadingDoneWithError(type);
        }


        private void NotifyLoadingDoneSuccessfully(NewsType type)
        {
            if (LoadingDoneSuccessfully != null)
                LoadingDoneSuccessfully(type, News[type]);
        }
    }
}
