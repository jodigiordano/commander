namespace EphemereGames.Commander
{
    using System.Collections.Generic;


    class CheatsController
    {
        public event StringHandler CheatActivated;


        private Dictionary<string, Cheat> Cheats;


        public CheatsController()
        {
            Cheats = new Dictionary<string, Cheat>();

            Cheat c;
            
            c = new AllLevelsUnlockedCheat();
            Cheats.Add(c.Name, c);

            c = new LevelWonCheat();
            Cheats.Add(c.Name, c);
        }


        public void Initialize()
        {
            foreach (var cheat in Cheats.Values)
                cheat.Initialize();
        }


        public bool IsCheatActive(string cheatName)
        {
            return Cheats[cheatName].Active;
        }


        public void Update()
        {
            foreach (var c in Cheats.Values)
            {
                if (c.ActivatedThisTick)
                    NotifyCheatActivated(c.Name);

                c.Update();
            }
        }


        private void NotifyCheatActivated(string name)
        {
            if (CheatActivated != null)
                CheatActivated(name);
        }
    }
}
