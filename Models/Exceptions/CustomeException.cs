using System.Net;

namespace TinyUrl.Models.Exceptions
{
    public class CustomeException : Exception
    {
        public List<string> ErrorMessages { get; }
        public HttpStatusCode StatusCode{ get; }

        public CustomeException(string msg, List<string> errors = default, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
            : base(msg)
        {
            ErrorMessages = errors;
            StatusCode = httpStatusCode;
        }
    }
}
