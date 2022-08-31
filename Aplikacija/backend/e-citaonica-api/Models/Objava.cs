using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Objava
{

    [Key]

    public int Id { get; set; }

    [Required]
    public string Sadrzaj { get; set; } = default!;

    [Required]
    public int BrZahvalnica { get; set; }

    [Required]
    public DateTime DatumKreiranja { get; set; }

    [Required]
    public DateTime DatumIzmene { get; set; }

    public bool Arhivirana { get; set; }

    // [JsonIgnore]
    public Korisnik Autor { get; set; } = default!;

    [JsonIgnore]
    public List<Obavestenje> Obavestenja { get; set; } = default!;

    // [JsonIgnore]
    public List<DodatakObjavi> Dodaci { get; set; } = default!;

    // [JsonIgnore]
    public List<Komentar> Komentari { get; set; } = default!;

    [JsonIgnore]
    public List<Zahvalnica> Zahvalnice { get; set; } = default!;

}