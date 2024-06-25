using System.ComponentModel.DataAnnotations;

namespace MTM.Entities.DTO
{
    public class ResetPasswordModel
    {
        public string? email { get; set; }
        public string? token { get; set; }
        [Required]
        public string? password { get; set; }
        [Required]
        public string? confirmPassword { get; set; }
        [Required]
        public string? oldPassword { get; set; }
    }
}
