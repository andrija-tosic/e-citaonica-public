using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class ZadatakModel
    {
        public int Id { get; set; }
        public int RedniBr { get; set; }
        public string Tekst { get; set; } = default!;

        public double BrPoena { get; set; } = default!;

        public string Tip { get; set; } = default!;
        public int[] OblastiIds { get; set; } = default!;
    }
}
