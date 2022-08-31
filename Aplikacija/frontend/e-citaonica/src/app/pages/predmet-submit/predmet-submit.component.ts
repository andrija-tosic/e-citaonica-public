import { Observable } from 'rxjs';
import { ComponentCanDeactivate } from './../../guards/pending-changes.guard';
import { Router } from '@angular/router';
import { constants } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PredmetService } from 'src/app/services/predmet.service';
import { PredmetEdit } from './../../models/DTOs/predmet.edit.dto';
import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { apiError } from 'src/app/types/apiError.type';
import { T } from '@angular/cdk/keycodes';

@Component({
  selector: 'predmet-submit',
  templateUrl: './predmet-submit.component.html',
  styleUrls: ['./predmet-submit.component.scss']
})
export class PredmetSubmitComponent implements OnInit, ComponentCanDeactivate {

  constructor(private location: Location, private predmetService: PredmetService, private _snackbar: MatSnackBar,
    private router: Router) { }

  ngOnInit(): void {
  }

  isEditing: boolean = true;

  onCancelEditing() {
    this.location.back();
  }

  onSacuvajIzmene(predmet: PredmetEdit) {
    this.isEditing = false;

    console.log(predmet);

    this.predmetService.dodajPredmet(predmet).subscribe({
      next: (predmet) => {
        this._snackbar.open(`Uspešno dodat predmet.`, 'OK', { ...constants.snackbarPosition, duration: 3000 });
        this.router.navigate([`/predmet/${predmet.id}`]);
      },
      error: (err: apiError) => {
        console.log(err);
        this._snackbar.open("Došlo je do serverske greške", '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
      }
    });
  }

  canDeactivate(): boolean | Observable<boolean> {
    return !this.isEditing
  }
}
