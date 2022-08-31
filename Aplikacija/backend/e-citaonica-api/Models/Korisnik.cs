using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public abstract class Korisnik
{

    [Key]
    public int Id { get; set; }

    [Required]

    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [JsonIgnore]
    public string LozinkaHash { get; set; } = default!;

    [Required]
    public string Ime { get; set; } = default!;

    [Required]
    [Range(0, int.MaxValue)]
    public int BrZahvalnica { get; set; }

    [Required]
    public string SlikaURL { get; set; } = default!;

    [Required]
    public string Tip { get; set; } = default!;

    [Required]
    public bool PotvrdjenEmail { get; set; } = false;

    [JsonIgnore]
    public List<Obavestenje> Obavestenja { get; set; } = default!;

    [JsonIgnore]
    public List<Zahvalnica> Zahvalnice { get; set; } = default!;

    [JsonIgnore]
    public List<Predmet> PraceniPredmeti { get; set; } = default!;

    [JsonIgnore]
    public List<Diskusija> PraceneDiskusije { get; set; } = default!;

    [JsonIgnore]
    public List<Objava> Objave { get; set; } = default!;

}