using e_citaonica_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class BlanketModel
    {
        public int Id { get; set; }

        public DateTime Datum { get; set; } = default!;
        public string Tip { get; set; } = default!;

        public int IspitniRokId { get; set; }

        public int PredmetId { get; set; }

        public List<ZadatakModel> Zadaci { get; set; } = new List<ZadatakModel>();
    }
}
