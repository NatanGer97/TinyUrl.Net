using System.Globalization;
using TinyUrl.Models.Exceptions;

namespace TinyUrl.Models.Dto
{
    public class UserStatInfoReqDto
    {
        public string Username { get; set; }

        public string? Tinycode { get; set; }


        
        public string? MonthAndYear { get; set; }




        public UserStatInfoReqDto()
        {

        }

    }
}
