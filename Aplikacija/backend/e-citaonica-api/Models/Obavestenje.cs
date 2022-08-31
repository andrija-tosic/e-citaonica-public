using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;
public class Obavestenje
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Sadrzaj { get; set; } = default!;

    [Required]
    public DateTime DatumIVreme { get; set; }

    [JsonIgnore]
    public Korisnik Korisnik { get; set; } = default!;

    [JsonIgnore]
    public Objava Objava { get; set; } = default!;
}