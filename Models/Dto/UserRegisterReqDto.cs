using System.ComponentModel.DataAnnotations;

namespace TinyUrl.Models.Dto
{
    public class UserRegisterReqDto
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
