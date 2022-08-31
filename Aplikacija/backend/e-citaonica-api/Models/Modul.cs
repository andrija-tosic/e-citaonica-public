using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class Modul
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = default!;

    [JsonIgnore]
    public List<Predmet> Predmeti { get; set; } = default!;

    [JsonIgnore]
    public List<Student> Studenti { get; set; } = default!;
}