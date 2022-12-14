// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using e_citaonica_api.Models;

#nullable disable

namespace e_citaonica_api.Migrations
{
    [DbContext(typeof(ECitaonicaContext))]
    [Migration("20220510170633_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BlanketZadatak", b =>
                {
                    b.Property<int>("BlanketiId")
                        .HasColumnType("int");

                    b.Property<int>("ZadaciId")
                        .HasColumnType("int");

                    b.HasKey("BlanketiId", "ZadaciId");

                    b.HasIndex("ZadaciId");

                    b.ToTable("BlanketZadatak");
                });

            modelBuilder.Entity("DiskusijaKorisnik", b =>
                {
                    b.Property<int>("PraceneDiskusijeId")
                        .HasColumnType("int");

                    b.Property<int>("PratiociId")
                        .HasColumnType("int");

                    b.HasKey("PraceneDiskusijeId", "PratiociId");

                    b.HasIndex("PratiociId");

                    b.ToTable("DiskusijaKorisnik");
                });

            modelBuilder.Entity("DiskusijaOblastOblast", b =>
                {
                    b.Property<int>("DiskusijeId")
                        .HasColumnType("int");

                    b.Property<int>("OblastiId")
                        .HasColumnType("int");

                    b.HasKey("DiskusijeId", "OblastiId");

                    b.HasIndex("OblastiId");

                    b.ToTable("DiskusijaOblastOblast");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Blanket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Datum")
                        .HasColumnType("datetime2");

                    b.Property<int>("IspitniRokId")
                        .HasColumnType("int");

                    b.Property<int>("PredmetId")
                        .HasColumnType("int");

                    b.Property<string>("Tip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IspitniRokId");

                    b.HasIndex("PredmetId");

                    b.ToTable("Blanketi");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Dodatak", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sadrzaj")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Dodaci");

                    b.HasDiscriminator<string>("Tip").HasValue("Dodatak");
                });

            modelBuilder.Entity("e_citaonica_api.Models.IspitniRok", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IspitniRokovi");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Korisnik", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BrZahvalnica")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LozinkaHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PotvrdjenEmail")
                        .HasColumnType("bit");

                    b.Property<string>("SlikaURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Korisnici");

