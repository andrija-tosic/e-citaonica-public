using e_citaonica_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class KomentarModel
    {
        public int? Id { get; set; } = default!;
        public int ObjavaId { get; set; } = default!;
        public List<DodatakModel> Dodaci { get; set; } = new();
        public bool PredlogResenja { get; set; } = false;
        public string Sadrzaj { get; set; } = default!;
    }
}
