namespace EphemereGames.Commander
{
    public class MultiverseMessage
    {
        public MultiverseMessageType Type;
        public string Message;


        public MultiverseMessage()
        {
            Type = MultiverseMessageType.None;
            Message = "";
        }
    }
}
