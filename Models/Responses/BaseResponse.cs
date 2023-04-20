using System.Net;

namespace TinyUrl.Models.Responses
{
    public abstract  class BaseResponse
    {
 
        public string  Message { get; set; }


        public bool IsSuccess { get; set; }

        public BaseResponse()
        {

        }

        public BaseResponse(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
