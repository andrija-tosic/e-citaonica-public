import { Title } from '@angular/platform-browser';
import { AUTORIZACIJA_DISABLED } from './../../constants';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { BehaviorSubject, Observable, of, ReplaySubject, switchMap } from 'rxjs';
import { Diskusija } from 'src/app/models/diskusija.model';
import { DiskusijaAdd } from 'src/app/models/DTOs/diskusija.add.dto';
import { Komentar } from 'src/app/models/komentar.model';
import { Predmet } from 'src/app/models/predmet.model';
import { UserBasic } from 'src/app/models/user-basic.model';
import { AuthService } from 'src/app/services/auth.service';
import { PredmetService } from 'src/app/services/predmet.service';
import { ActiveCommentInterface } from 'src/app/types/activeCommentInterface.type';
import { ActivatedRoute, NavigationEnd, Router, RouterModule } from '@angular/router';
import { DiskusijaService } from 'src/app/services/diskusija.service';
import { apiError } from 'src/app/types/apiError.type';
import { ApiMsg } from 'src/app/types/apiMsg.type';
import { DiskusijaEditRes } from 'src/app/models/DTOs/diskusija.edit.res';
import { MatSnackBar } from '@angular/material/snack-bar';
import { constants } from 'src/app/constants';
import { DeleteConfirmationDialogComponent } from 'src/app/components/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { PrijaviDialogComponent } from 'src/app/components/prijavi-dialog/prijavi-dialog.component';

@Component({
  selector: 'diskusija',
  templateUrl: './diskusija.component.html',
  styleUrls: ['./diskusija.component.scss']
})
export class DiskusijaComponent implements OnInit, AfterViewInit {
  commentRef: ElementRef;
  @ViewChildren("commentRef") commentRefs: QueryList<ElementRef>;
  diskusija: Diskusija;
  isEditing: boolean = false;
  diskusijaId: number = 0;
  scrollAnchor: string;

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  public activeComment$: BehaviorSubject<ActiveCommentInterface | null> =
    new BehaviorSubject<ActiveCommentInterface | null>(null);
  user$: BehaviorSubject<UserBasic | null>;
  predmet: Predmet | null;
  diskusijaLoaded: boolean = false;

  constructor(
    private authService: AuthService,
    private predmetService: PredmetService,
    private diskusijaService: DiskusijaService,
    private route: ActivatedRoute,
    private _snackbar: MatSnackBar,
    private router: Router,
    private dialog: MatDialog,
    private title: Title) {

    this.predmetService.predmet$.subscribe(p => this.predmet = p);

    this.scrollAnchor = (this.router.getCurrentNavigation()?.extras.state as { scrollAnchor: string })?.scrollAnchor;
  }

  profesorPredmeta(): boolean {
    return this.authService.user.tip === 'profesor'
      && this.predmet?.profesori?.map(p => p.id).includes(this.authService.user.id) || false;
  }

  ngAfterViewInit(): void {
    this.commentRefs.changes.subscribe((res) => {
      this.commentRef = res.first;
      if (this.scrollAnchor == "comment") {
        this.commentRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
        setTimeout(_ => this.scrollAnchor = "-1", 0);
      }
    })
  }

  scrollToForm() {
    this.commentRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
  }

  private sortPrvoPotvrdjenaIPrihvacena() {
    return function (a: Komentar, b: Komentar) {
      if (a === b) {
        return 0;
      }
      else if (a.potvrdilacResenja && b.potvrdilacResenja) {
        return new Date(b.datumKreiranja).getTime() - new Date(a.datumKreiranja).getTime();
      }
      else if (a.potvrdilacResenja || b.potvrdilacResenja) {
        return a.potvrdilacResenja ? -1 : 1;
      }
      else if (a.prihvacen && b.prihvacen) {
        return new Date(b.datumKreiranja).getTime() - new Date(a.datumKreiranja).getTime();
      }
      else if (a.prihvacen || b.prihvacen) {
        return a.prihvacen ? -1 : 1;
      }
      else {
        return new Date(b.datumKreiranja).getTime() - new Date(a.datumKreiranja).getTime();
      }
    };
  }

  ngOnInit(): void {
    this.route.paramMap
      .pipe(switchMap((params) => {
        return this.diskusijaService.getDiskusija(Number(params.get('id')));
      }))
      .subscribe({
        next: (diskusija: Diskusija) => {
          this.title.setTitle(`${diskusija.naslov} • e-Čitaonica`);

          this.diskusijaLoaded = true;
          this.diskusija = diskusija;
          this.diskusija.komentari?.sort(this.sortPrvoPotvrdjenaIPrihvacena());
          console.log(this.diskusija.komentari);
        },
        error: () => this.diskusijaLoaded = true
      });
  }

