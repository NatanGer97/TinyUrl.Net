using System.Net;
using TinyUrl.Models.Exceptions;

namespace StudentsDashboard.Errors
{
    public class ConflictException : CustomeException
    {
        public ConflictException(string msg) : base(msg,null, HttpStatusCode.Conflict )
        {

        }
    }
}
