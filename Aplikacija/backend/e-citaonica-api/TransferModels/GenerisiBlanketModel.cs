using e_citaonica_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class GenerisiBlanketModel
    {
        public int[] OblastiIds { get; set; } = default!;
        public string[] Tipovi { get; set; } = default!;
        public int PredmetId { get; set; }
    }
}
