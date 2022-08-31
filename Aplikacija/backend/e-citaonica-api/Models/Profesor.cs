using System.Text.Json.Serialization;

namespace e_citaonica_api.Models
{
    public class Profesor : Korisnik
    {
        [JsonIgnore]
        public List<Komentar> PotvrdjenaResenja { get; set; } = default!;

        [JsonIgnore]
        public List<Predmet> Predmeti { get; set; } = default!;
    }
}