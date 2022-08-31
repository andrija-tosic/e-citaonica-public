using e_citaonica_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class PredmetModel
    {
        public int Id { get; set; }
        public int Godina { get; set; }
        public string Naziv { get; set; } = default!;
        public string Opis { get; set; } = default!;
        public int Semestar { get; set; }
        public int[] ModuliIds { get; set; } = default!;
        public int[] ProfesoriIds { get; set; } = default!;
        public List<EditPredmetOblastModel> Oblasti { get; set; } = default!;
    }
}
