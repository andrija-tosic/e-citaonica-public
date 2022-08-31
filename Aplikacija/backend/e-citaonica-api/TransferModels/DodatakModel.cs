using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class DodatakModel
    {

        public int Id { get; set; } = default;

        public string Sadrzaj { get; set; } = default!;

        public string Naziv { get; set; } = default!;
    }
}
