using System.ComponentModel.DataAnnotations;

namespace VNUK.Dtos.UserDtos.UserInput
{
    public class ChangePasswordInputDto
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
