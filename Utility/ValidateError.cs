namespace Reddit_App.Utility
{
    public class ValidateError : Exception
    {
        public int ErrorCode { get; set; }
        public ValidateError(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
