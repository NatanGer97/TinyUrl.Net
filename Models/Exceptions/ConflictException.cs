using System.Net;
using TinyUrl.Models.Exceptions;

namespace TinyUrl.Errors
{
    public class ConflictException : CustomeException
    {
        public ConflictException(string msg) : base(msg,null, HttpStatusCode.Conflict )
        {

        }
    }
}
