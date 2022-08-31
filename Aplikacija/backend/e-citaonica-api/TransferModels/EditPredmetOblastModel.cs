using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_citaonica_api.Models;

public class EditPredmetOblastModel
{
    public int Id { get; set; }

    public int RedniBr { get; set; }

    public string Naziv { get; set; } = default!;

}