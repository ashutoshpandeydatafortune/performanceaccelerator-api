namespace DF_EvolutionAPI.Models.Response
{
    public class ErrorResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }

        public ErrorResponse() { }
        public ErrorResponse(bool error, string message, string errorCode)
        {
            Error = error;
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
