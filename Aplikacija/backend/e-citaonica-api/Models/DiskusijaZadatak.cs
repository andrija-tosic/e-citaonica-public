using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class DiskusijaZadatak : Diskusija
{
    [Required]
    public Zadatak Zadatak { get; set; } = default!;
}
