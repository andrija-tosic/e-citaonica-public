using System.Text.Json.Serialization;

namespace e_citaonica_api.Models
{
    public class DodatakZadatku : Dodatak
    {
        [JsonIgnore]
        public Zadatak Zadatak { get; set; } = default!;

    }
}
