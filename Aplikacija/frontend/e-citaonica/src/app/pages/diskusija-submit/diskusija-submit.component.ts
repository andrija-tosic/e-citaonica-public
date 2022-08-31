import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatChip } from '@angular/material/chips';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, ReplaySubject, BehaviorSubject } from 'rxjs';
import { ApiMsg } from "src/app/types/apiMsg.type";
import { Diskusija } from 'src/app/models/diskusija.model';
import { DiskusijaAdd } from 'src/app/models/DTOs/diskusija.add.dto';
import { Predmet } from 'src/app/models/predmet.model';
import { PredmetService } from 'src/app/services/predmet.service';
import { Zadatak } from 'src/app/models/zadatak.model';
import { ComponentCanDeactivate } from 'src/app/guards/pending-changes.guard';
import { BrowserRefreshService } from 'src/app/services/browser-refresh.service';
import { constants } from 'src/app/constants';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'diskusija-submit',
  templateUrl: './diskusija-submit.component.html',
  styleUrls: ['./diskusija-submit.component.scss']
})
export class DiskusijaSubmitComponent implements OnInit, ComponentCanDeactivate {
  @ViewChild('naslov') naslovRef: ElementRef;

  predmet$: BehaviorSubject<Predmet | null>;
  zadatak: Zadatak | undefined;
  browserRef: boolean;
  constructor(
    private predmetService: PredmetService,
    private route: ActivatedRoute,
    private router: Router,
    private browserRefresh: BrowserRefreshService,
    private _snackbar : MatSnackBar
  ) {
    this.zadatak = this.router.getCurrentNavigation()?.extras.state as Zadatak;

    this.predmet$ = predmetService.getPredmet();
  }

  isFinished = false;

  ngOnInit(): void {
    this.browserRef = this.browserRefresh.refreshed;
    if (this.browserRef) {
      this.router.navigate(['../'], { relativeTo: this.route })
    };
  }

  canDeactivate(): boolean | Observable<boolean> {
    return this.isFinished || this.browserRef;
  }

  onSubmit(diskusija: DiskusijaAdd) {
    this.predmetService.addDiskusija(diskusija)
      .subscribe({
        next: (diskusija) => {
          this.isFinished = true;
          this.router.navigate([`../${diskusija.id}`], { relativeTo: this.route });
        },
        error: (err: ApiMsg) =>  { 
          console.log(err.msg);
          this._snackbar.open("Došlo je do greške", '', 
            { ...constants.snackbarPosition, 
              duration: 3000, 
              panelClass: ['mat-toolbar', 'mat-warn'] 
            }); 
        }
      });
  }

  onCancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
