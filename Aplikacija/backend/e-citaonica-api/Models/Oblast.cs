using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Oblast
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int RedniBr { get; set; }

    [Required]
    public string Naziv { get; set; } = default!;

    [JsonIgnore]
    public Predmet Predmet { get; set; } = default!;

    [JsonIgnore]
    public List<Zadatak> Zadaci { get; set; } = default!;

    [JsonIgnore]
    public List<DiskusijaOblast> Diskusije { get; set; } = default!;
}