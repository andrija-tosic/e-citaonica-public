using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class DiskusijaOblast : Diskusija
{
    [Required]
    public List<Oblast> Oblasti { get; set; } = default!;
}