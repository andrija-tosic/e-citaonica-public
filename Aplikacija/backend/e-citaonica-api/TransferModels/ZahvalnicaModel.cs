using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_citaonica_api.TransferModels
{
    public class ZahvalnicaModel
    {

        public int Id { get; set; }

        public byte Vrednost { get; set; }

        public int KorisnikId { get; set; }

        public int ObjavaId { get; set; }
    }
}
