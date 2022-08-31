using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models
{
    public class Komentar : Objava
    {
        public bool PredlogResenja { get; set; }
        public bool Prihvacen { get; set; } = false;

        [Required]
        [JsonIgnore]
        public Objava Objava { get; set; } = default!;
        [JsonIgnore]
        public Diskusija? Diskusija { get; set; } = default!;
        //[JsonIgnore]
        public Profesor? PotvrdilacResenja { get; set; }
        [NotMapped]
        public bool Zahvaljena { get; set; } = false;
    }
}