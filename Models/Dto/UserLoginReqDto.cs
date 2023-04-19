using System.ComponentModel.DataAnnotations;

namespace TinyUrl.Models.Dto
{
    public class UserLoginReqDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
