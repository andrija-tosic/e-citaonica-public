using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public abstract class Diskusija : Objava
{
    [Required]
    public string Naslov { get; set; } = default!;

    [Required]
    public bool Zavrsena { get; set; } = false;

    public Predmet? Predmet { get; set; } = default!;
    [JsonIgnore]
    public List<Korisnik> Pratioci { get; set; } = default!;
    [JsonIgnore]
    public List<Komentar> KomentariDiskusije { get; set; } = default!;

}
