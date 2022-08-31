using e_citaonica_api.Models;
using Microsoft.EntityFrameworkCore;


namespace e_citaonica_api.Services;
public class DeleteService : IDeleteService
{
    private ECitaonicaContext _context;
    private IFileService _fileService;
    public DeleteService(ECitaonicaContext _context, IFileService fileService)
    {
        this._context = _context;
        this._fileService = fileService;
    }

    public async Task ObrisiKorisnika(int id)
    {
        Korisnik delete = await _context.Korisnici.Where(k => k.Id == id)
            .Include(k => k.PraceneDiskusije)
            .Include(k => k.PraceniPredmeti)
            .Include(k => k.Zahvalnice)
            .Include(k => k.Obavestenja)
            .Include(k => (k as Profesor).PotvrdjenaResenja)
            .Include(k => (k as Profesor).Predmeti)
            .Include(k => k.Objave).ThenInclude(o => o.Obavestenja)
            .Include(k => k.Objave).ThenInclude(o => o.Zahvalnice)
            .Include(k => k.Objave).ThenInclude(o => o.Dodaci)
            .Include(k => k.Objave).ThenInclude(o => (o as Diskusija).Pratioci)
            .Include(k => k.Objave).ThenInclude(o => (o as Diskusija).Komentari)
            .Include(k => k.Objave).ThenInclude(o => (o as Diskusija).Komentari).ThenInclude(k => k.Obavestenja)
            .Include(k => k.Objave).ThenInclude(o => (o as Diskusija).Komentari).ThenInclude(k => k.Zahvalnice)
            .Include(k => k.Objave).ThenInclude(o => (o as Diskusija).Komentari).ThenInclude(k => k.Dodaci)
            .Include(k => k.Objave).ThenInclude(o => (o as DiskusijaOblast).Oblasti)
            .FirstAsync();

        _context.RemoveRange(delete.Obavestenja);
        _context.RemoveRange(delete.Zahvalnice);
        foreach (Objava o in delete.Objave)
            await this.ObrisiObjavu(o);
        delete.PraceneDiskusije.Clear();
        delete.PraceniPredmeti.Clear();
        _fileService.DeleteFile(delete.SlikaURL);
        if (delete is Profesor)
        {
            (delete as Profesor)?.Predmeti.Clear();
            (delete as Profesor)?.PotvrdjenaResenja.Clear();
        }
        _context.Korisnici.Remove(delete);
    }

    public async Task ObrisiPodstablo(Komentar kom)
    {
        var children = await _context.Komentari
            .Where(k => k.Objava == kom)
            .Include(k => k.Zahvalnice)
            .Include(k => k.Dodaci)
            .Include(k => k.Obavestenja)
            .ToListAsync();
        foreach (var child in children)
        {
            await ObrisiPodstablo(child);
        }

        _context.Zahvalnice.RemoveRange(kom.Zahvalnice);
        _context.Obavestenja.RemoveRange(kom.Obavestenja);
        kom.Dodaci.ForEach(d => _fileService.DeleteFileNoAwait(d.Sadrzaj));
        _context.Dodaci.RemoveRange(kom.Dodaci);
        _context.Komentari.Remove(kom);
    }

    public async Task ObrisiDiskusiju(Diskusija diskusija)
    {
        if (diskusija != null)
        {
            _context.Zahvalnice.RemoveRange(diskusija.Zahvalnice);
            diskusija.Dodaci.ForEach(d => _fileService.DeleteFileNoAwait(d.Sadrzaj));
            _context.Dodaci.RemoveRange(diskusija.Dodaci);
            _context.Obavestenja.RemoveRange(diskusija.Obavestenja);
            diskusija.Pratioci.Clear();
            if (diskusija is DiskusijaOblast) (diskusija as DiskusijaOblast)?.Oblasti.Clear();

            foreach (var k in diskusija.Komentari)
            {
                await this.ObrisiPodstablo(k);
            }

            _context.Diskusije.Remove(diskusija);

        }
    }

    public async Task ObrisiBlanket(Blanket blanket)
    {
        // foreach (var zadatak in blanket.Zadaci.ToList())
        // {
        //     zadatak.Blanketi.Clear();
        //     zadatak.Oblasti.Clear();

        // foreach (var diskusija in zadatak.Diskusije)
        // {
        //     await this.ObrisiDiskusiju(diskusija);
        // }

        // blanket.Zadaci.Remove(zadatak);
        // }

        blanket.Zadaci.Clear();
        _context.Blanketi.Remove(blanket);
    }

    public async Task ObrisiPredmet(Predmet predmet)
    {
        predmet.Moduli.Clear();
        predmet.Profesori.Clear();
        predmet.Pratioci.Clear();

        foreach (Diskusija dis in predmet.Diskusije.ToList())
        {
            await this.ObrisiDiskusiju(dis);
        }

        foreach (Blanket b in predmet.Blanketi.ToList())
        {
            await this.ObrisiBlanket(b);
        }


        foreach (var oblast in predmet.Oblasti.ToList())
        {
            foreach (var zadatak in oblast.Zadaci.ToList())
            {
                await this.ObrisiZadatak(zadatak);
            }

            await this.ObrisiOblast(oblast);
        }

        _context.Predmeti.Remove(predmet);
    }

    public async Task ObrisiOblast(Oblast oblast)
    {
        foreach (var diskusija in oblast.Diskusije.ToList())
        {
            await this.ObrisiDiskusiju(diskusija);
        }

        foreach (var zadatak in oblast.Zadaci.ToList())
        {
            await this.ObrisiZadatak(zadatak);
        }

        _context.Oblasti.Remove(oblast);
    }

    public async Task ObrisiZadatak(Zadatak zadatak)
    {
        zadatak.Oblasti.Clear();

        var blanketi = zadatak.Blanketi.ToList();
        zadatak.Blanketi.Clear();

        foreach (var blanket in blanketi)
        {
            // ako je jedini preostali zadatak, obrisati blanket
            if (blanket.Zadaci.Count == 1)
            {
                _context.Blanketi.Remove(blanket);
            }
        }

        foreach (var diskusija in zadatak.Diskusije.ToList())
        {
            await this.ObrisiDiskusiju(diskusija);
        }

        _context.DodaciZadatku.RemoveRange(zadatak.Dodaci);
        _context.Zadaci.Remove(zadatak);
    }

    public async Task ObrisiObjavu(Objava o)
    {
        if (o is Komentar)
        {
            await this.ObrisiPodstablo(o as Komentar);
        }
        else
        {
            await this.ObrisiDiskusiju(o as Diskusija);
        }
    }
}