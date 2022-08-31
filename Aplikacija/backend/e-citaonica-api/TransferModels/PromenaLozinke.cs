namespace e_citaonica_api.Models
{
    public class PromenaLozinke
    {
        public string oldPassword { get; set; } = default!;
        public string newPassword { get; set; } = default!;
        public string newPasswordAgain { get; set; } = default!;
    }
}
