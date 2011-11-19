namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using EphemereGames.Core.SimplePersistence;


    public class HighScores : SimpleData
    {
        [XmlArrayItem("Level")]
        public List<LevelScores> Scores;

        private Dictionary<int, LevelScores> InternalScores;


        public HighScores()
        {
            Scores = new List<LevelScores>();
            InternalScores = new Dictionary<int, LevelScores>();

            File = "highscores.xml";
        }


        public void Add(int level, string player, int score)
        {
            if (!InternalScores.ContainsKey(level))
            {
                var levelScore = new LevelScores() { id = level };
                InternalScores.Add(level, levelScore);
                Scores.Add(levelScore);
            }

            InternalScores[level].Add(player, score);
        }


        public bool ContainsHighScores(int level)
        {
            return GetHighScore(level) != null;
        }


        public LevelScore GetHighScore(int level)
        {
            LevelScores levelScores;

            if (!InternalScores.TryGetValue(level, out levelScores))
                return null;

            return levelScores.GetHighest();
        }


        public LevelScores GetAllHighScores(int level)
        {
            LevelScores levelScores;

            if (!InternalScores.TryGetValue(level, out levelScores))
                return new LevelScores();

            return levelScores;
        }


        public void Clear()
        {
            foreach (var s in Scores)
                s.Clear();
        }


        protected override void DoInitialize(object data)
        {
            var d = data as HighScores;

            Scores = d.Scores;


            InternalScores.Clear();

            foreach (var s in Scores)
                InternalScores.Add(s.id, s);
        }


        public override void DoFileNotFound()
        {
            base.DoFileNotFound();

            FirstLoad();
        }


        protected override void DoLoadFailed()
        {
            base.DoLoadFailed();

            FirstLoad();
        }


        private void FirstLoad()
        {
            Scores = new List<LevelScores>();
            InternalScores = new Dictionary<int, LevelScores>();

            Main.PlayersController.CreateDirectory(Directory);
            Persistence.SaveData(this);
            Loaded = true;
        }
    }


    public class LevelScores
    {
        [XmlAttribute("id")]
        public int id;

        [XmlArrayItem("Score")]
        public List<LevelScore> Scores;


        public LevelScores()
        {
            id = -1;
            Scores = new List<LevelScore>();
        }


        public void Add(string player, int score)
        {
            Scores.Add(new LevelScore() { Player = player, Score = score });

            Scores.Sort(delegate(LevelScore s1, LevelScore s2) { return -s1.Score.CompareTo(s2.Score); });

            if (Scores.Count > 10)
                Scores.RemoveAt(9);
        }


        public LevelScore GetHighest()
        {
            return Scores.Count > 0 ? Scores[0] : null;
        }


        public void Clear()
        {
            Scores.Clear();
        }
    }


    public class LevelScore
    {
        [XmlAttribute("player")]
        public string Player;

        [XmlAttribute("score")]
        public int Score;


        public LevelScore()
        {
            Player = "";
            Score = 0;
        }
    }
}
