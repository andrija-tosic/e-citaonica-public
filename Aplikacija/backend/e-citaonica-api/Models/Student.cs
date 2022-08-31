using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models
{
    public class Student : Korisnik
    {
        [Required]
        public int Indeks { get; set; }

        [Required]
        public int Godina { get; set; }

        [JsonIgnore]
        public Modul Modul { get; set; } = default!;
    }
}