namespace CitroenAPI.Models
{
    public class RootObject
    {
        public int code { get; set; }
        public int nbResults { get; set; }
        public string method { get; set; }
        public List<Message> message { get; set; }
    }
}
