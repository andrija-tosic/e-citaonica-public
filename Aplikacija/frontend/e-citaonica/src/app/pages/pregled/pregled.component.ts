import { Title } from '@angular/platform-browser';
import { AUTORIZACIJA_DISABLED } from './../../constants';
import { PredmetEdit } from './../../models/DTOs/predmet.edit.dto';
import { ComponentCanDeactivate } from './../../guards/pending-changes.guard';
import { Profesor } from './../../models/profesor.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './../../services/auth.service';
import { UserBasic } from './../../models/user-basic.model';
import { BreakpointObserver } from '@angular/cdk/layout';
import { DeleteConfirmationDialogComponent } from '../../components/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { Predmet } from './../../models/predmet.model';
import { Oblast } from './../../models/oblast.model';
import { Observable, BehaviorSubject, first, map, shareReplay, Subject, ReplaySubject } from 'rxjs';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { constants } from '../../constants';
import { ApiMsg } from "../../types/apiMsg.type";
import { PredmetService } from './../../services/predmet.service';
import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, CanDeactivate, Router, NavigationStart, NavigationEnd } from '@angular/router';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from 'src/app/services/user.service';
import { apiError } from 'src/app/types/apiError.type';

@Component({
  selector: 'pregled',
  templateUrl: './pregled.component.html',
  styleUrls: ['./pregled.component.scss']
})


export class PregledComponent implements OnInit, ComponentCanDeactivate {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  user: UserBasic | null;

  constructor(private predmetService: PredmetService, private router: Router,
    private breakpointObserver: BreakpointObserver,
    public userService: UserService,
    private authService: AuthService,
    private _snackbar: MatSnackBar,
    private dialog: MatDialog,
    private title: Title) {
  }
  canDeactivate(): boolean | Observable<boolean> {
    return !this.editing;
  }

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  readonly separatorKeysCodes = [ENTER, COMMA] as const;

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);
  ostaliProfesori$: BehaviorSubject<Profesor[] | null> = new BehaviorSubject<Profesor[] | null>(null);

  predmetniProfesori: Profesor[];
  ostaliProfesori: Profesor[];

  editing: boolean = false;

  ngOnInit(): void {
    this.editing = false;
    this.predmetService.getPredmet().subscribe({
      next: p => {
        if (p)
          this.title.setTitle(`${p.naziv} • e-Čitaonica`);
        this.predmet$.next(p);
      }
    });

    this.authService.getUserObserver()
      .subscribe((user) => {
        this.user = user;
      });

    this.router.events.subscribe((val) => {
      if (val instanceof NavigationEnd)
        this.editing = false;
    });
  }

  profesorPredmeta(): boolean {
    // console.log(this.user?.tip);
    // console.log(this.predmet$.value?.profesori?.map(p => p.id).includes(this.user?.id));

    return this.user!.tip === 'profesor'
      && this.predmet$.value?.profesori?.map(p => p.id).includes(this.user!.id) || false;
  }

  pratiIliOtprati() {
    let result: string = "Serverska greška pri pracenju predmeta.";

    this.userService.pratiIliOtprati(this.predmet$.value!).subscribe({
      next: (res: ApiMsg) => {
        if (this.user) {
          console.log(res.msg);

          if (this.predmet$.value!.pracen)
            this.user!.praceniPredmeti = this.user!.praceniPredmeti?.filter(p => p.id != this.predmet$.value!.id);
          else {
            this.user!.praceniPredmeti?.push(this.predmet$.value!);
            this.user!.praceniPredmeti?.sort((p1, p2) => p1.semestar - p2.semestar);
          }
          this.predmet$.value!.pracen = !this.predmet$.value!.pracen;

          this.authService.getUserObserver().next(this.user);

          result = res.msg;
        }
      },
      error: (err: ApiMsg) => {
        result = err.msg;
      },
      complete: () => {
        this._snackbar.open(result, 'OK', {
          ...constants.snackbarPosition,
          duration: 3000
        });

      }
    });
  }

  onIzmeni() {
    this.editing = !this.editing;
    console.log(this.editing);
  }

  onDelete() {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, { data: { title: "Sigurno obrisati predmet?" } });
    dialogRef.afterClosed().subscribe(res => {
      if (!res) return;

      this.predmetService.obrisiPredmet(this.predmet$.value!.id!).subscribe({
        next: msg => {
          this._snackbar.open(msg.msg, 'OK', { ...constants.snackbarPosition, duration: 3000 });
          this.predmetService.predmet$ = new BehaviorSubject<Predmet | null>(null);

          this.authService.getUserObserver().value!.praceniPredmeti = this.authService.user.praceniPredmeti!.filter(p => p.id !== this.predmet$.value!.id);

          this.authService.getUserObserver().next(this.authService.getUserObserver().value);
          this.router.navigate(['/pocetna']);
        },
        error: (err: apiError) => {
          console.log(err.error.msg);
          this._snackbar.open("Došlo je do serverske greške", '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
        }
      })

    })

  }

  onSacuvajIzmene(predmet: PredmetEdit) {

    console.log(predmet);

    this.predmetService.izmeniPredmet(predmet).subscribe({
      next: (p) => {
        console.log(p);
        p.oblasti! = p.oblasti!.sort(o => o.redniBr)!;
        this._snackbar.open('Uspešno izmenjene informacije o predmetu', 'OK', { ...constants.snackbarPosition, duration: 3000 });
        this.predmetService.predmet$.next({ ...p, pracen: this.predmet$.value!.pracen });
        this.userService.fetchPraceniPredmeti(); // da se azurira side-nav ako promeni semestar
        this.predmet$.next({ ...p, pracen: this.predmet$.value!.pracen });
        this.predmetService.izabraniBlanket$.next(null);
        this.editing = false;
      },
      error: (err: apiError) => {
        console.log(err);
        this._snackbar.open("Došlo je do serverske greške", '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
      }
    });
  }

  onCancelEditing() {
    this.editing = false;
  }

}

