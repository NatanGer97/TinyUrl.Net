using System.Net;
using TinyUrl.Models.Exceptions;

namespace StudentsDashboard.Errors
{
    public class NotFoundException : CustomeException
    {
        public NotFoundException(string msg) 
            : base(msg, null, HttpStatusCode.NotFound)
        {
            
        }

        
    }
}
