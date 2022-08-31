using Microsoft.EntityFrameworkCore;

namespace e_citaonica_api.Models
{
    public class ECitaonicaContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>()
                .HasDiscriminator(u => u.Tip)
                .HasValue<Profesor>("profesor")
                .HasValue<Student>("student")
                .HasValue<Administrator>("administrator");

            modelBuilder.Entity<Dodatak>()
                .HasDiscriminator(u => u.Tip)
                .HasValue<DodatakObjavi>("dodatak_objavi")
                .HasValue<DodatakZadatku>("dodatak_zadatku");

            modelBuilder.Entity<Komentar>()
                .HasOne(k => k.Diskusija)
                .WithMany(d => d.KomentariDiskusije);

            modelBuilder.Entity<Komentar>()
                .HasOne(k => k.Objava)
                .WithMany(o => o.Komentari);

            // potrebno za migraciju
            modelBuilder.Entity<Predmet>()
                .HasMany(e => e.Profesori)
                .WithMany(e => e.Predmeti);

            // uklanja on-delete-cascade svuda
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.NoAction;

            //modelBuilder.Entity<Komentar>()
            //    .HasOne(e => e.Objava)
            //    .WithMany(e => e.Komentari)
            //    .OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<DiskusijaOblast>()
            //     .HasOne(e => e.Diskusija)
            //     .WithMany(e => e.Oblasti)
            //     .OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<Obavestenje>()
            //     .HasOne(e => e.Objava)
            //     .WithMany(e => e.Obavestenja)
            //     .OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<Zahvalnica>()
            //     .HasOne(e => e.Objava)
            //     .WithMany(e => e.Zahvalnice)
            //     .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<Korisnik> Korisnici { get; set; } = default!;
        public DbSet<Student> Studenti { get; set; } = default!;
        public DbSet<Profesor> Profesori { get; set; } = default!;
        public DbSet<Administrator> Administratori { get; set; } = default!;
        public DbSet<Blanket> Blanketi { get; set; } = default!;
        public DbSet<Objava> Objave { get; set; } = default!;
        public DbSet<Diskusija> Diskusije { get; set; } = default!;
        public DbSet<DiskusijaZadatak> DiskusijeZaZadatke { get; set; } = default!;
        public DbSet<DiskusijaOblast> DiskusijeZaOblasti { get; set; } = default!;
        public DbSet<Komentar> Komentari { get; set; } = default!;
        public DbSet<Dodatak> Dodaci { get; set; } = default!;
        public DbSet<DodatakObjavi> DodaciObjavi { get; set; } = default!;
        public DbSet<DodatakZadatku> DodaciZadatku { get; set; } = default!;
        public DbSet<IspitniRok> IspitniRokovi { get; set; } = default!;
        public DbSet<Modul> Moduli { get; set; } = default!;
        public DbSet<Obavestenje> Obavestenja { get; set; } = default!;
        public DbSet<Oblast> Oblasti { get; set; } = default!;
        public DbSet<Predmet> Predmeti { get; set; } = default!;
        public DbSet<Zadatak> Zadaci { get; set; } = default!;
        public DbSet<Zahvalnica> Zahvalnice { get; set; } = default!;

        public ECitaonicaContext(DbContextOptions options) : base(options)
        {

        }
    }

}
