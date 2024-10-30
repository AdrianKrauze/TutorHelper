namespace TutorHelper.Models.IdentityModels
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}