  onRootCommentSubmit(komentar: Komentar) {
    this.diskusijaService.postKomentar({ ...komentar, objavaId: this.diskusija.id })
      .subscribe({
        next: (komentar) => {
          if (this.diskusija && !this.diskusija.komentari) this.diskusija.komentari = [];
          this.diskusija.komentari?.push(komentar);
          this.diskusija.komentari?.sort(this.sortPrvoPotvrdjenaIPrihvacena());
        },
        error: (err: apiError) => console.log(err.error)
      });
  }

  togglePracena() {
    this.diskusija.pracena = !this.diskusija.pracena;

    const pracenjeText = this.diskusija.pracena ? "Dobićete obaveštenja o promeni diskusije." : "Nećete više dobijati obaveštenja o promeni diskusije.";

    this.diskusijaService.zapratiToggle(this.diskusija?.id || 0)
      .subscribe({
        next: msg => {
          this._snackbar.open(pracenjeText, 'OK', {
            ...constants.snackbarPosition,
            duration: 3000,
          });
        },
        error: _ => {
          this.diskusija.pracena = !this.diskusija.pracena;
        }
      })
  }

  toggleZahvaljena() {
    if (!this.diskusija) return;
    this.diskusija.zahvaljena = !this.diskusija?.zahvaljena;
    this.diskusija?.zahvaljena ? this.diskusija.brZahvalnica++ : this.diskusija.brZahvalnica--;

    this.diskusijaService.zahvaliToggle(this.diskusija?.id || 0).subscribe({
    });
  }

  onIzmeniClick() {
    this.isEditing = !this.isEditing;
  }

  onChildDelete(komentar: Komentar) {
    if (!komentar.id) return;
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, { data: { title: "Da li ste sigurni da želite da obrišete komentar?" } });
    dialogRef.afterClosed().subscribe(res => {
      if (!res || !komentar.id) return;
      this.diskusijaService.deleteKomentar(komentar.id)
        .subscribe({
          next: (msg) => {
            this._snackbar.open('Komentar obrisan.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
            this.diskusija.komentari = this.diskusija.komentari?.filter(k => k.id != komentar.id);
          },
          error: (msg) => {
            console.log(msg);
            this._snackbar.open('Došlo je do greške.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
          }
        })
    })
  }

  onIzmeniSubmit(diskusija: DiskusijaAdd) {
    console.log(JSON.stringify(diskusija));
    this.diskusijaService.updateDiskusija(diskusija).subscribe({
      next: (res: DiskusijaEditRes) => {
        this.diskusija.naslov = diskusija.naslov;
        this.diskusija.datumIzmene = res.datumIzmene;
        this.diskusija.sadrzaj = diskusija.sadrzaj;
        if (diskusija.tip == "oblast")
          this.diskusija.oblasti = res.oblasti;
        this.diskusija.dodaci = res.dodaci;

        this.isEditing = false;
        this._snackbar.open("Uspešno izmenjena diskusija.", 'OK',
          {
            ...constants.snackbarPosition,
            duration: 3000,
          });
      },
      error: (res: ApiMsg) => {
        console.log(res.msg)
      }
    })
  }

  onDeleteClick() {
    if (!this.diskusija.id) return;
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      data: { title: "Da li ste sigurni da želite da obrišete diskusiju?" }
    });
    dialogRef.afterClosed().subscribe(res => {
      if (!res || !this.diskusija.id) return;
      this.diskusijaService.deleteDiskusija(this.diskusija.id)
        .subscribe({
          next: _ => {
            this._snackbar.open('Diskusija obrisana.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
            this.router.navigate(['../'], { relativeTo: this.route });
          },
          error: (msg) => {
            console.log(msg);
            this._snackbar.open('Došlo je do greške.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
          }
        })
    })
  }

  onPrijaviClick() {
    this.dialog.open(PrijaviDialogComponent, {
      maxWidth: '500px',
      width: '90%'
    }).afterClosed()
      .subscribe((res) => {
        if (!res || !this.diskusija?.id) return;
        this.diskusijaService.prijaviObjavu(this.diskusija.id, res.tekst)
          .subscribe({
            next: (res) => this._snackbar.open('Diskusija prijavljena.', 'OK', { ...constants.snackbarPosition, duration: 3000 }),
            error: (err) => this._snackbar.open('Došlo je do greške, neuspešna prijava.', 'OK', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] })
          });
      })
  }

  onArhivirajClick() {
    if (!this.diskusija.id) return;
    this.diskusijaService.arhiviraj(this.diskusija.id).subscribe({
      next: (res) => {
        this.diskusija.arhivirana = !this.diskusija.arhivirana;
        const msg = this.diskusija.arhivirana ? "Diskusija arhivirana" : "Diskusija odarhivirana";
        this._snackbar.open(msg, 'OK', { ...constants.snackbarPosition, duration: 3000 });
      },
      error: (err) => {
        this._snackbar.open('Došlo je do greške', 'OK', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] })
      }
    });
  }
}
