namespace MTM.Entities.DTO
{
    public class ResetPasswordModel
    {
        public string? email { get; set; }
        public string? token { get; set; }
        public string? password { get; set; }
        public string? confirmPassword { get; set; }
    }
}
