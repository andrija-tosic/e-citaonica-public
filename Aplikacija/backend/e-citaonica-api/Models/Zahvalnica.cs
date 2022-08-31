using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Zahvalnica
{
    [Key]
    public int Id { get; set; }

    [Required]
    public byte Vrednost { get; set; } = 1;

    [JsonIgnore]
    public Korisnik Korisnik { get; set; } = default!;

    [JsonIgnore]
    public Objava Objava { get; set; } = default!;
}