                    b.HasDiscriminator<string>("Tip").HasValue("Korisnik");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Modul", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Moduli");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Obavestenje", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DatumIVreme")
                        .HasColumnType("datetime2");

                    b.Property<int>("KorisnikId")
                        .HasColumnType("int");

                    b.Property<int>("ObjavaId")
                        .HasColumnType("int");

                    b.Property<string>("Sadrzaj")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("KorisnikId");

                    b.HasIndex("ObjavaId");

                    b.ToTable("Obavestenja");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Objava", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Arhivirana")
                        .HasColumnType("bit");

                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<int>("BrZahvalnica")
                        .HasColumnType("int");

                    b.Property<DateTime>("DatumIzmene")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatumKreiranja")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sadrzaj")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AutorId");

                    b.ToTable("Objave");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Objava");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Oblast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PredmetId")
                        .HasColumnType("int");

                    b.Property<int>("RedniBr")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PredmetId");

                    b.ToTable("Oblasti");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Predmet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Godina")
                        .HasColumnType("int");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Opis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Semestar")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Predmeti");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Zadatak", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("BrPoena")
                        .HasColumnType("float");

                    b.Property<int>("RedniBr")
                        .HasColumnType("int");

                    b.Property<string>("Tekst")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Zadaci");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Zahvalnica", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("KorisnikId")
                        .HasColumnType("int");

                    b.Property<int>("ObjavaId")
                        .HasColumnType("int");

                    b.Property<byte>("Vrednost")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("KorisnikId");

                    b.HasIndex("ObjavaId");

                    b.ToTable("Zahvalnice");
                });

            modelBuilder.Entity("KorisnikPredmet", b =>
                {
                    b.Property<int>("PraceniPredmetiId")
                        .HasColumnType("int");

                    b.Property<int>("PratiociId")
                        .HasColumnType("int");

                    b.HasKey("PraceniPredmetiId", "PratiociId");

                    b.HasIndex("PratiociId");

                    b.ToTable("KorisnikPredmet");
                });

            modelBuilder.Entity("ModulPredmet", b =>
                {
                    b.Property<int>("ModuliId")
                        .HasColumnType("int");

                    b.Property<int>("PredmetiId")
                        .HasColumnType("int");

                    b.HasKey("ModuliId", "PredmetiId");

                    b.HasIndex("PredmetiId");

                    b.ToTable("ModulPredmet");
                });

            modelBuilder.Entity("OblastZadatak", b =>
                {
                    b.Property<int>("OblastiId")
                        .HasColumnType("int");

                    b.Property<int>("ZadaciId")
                        .HasColumnType("int");

                    b.HasKey("OblastiId", "ZadaciId");

                    b.HasIndex("ZadaciId");

                    b.ToTable("OblastZadatak");
                });

            modelBuilder.Entity("PredmetProfesor", b =>
                {
                    b.Property<int>("PredmetiId")
                        .HasColumnType("int");

                    b.Property<int>("ProfesoriId")
                        .HasColumnType("int");

                    b.HasKey("PredmetiId", "ProfesoriId");

                    b.HasIndex("ProfesoriId");

                    b.ToTable("PredmetProfesor");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Administrator", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Korisnik");

                    b.HasDiscriminator().HasValue("administrator");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Diskusija", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Objava");

                    b.Property<string>("Naslov")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Zavrsena")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("Diskusija");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DodatakObjavi", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Dodatak");

                    b.Property<int>("ObjavaId")
                        .HasColumnType("int");

                    b.HasIndex("ObjavaId");

                    b.HasDiscriminator().HasValue("dodatak_objavi");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DodatakZadatku", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Dodatak");

                    b.Property<int>("ZadatakId")
                        .HasColumnType("int");

                    b.HasIndex("ZadatakId");

                    b.HasDiscriminator().HasValue("dodatak_zadatku");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Komentar", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Objava");

                    b.Property<int>("ObjavaId")
                        .HasColumnType("int");

                    b.Property<int?>("PotvrdilacResenjaId")
                        .HasColumnType("int");

                    b.Property<bool>("PredlogResenja")
                        .HasColumnType("bit");

                    b.Property<bool>("Prihvacen")
                        .HasColumnType("bit");

                    b.HasIndex("ObjavaId");

                    b.HasIndex("PotvrdilacResenjaId");

                    b.HasDiscriminator().HasValue("Komentar");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Profesor", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Korisnik");

                    b.HasDiscriminator().HasValue("profesor");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Student", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Korisnik");

                    b.Property<int>("Godina")
                        .HasColumnType("int");

                    b.Property<int>("Indeks")
                        .HasColumnType("int");

                    b.Property<int>("ModulId")
                        .HasColumnType("int");

                    b.HasIndex("ModulId");

                    b.HasDiscriminator().HasValue("student");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DiskusijaOblast", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Diskusija");

                    b.HasDiscriminator().HasValue("DiskusijaOblast");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DiskusijaZadatak", b =>
                {
                    b.HasBaseType("e_citaonica_api.Models.Diskusija");

                    b.Property<int>("ZadatakId")
                        .HasColumnType("int");

                    b.HasIndex("ZadatakId");

                    b.HasDiscriminator().HasValue("DiskusijaZadatak");
                });

            modelBuilder.Entity("BlanketZadatak", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Blanket", null)
                        .WithMany()
                        .HasForeignKey("BlanketiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Zadatak", null)
                        .WithMany()
                        .HasForeignKey("ZadaciId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("DiskusijaKorisnik", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Diskusija", null)
                        .WithMany()
                        .HasForeignKey("PraceneDiskusijeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("PratiociId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("DiskusijaOblastOblast", b =>
                {
                    b.HasOne("e_citaonica_api.Models.DiskusijaOblast", null)
                        .WithMany()
                        .HasForeignKey("DiskusijeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Oblast", null)
                        .WithMany()
                        .HasForeignKey("OblastiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("e_citaonica_api.Models.Blanket", b =>
                {
                    b.HasOne("e_citaonica_api.Models.IspitniRok", "IspitniRok")
                        .WithMany("Blanketi")
                        .HasForeignKey("IspitniRokId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Predmet", "Predmet")
                        .WithMany("Blanketi")
                        .HasForeignKey("PredmetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("IspitniRok");

                    b.Navigation("Predmet");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Obavestenje", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Korisnik", "Korisnik")
                        .WithMany("Obavestenja")
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Objava", "Objava")
                        .WithMany("Obavestenja")
                        .HasForeignKey("ObjavaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("Objava");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Objava", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Korisnik", "Autor")
                        .WithMany("Objave")
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Autor");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Oblast", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Predmet", "Predmet")
                        .WithMany("Oblasti")
                        .HasForeignKey("PredmetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Predmet");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Zahvalnica", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Korisnik", "Korisnik")
                        .WithMany("Zahvalnice")
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Objava", "Objava")
                        .WithMany("Zahvalnice")
                        .HasForeignKey("ObjavaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("Objava");
                });

            modelBuilder.Entity("KorisnikPredmet", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Predmet", null)
                        .WithMany()
                        .HasForeignKey("PraceniPredmetiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Korisnik", null)
                        .WithMany()
                        .HasForeignKey("PratiociId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("ModulPredmet", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Modul", null)
                        .WithMany()
                        .HasForeignKey("ModuliId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Predmet", null)
                        .WithMany()
                        .HasForeignKey("PredmetiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("OblastZadatak", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Oblast", null)
                        .WithMany()
                        .HasForeignKey("OblastiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Zadatak", null)
                        .WithMany()
                        .HasForeignKey("ZadaciId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("PredmetProfesor", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Predmet", null)
                        .WithMany()
                        .HasForeignKey("PredmetiId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Profesor", null)
                        .WithMany()
                        .HasForeignKey("ProfesoriId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("e_citaonica_api.Models.DodatakObjavi", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Objava", "Objava")
                        .WithMany("Dodaci")
                        .HasForeignKey("ObjavaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Objava");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DodatakZadatku", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Zadatak", "Zadatak")
                        .WithMany("Dodaci")
                        .HasForeignKey("ZadatakId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Zadatak");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Komentar", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Objava", "Objava")
                        .WithMany("Komentari")
                        .HasForeignKey("ObjavaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("e_citaonica_api.Models.Profesor", "PotvrdilacResenja")
                        .WithMany("PotvrdjenaResenja")
                        .HasForeignKey("PotvrdilacResenjaId");

                    b.Navigation("Objava");

                    b.Navigation("PotvrdilacResenja");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Student", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Modul", "Modul")
                        .WithMany("Studenti")
                        .HasForeignKey("ModulId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Modul");
                });

            modelBuilder.Entity("e_citaonica_api.Models.DiskusijaZadatak", b =>
                {
                    b.HasOne("e_citaonica_api.Models.Zadatak", "Zadatak")
                        .WithMany("Diskusije")
                        .HasForeignKey("ZadatakId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Zadatak");
                });

            modelBuilder.Entity("e_citaonica_api.Models.IspitniRok", b =>
                {
                    b.Navigation("Blanketi");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Korisnik", b =>
                {
                    b.Navigation("Obavestenja");

                    b.Navigation("Objave");

                    b.Navigation("Zahvalnice");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Modul", b =>
                {
                    b.Navigation("Studenti");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Objava", b =>
                {
                    b.Navigation("Dodaci");

                    b.Navigation("Komentari");

                    b.Navigation("Obavestenja");

                    b.Navigation("Zahvalnice");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Predmet", b =>
                {
                    b.Navigation("Blanketi");

                    b.Navigation("Oblasti");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Zadatak", b =>
                {
                    b.Navigation("Diskusije");

                    b.Navigation("Dodaci");
                });

            modelBuilder.Entity("e_citaonica_api.Models.Profesor", b =>
                {
                    b.Navigation("PotvrdjenaResenja");
                });
#pragma warning restore 612, 618
        }
    }
}
