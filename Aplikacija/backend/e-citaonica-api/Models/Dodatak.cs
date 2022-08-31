using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public abstract class Dodatak {

    [Key]
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = default!;

    [Required]
    public string Tip { get; set; } = default!;

    [Required]
    public string Sadrzaj { get; set; } = default!;
}