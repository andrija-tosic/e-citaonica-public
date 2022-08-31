using System.Text.Json.Serialization;

namespace e_citaonica_api.Models
{
    public class DodatakObjavi : Dodatak
    {
        [JsonIgnore]
        public Objava Objava { get; set; } = default!;

    }
}
