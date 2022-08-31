using System.ComponentModel.DataAnnotations;

namespace e_citaonica_api.TransferModels
{
    public class ConfirmRegister
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = default!;
    }
}
