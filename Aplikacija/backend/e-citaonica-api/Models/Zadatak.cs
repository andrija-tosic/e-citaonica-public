using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Zadatak
{

    [Key]
    public int Id { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int RedniBr { get; set; }

    [Required]
    public string Tekst { get; set; } = default!;

    [Required]
    public string Tip { get; set; } = default!;

    [Required]
    public double BrPoena { get; set; }

    // [JsonIgnore]
    public List<Oblast> Oblasti { get; set; } = default!;

    [JsonIgnore]
    public List<Blanket> Blanketi { get; set; } = default!;

    [JsonIgnore]
    public List<DodatakZadatku> Dodaci { get; set; } = default!;

    [JsonIgnore]
    public List<DiskusijaZadatak> Diskusije { get; set; } = default!;

}