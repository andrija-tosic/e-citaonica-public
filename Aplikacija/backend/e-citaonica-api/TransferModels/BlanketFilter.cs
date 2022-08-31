using System.ComponentModel.DataAnnotations;

namespace e_citaonica_api.TransferModels;

public class BlanketFilter
{
    public int PredmetId { get; set; } = default;
    public string[] Tipovi { get; set; } = default!;

    public string Tip { get; set; } = default!;
    public int[] RokoviIds { get; set; } = default!;
    public int IspitniRokId { get; set; }
    public int Godina { get; set; } = default!;
    public DateTime Datum { get; set; } = default!;
    public int[] ZadaciIds { get; set; } = default!;
}
