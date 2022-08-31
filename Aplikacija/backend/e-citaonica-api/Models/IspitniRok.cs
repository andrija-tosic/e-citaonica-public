using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class IspitniRok
{

    [Key]
    public int Id { get; set; }

    [Required]
    public string Naziv { get; set; } = default!;

    [JsonIgnore]
    public List<Blanket> Blanketi { get; set; } = default!;
}