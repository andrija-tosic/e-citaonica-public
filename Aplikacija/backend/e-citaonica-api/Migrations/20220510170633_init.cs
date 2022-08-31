using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_citaonica_api.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IspitniRokovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IspitniRokovi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moduli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moduli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Predmeti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Godina = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semestar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predmeti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zadaci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RedniBr = table.Column<int>(type: "int", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrPoena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zadaci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LozinkaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrZahvalnica = table.Column<int>(type: "int", nullable: false),
                    SlikaURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PotvrdjenEmail = table.Column<bool>(type: "bit", nullable: false),
                    Indeks = table.Column<int>(type: "int", nullable: true),
                    Godina = table.Column<int>(type: "int", nullable: true),
                    ModulId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Korisnici_Moduli_ModulId",
                        column: x => x.ModulId,
                        principalTable: "Moduli",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blanketi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IspitniRokId = table.Column<int>(type: "int", nullable: false),
                    PredmetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blanketi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blanketi_IspitniRokovi_IspitniRokId",
                        column: x => x.IspitniRokId,
                        principalTable: "IspitniRokovi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Blanketi_Predmeti_PredmetId",
                        column: x => x.PredmetId,
                        principalTable: "Predmeti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModulPredmet",
                columns: table => new
                {
                    ModuliId = table.Column<int>(type: "int", nullable: false),
                    PredmetiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulPredmet", x => new { x.ModuliId, x.PredmetiId });
                    table.ForeignKey(
                        name: "FK_ModulPredmet_Moduli_ModuliId",
                        column: x => x.ModuliId,
                        principalTable: "Moduli",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModulPredmet_Predmeti_PredmetiId",
                        column: x => x.PredmetiId,
                        principalTable: "Predmeti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Oblasti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RedniBr = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PredmetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oblasti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oblasti_Predmeti_PredmetId",
                        column: x => x.PredmetId,
                        principalTable: "Predmeti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KorisnikPredmet",
                columns: table => new
                {
                    PraceniPredmetiId = table.Column<int>(type: "int", nullable: false),
                    PratiociId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikPredmet", x => new { x.PraceniPredmetiId, x.PratiociId });
                    table.ForeignKey(
                        name: "FK_KorisnikPredmet_Korisnici_PratiociId",
                        column: x => x.PratiociId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KorisnikPredmet_Predmeti_PraceniPredmetiId",
                        column: x => x.PraceniPredmetiId,
                        principalTable: "Predmeti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Objave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sadrzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrZahvalnica = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumIzmene = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arhivirana = table.Column<bool>(type: "bit", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zavrsena = table.Column<bool>(type: "bit", nullable: true),
                    ZadatakId = table.Column<int>(type: "int", nullable: true),
                    PredlogResenja = table.Column<bool>(type: "bit", nullable: true),
                    Prihvacen = table.Column<bool>(type: "bit", nullable: true),
                    ObjavaId = table.Column<int>(type: "int", nullable: true),
                    PotvrdilacResenjaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objave_Korisnici_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Objave_Korisnici_PotvrdilacResenjaId",
                        column: x => x.PotvrdilacResenjaId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Objave_Objave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Objave_Zadaci_ZadatakId",
                        column: x => x.ZadatakId,
                        principalTable: "Zadaci",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PredmetProfesor",
                columns: table => new
                {
                    PredmetiId = table.Column<int>(type: "int", nullable: false),
                    ProfesoriId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredmetProfesor", x => new { x.PredmetiId, x.ProfesoriId });
                    table.ForeignKey(
                        name: "FK_PredmetProfesor_Korisnici_ProfesoriId",
                        column: x => x.ProfesoriId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PredmetProfesor_Predmeti_PredmetiId",
                        column: x => x.PredmetiId,
                        principalTable: "Predmeti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlanketZadatak",
                columns: table => new
                {
                    BlanketiId = table.Column<int>(type: "int", nullable: false),
                    ZadaciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlanketZadatak", x => new { x.BlanketiId, x.ZadaciId });
                    table.ForeignKey(
                        name: "FK_BlanketZadatak_Blanketi_BlanketiId",
                        column: x => x.BlanketiId,
                        principalTable: "Blanketi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlanketZadatak_Zadaci_ZadaciId",
                        column: x => x.ZadaciId,
                        principalTable: "Zadaci",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OblastZadatak",
                columns: table => new
                {
                    OblastiId = table.Column<int>(type: "int", nullable: false),
                    ZadaciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OblastZadatak", x => new { x.OblastiId, x.ZadaciId });
                    table.ForeignKey(
                        name: "FK_OblastZadatak_Oblasti_OblastiId",
                        column: x => x.OblastiId,
                        principalTable: "Oblasti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OblastZadatak_Zadaci_ZadaciId",
                        column: x => x.ZadaciId,
                        principalTable: "Zadaci",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiskusijaKorisnik",
                columns: table => new
                {
                    PraceneDiskusijeId = table.Column<int>(type: "int", nullable: false),
                    PratiociId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskusijaKorisnik", x => new { x.PraceneDiskusijeId, x.PratiociId });
                    table.ForeignKey(
                        name: "FK_DiskusijaKorisnik_Korisnici_PratiociId",
                        column: x => x.PratiociId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiskusijaKorisnik_Objave_PraceneDiskusijeId",
                        column: x => x.PraceneDiskusijeId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiskusijaOblastOblast",
                columns: table => new
                {
                    DiskusijeId = table.Column<int>(type: "int", nullable: false),
                    OblastiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskusijaOblastOblast", x => new { x.DiskusijeId, x.OblastiId });
                    table.ForeignKey(
                        name: "FK_DiskusijaOblastOblast_Objave_DiskusijeId",
                        column: x => x.DiskusijeId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiskusijaOblastOblast_Oblasti_OblastiId",
                        column: x => x.OblastiId,
                        principalTable: "Oblasti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dodaci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjavaId = table.Column<int>(type: "int", nullable: true),
                    ZadatakId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dodaci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dodaci_Objave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dodaci_Zadaci_ZadatakId",
                        column: x => x.ZadatakId,
                        principalTable: "Zadaci",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Obavestenja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sadrzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumIVreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    ObjavaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obavestenja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obavestenja_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Obavestenja_Objave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zahvalnice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vrednost = table.Column<byte>(type: "tinyint", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    ObjavaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zahvalnice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zahvalnice_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zahvalnice_Objave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "Objave",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blanketi_IspitniRokId",
                table: "Blanketi",
                column: "IspitniRokId");

            migrationBuilder.CreateIndex(
                name: "IX_Blanketi_PredmetId",
                table: "Blanketi",
                column: "PredmetId");

            migrationBuilder.CreateIndex(
                name: "IX_BlanketZadatak_ZadaciId",
                table: "BlanketZadatak",
                column: "ZadaciId");

            migrationBuilder.CreateIndex(
                name: "IX_DiskusijaKorisnik_PratiociId",
                table: "DiskusijaKorisnik",
                column: "PratiociId");

            migrationBuilder.CreateIndex(
                name: "IX_DiskusijaOblastOblast_OblastiId",
                table: "DiskusijaOblastOblast",
                column: "OblastiId");

            migrationBuilder.CreateIndex(
                name: "IX_Dodaci_ObjavaId",
                table: "Dodaci",
                column: "ObjavaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dodaci_ZadatakId",
                table: "Dodaci",
                column: "ZadatakId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_ModulId",
                table: "Korisnici",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikPredmet_PratiociId",
                table: "KorisnikPredmet",
                column: "PratiociId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulPredmet_PredmetiId",
                table: "ModulPredmet",
                column: "PredmetiId");

            migrationBuilder.CreateIndex(
                name: "IX_Obavestenja_KorisnikId",
                table: "Obavestenja",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Obavestenja_ObjavaId",
                table: "Obavestenja",
                column: "ObjavaId");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_AutorId",
                table: "Objave",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_ObjavaId",
                table: "Objave",
                column: "ObjavaId");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_PotvrdilacResenjaId",
                table: "Objave",
                column: "PotvrdilacResenjaId");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_ZadatakId",
                table: "Objave",
                column: "ZadatakId");

            migrationBuilder.CreateIndex(
                name: "IX_Oblasti_PredmetId",
                table: "Oblasti",
                column: "PredmetId");

            migrationBuilder.CreateIndex(
                name: "IX_OblastZadatak_ZadaciId",
                table: "OblastZadatak",
                column: "ZadaciId");

            migrationBuilder.CreateIndex(
                name: "IX_PredmetProfesor_ProfesoriId",
                table: "PredmetProfesor",
                column: "ProfesoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Zahvalnice_KorisnikId",
                table: "Zahvalnice",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zahvalnice_ObjavaId",
                table: "Zahvalnice",
                column: "ObjavaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlanketZadatak");

            migrationBuilder.DropTable(
                name: "DiskusijaKorisnik");

            migrationBuilder.DropTable(
                name: "DiskusijaOblastOblast");

            migrationBuilder.DropTable(
                name: "Dodaci");

            migrationBuilder.DropTable(
                name: "KorisnikPredmet");

            migrationBuilder.DropTable(
                name: "ModulPredmet");

            migrationBuilder.DropTable(
                name: "Obavestenja");

            migrationBuilder.DropTable(
                name: "OblastZadatak");

            migrationBuilder.DropTable(
                name: "PredmetProfesor");

            migrationBuilder.DropTable(
                name: "Zahvalnice");

            migrationBuilder.DropTable(
                name: "Blanketi");

            migrationBuilder.DropTable(
                name: "Oblasti");

            migrationBuilder.DropTable(
                name: "Objave");

            migrationBuilder.DropTable(
                name: "IspitniRokovi");

            migrationBuilder.DropTable(
                name: "Predmeti");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "Zadaci");

            migrationBuilder.DropTable(
                name: "Moduli");
        }
    }
}
