using e_citaonica_api.Models;

namespace e_citaonica_api.TransferModels
{
    public class DiskusijaZadatakResponse : DiskusijaZadatak
    {
        public bool Zahvaljena { get; set; } = default!;
        public bool Pracena { get; set; } = default!;
        public int BrKomentara { get; set; } = default!;
        public string Tip { get; set; } = "zadatak";
    }
}
