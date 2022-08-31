using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Predmet
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = default!;

    [Required]
    public int Godina { get; set; }

    [Required]
    public string Opis { get; set; } = default!;

    [Required]
    public int Semestar { get; set; } = default!;

    [JsonIgnore]
    public List<Oblast> Oblasti { get; set; } = default!;

    [JsonIgnore]
    public List<Blanket> Blanketi { get; set; } = default!;

    [JsonIgnore]
    public List<Modul> Moduli { get; set; } = default!;

    [JsonIgnore]
    public List<Profesor> Profesori { get; set; } = default!;

    [JsonIgnore]
    public List<Korisnik> Pratioci { get; set; } = default!;
    [JsonIgnore]
    public List<Diskusija> Diskusije { get; set; } = default!; 
}