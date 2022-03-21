namespace SB.WebAPI.DTO
{
    public class Error_DTO_Out
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public Error_DTO_Out(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}