using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Blanket
{

    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Datum { get; set; }

    [Required]
    public string Tip { get; set; } = default!;

    // [JsonIgnore]
    public IspitniRok IspitniRok { get; set; } = default!;

    // [JsonIgnore]
    public Predmet Predmet { get; set; } = default!;

    // [JsonIgnore]
    public List<Zadatak> Zadaci { get; set; } = default!;
}