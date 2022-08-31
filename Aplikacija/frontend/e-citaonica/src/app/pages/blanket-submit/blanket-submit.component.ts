import { constants } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ComponentCanDeactivate } from './../../guards/pending-changes.guard';
import { apiError } from "../../types/apiError.type";
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { first, Observable, ReplaySubject, BehaviorSubject } from 'rxjs';
import { ApiMsg } from "src/app/types/apiMsg.type";
import { Blanket } from 'src/app/models/blanket.model';
import { IspitniRok } from 'src/app/models/ispitni-rok.model';
import { Predmet } from 'src/app/models/predmet.model';
import { PredmetService } from 'src/app/services/predmet.service';

@Component({
  selector: 'blanket-submit',
  templateUrl: './blanket-submit.component.html',
  styleUrls: ['./blanket-submit.component.scss']
})
export class BlanketSubmitComponent implements OnInit, ComponentCanDeactivate {
  ispitniRokovi$: ReplaySubject<IspitniRok[]>;
  predmet$: BehaviorSubject<Predmet | null>;
  private predmetId: number;
  isFinished = false;

  constructor(
    private predmetService: PredmetService,
    private router: Router,
    private route: ActivatedRoute,
    private _snackbar: MatSnackBar
  ) {

  }
  canDeactivate(): boolean | Observable<boolean> {
    return this.isFinished;
  }

  ngOnInit(): void {
    this.ispitniRokovi$ = this.predmetService.getIspitniRokovi();
    this.predmet$ = this.predmetService.getPredmet();

    this.predmet$.subscribe(predmet => this.predmetId = predmet?.id!);
  }

  onAddBlanket(blanket: Blanket) {
    this.predmetService.dodajBlanket({ predmetId: this.predmetId, ...blanket })
      .subscribe({
        next: (rezultujuciBlanket) => {
          this.isFinished = true;
          console.log(rezultujuciBlanket);

          this._snackbar.open('Blanket uspešno dodat.', 'OK', { ...constants.snackbarPosition, duration: 3000 });

          this.predmetService.izabraniBlanket$.next(rezultujuciBlanket);

          this.predmetService.predmet$.pipe(first()).subscribe(p => {
            p?.blanketi?.push(rezultujuciBlanket);
            this.router.navigate(['../'], { relativeTo: this.route });
          })
        },
        error: (err: apiError) => console.log(err)
      });
  }
}
