import { Title } from '@angular/platform-browser';
import { constants } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Blanket } from 'src/app/models/blanket.model';
import { OblastFilterService } from './../../services/oblast-filter.service';
import { BehaviorSubject, first } from 'rxjs';
import { PredmetService } from 'src/app/services/predmet.service';
import { Predmet } from 'src/app/models/predmet.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'vezbanje',
  templateUrl: './vezbanje.component.html',
  styleUrls: ['./vezbanje.component.scss']
})
export class VezbanjeComponent implements OnInit {

  predmet$: BehaviorSubject<Predmet | null> = new BehaviorSubject<Predmet | null>(null);
  blanket$: BehaviorSubject<Blanket | null> = new BehaviorSubject<Blanket | null>(null);

  constructor(private predmetService: PredmetService,
    private oblastFilterService: OblastFilterService,
    private _snackbar: MatSnackBar,
    private title: Title) { }

  ngOnInit(): void {
    this.predmetService.predmet$.subscribe(p => {
      this.title.setTitle(`${p?.naziv} - Vežbanje • e-Čitaonica`)
      this.predmet$.next(p);
    });

    this.predmetService.generisaniBlanket$.subscribe(b => this.blanket$.next(b));
  }

  generisi() {
    if (this.oblastFilterService.selektovaneOblasti$.value.length === 0) {
      this._snackbar.open('Morate odabrati neku od oblasti.', '', {
        ...constants.snackbarPosition,
        duration: 3000, panelClass: ['mat-toolbar', 'mat-warn']
      });

      return;
    }

    if (this.oblastFilterService.selektovaniTipovi$.value.length === 0) {
      this._snackbar.open('Morate odabrati neki tip zadatka.', '', {
        ...constants.snackbarPosition,
        duration: 3000, panelClass: ['mat-toolbar', 'mat-warn']
      });

      return;
    };

    this.predmetService.getGenerisanBlanket(
      {
        oblastiIds: this.oblastFilterService.selektovaneOblasti$.value.map(o => o.id!),
        tipovi: this.oblastFilterService.selektovaniTipovi$.value,
        predmetId: this.predmet$.value?.id!
      }).subscribe({
        next: blanket => {
          console.log(blanket.zadaci);
          // this.blanket$.next(blanket);
          this.predmetService.generisaniBlanket$.next(blanket);
        },
        error: msg => {
          this._snackbar.open('Ovaj predmet još nema dodate blankete sa zadacima iz izabranih oblasti.', '', {
            ...constants.snackbarPosition,
            duration: 3000, panelClass: ['mat-toolbar', 'mat-warn']
          });

        }
      });
  }

}
