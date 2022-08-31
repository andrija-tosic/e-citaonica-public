

using e_citaonica_api.Models;

namespace e_citaonica_api.TransferModels
{
    public class DiskusijaModel
    {
        public int? Id { get; set; } = default!;
        public string Naslov { get; set; } = default!;
        public string Sadrzaj { get; set; } = default!;
        public string Tip { get; set; } = default!;
        public int? ZadatakId { get; set; } = default!;
        public List<int>? OblastiIds { get; set; } = new();
        public int AutorId { get; set; } = default!;
        public List<DodatakModel> dodaci { get; set; } = new();
        public List<int> OsobeIds { get; set; } = new();
    }
}
