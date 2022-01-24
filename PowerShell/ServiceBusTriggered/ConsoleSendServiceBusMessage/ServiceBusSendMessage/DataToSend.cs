namespace ServiceBusSendMessage
{
    public class DataToSend
    {
        public DataToSend()
        {
        }

        public DataToSend(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; set; }
        public string Url { get; set; }
    }
}
