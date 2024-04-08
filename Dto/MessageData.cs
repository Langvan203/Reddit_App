namespace Reddit_App.Dto
{
    public class MessageData
    {
        public int Status { get; set; } = 1;
        public int ErrorCode { get; set; }
        public string Code { get; set; } = "Ok";
        public object Data { get; set; }
        public string Des { get; set; }
    }
}
