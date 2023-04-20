using System.Net;
using TinyUrl.Models.Exceptions;

namespace StudentsDashboard.Errors
{
    public class InternalServerException : CustomeException
    {
        public InternalServerException(string msg, List<string>? errors = default)
            : base(msg, errors, HttpStatusCode.InternalServerError)
        {

        }
    }
}
