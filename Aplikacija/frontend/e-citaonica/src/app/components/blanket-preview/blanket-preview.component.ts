import { AuthService } from './../../services/auth.service';
import { UserService } from './../../services/user.service';
import { ComponentCanDeactivate } from 'src/app/guards/pending-changes.guard';
import { DeleteConfirmationDialogComponent } from '../delete-confirmation-dialog/delete-confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { constants, AUTORIZACIJA_DISABLED } from './../../constants';
import { apiError } from "../../types/apiError.type";
import { BreakpointObserver } from '@angular/cdk/layout';
import { PredmetService } from 'src/app/services/predmet.service';
import { Component, Input, OnInit, Output } from '@angular/core';
import { first, map, Observable, ReplaySubject, shareReplay, BehaviorSubject } from 'rxjs';
import { Blanket } from 'src/app/models/blanket.model';
import { Predmet } from 'src/app/models/predmet.model';
import { IspitniRok } from 'src/app/models/ispitni-rok.model';
import { EventEmitter } from '@angular/core';
import { throws } from 'assert';
import { UserBasic } from 'src/app/models/user-basic.model';

@Component({
  selector: 'blanket-preview',
  templateUrl: './blanket-preview.component.html',
  styleUrls: ['./blanket-preview.component.scss']
})

export class BlanketPreviewComponent implements OnInit, ComponentCanDeactivate {

  @Input('blanket') blanket$: BehaviorSubject<Blanket | null> = new BehaviorSubject<Blanket | null>(null); /* = {
    'ispitniRok': { 'id': 1, naziv: 'Januarski'},
    'datum': '2022-01-01',
    'tip': 'Teorijski'
  };
  */
  predmet$: BehaviorSubject<Predmet | null>;
  ispitniRokovi$: ReplaySubject<IspitniRok[]>;

  @Input('zaVezbu') zaVezbu: boolean = false;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  isEditing: boolean = false;
  isExpanded: boolean = false;
  @Output() expandOrShrink = new EventEmitter<boolean>();

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  elapsedSeconds: number;
  elapsedTime: string;
  displayedTime: string;
  interval: NodeJS.Timeout;
  timerStarted: boolean = false;
  timerPaused: boolean = false;
  timerStopped: boolean = false;

  user: UserBasic | null;

  constructor(
    private predmetService: PredmetService,
    private breakpointObserver: BreakpointObserver,
    private _snackbar: MatSnackBar, private dialog: MatDialog, public authService: AuthService) {
    this.predmet$ = predmetService.getPredmet();
    this.ispitniRokovi$ = predmetService.getIspitniRokovi();

    this.authService.getUserObserver().subscribe(u => this.user = u);
  }
  canDeactivate(): boolean | Observable<boolean> {
    return !(this.timerStarted || this.isEditing);
  }

  profesorPredmeta(): boolean {
    // console.log(this.user?.tip);
    // console.log(this.predmet$.value?.profesori?.map(p => p.id).includes(this.user?.id));

    return this.user!.tip === 'profesor'
      && this.predmet$.value!.profesori!.map(p => p.id).includes(this.user!.id);
  }


  stopTimer() {
    clearInterval(this.interval);
    this.timerStarted = false;
    this.timerStopped = true;
    this.timerPaused = false;

    this.displayedTime = "Potrebno vreme: " + this.elapsedTime;
  }

  togglePauseTimer() {
    this.timerPaused = !this.timerPaused;

    if (this.timerPaused)
      this.displayedTime = "Pauza: " + this.elapsedTime;
  }

  startTimer() {
    this.elapsedTime = "0 s";
    this.displayedTime = '' + this.elapsedTime;
    clearInterval(this.interval);
    this.elapsedSeconds = 0;

    this.interval = setInterval(() => {
      if (this.timerStarted && !this.timerPaused && !this.timerStopped) {
        this.elapsedSeconds++;

        const seconds = this.elapsedSeconds;
        const minutes = Math.floor(this.elapsedSeconds / 60);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 60);

        this.elapsedTime = '';

        if (days)
          this.elapsedTime += days + "d ";

        if (hours)
          this.elapsedTime += hours + 'h '

        if (minutes)
          this.elapsedTime += minutes + 'min '

        this.elapsedTime += seconds + 's '

        this.displayedTime = this.elapsedTime;
      }

    }, 1000);

    this.timerStarted = true;
    this.timerStopped = false;
  }


  ngOnInit(): void {
    // this.blanket$.subscribe(b => console.log(b))

    this.predmetService.izabraniBlanket$.subscribe(_ => {
      this.timerStarted = false;
      this.timerStopped = true;
      this.displayedTime = '';
    })
  }

  onExpandOrShrink() {
    this.expandOrShrink.emit(this.isExpanded);
    this.isExpanded = !this.isExpanded;
  }

  onIzmeni() {
    this.isEditing = !this.isEditing;
  }

  onObrisi() {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent,
      {
        data: { title: "Sigurno obrisati blanket?" }
      });
    dialogRef.afterClosed().subscribe(res => {
      if (!res) return;

      this.predmetService.obrisiBlanket(this.blanket$.value?.id!).subscribe({
        next: msg => {
          this._snackbar.open(msg.msg, 'OK', { ...constants.snackbarPosition, duration: 3000 });
          this.predmetService.predmet$.pipe(first()).subscribe(p => {

            p!.blanketi = p?.blanketi?.filter(b => b.id !== this.blanket$.value?.id);
            this.predmetService.predmet$.next(p);
          }
          );

          this.predmetService.izabraniBlanket$.next(null);
        },
        error: (err: apiError) => {
          console.log(err.error.msg);
          this._snackbar.open("Došlo je do serverske greške.", '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
        }
      })

    })

  }

  onCancelEditing() {
    this.isEditing = false;
  }

  onSacuvajIzmene(blanket: any) {
    this.predmetService.izmeniBlanket(blanket).subscribe({
      next: (blanket: Blanket) => {
        this._snackbar.open('Blanket uspešno izmenjen.', 'OK', { ...constants.snackbarPosition, duration: 3000 });
        this.predmetService.izabraniBlanket$.next(blanket);
        this.predmetService.getBlanketi();

        this.isEditing = false;
      },
      error: (err: apiError) => {
        console.log(err.error.msg);
        this._snackbar.open("Došlo je do serverske greške.", '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
      }
    });
  }
}
