using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TinyUrl.Models.Dto
{
    public class NewTinyUrlReq
    {
        [Required, NotNull, Url]
        public string? Url { get; set; }

        [Required, NotNull, EmailAddress]
        public string Username { get; set; }
        public NewTinyUrlReq()
        {

        }


    }
}
