using e_citaonica_api.Models;

namespace e_citaonica_api.Services;
public interface IDeleteService
{
    Task ObrisiOblast(Oblast oblast);
    Task ObrisiPodstablo(Komentar kom);
    Task ObrisiDiskusiju(Diskusija diskusija);
    Task ObrisiBlanket(Blanket blanket);
    Task ObrisiPredmet(Predmet predmet);
    Task ObrisiKorisnika(int id);

}