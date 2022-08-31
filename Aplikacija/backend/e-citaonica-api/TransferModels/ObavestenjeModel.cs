using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class ObavestenjeModel
    {

        public int Id { get; set; }

        public int ObjavaId { get; set; }

        public string Sadrzaj { get; set; } = default!;

        public int KorisnikId { get; set; }
    }
}
