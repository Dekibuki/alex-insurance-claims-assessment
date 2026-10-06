namespace Claims.Domain.Exceptions
{
    public class BadValidationException : Exception
    {
        public int StatusCode { get; set; }

        public BadValidationException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
