import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BehaviorSubject, combineLatestAll, combineLatestWith, debounceTime, Subscription } from 'rxjs';
import { Preporuka } from 'src/app/models/DTOs/preporuka.dto';
import { DiskusijaService } from 'src/app/services/diskusija.service';
import { PredmetService } from 'src/app/services/predmet.service';

@Component({
  selector: 'odaberi-kolege-dialog',
  templateUrl: './odaberi-kolege-dialog.component.html',
  styleUrls: ['./odaberi-kolege-dialog.component.scss']
})
export class OdaberiKolegeDialogComponent implements OnInit, OnDestroy {
  preporuke: Preporuka[] = [];
  odabrani: Preporuka[] = [];
  unos: BehaviorSubject<string> = new BehaviorSubject('');
  subscriptions : Subscription[] = [];
  preporukeLoaded: boolean = false;
  constructor(
    public dialogRef: MatDialogRef<OdaberiKolegeDialogComponent>,
    private predmetService: PredmetService,
    @Inject(MAT_DIALOG_DATA) public _osobe: Preporuka[],
    ) {
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  } 

  ngOnInit(): void {
    if (this._osobe.length > 0) {
      this.odabrani = this._osobe;
    }

    this.unos
      .pipe(debounceTime(300))
      .pipe(combineLatestWith(this.predmetService.getPredmet()))
      .subscribe(([query, predmet]) => {
        this.preporukeLoaded = false;
        if (predmet && predmet?.id)
          this.predmetService.getPreporuke(predmet.id, query, this.odabrani.map(o => o.id)).subscribe(preporuke => {
            this.preporuke = preporuke.filter(p => this.odabrani.findIndex(o => o.id == p.id) == -1);
            this.preporukeLoaded = true;
          })
      })
  }

  onConfirm(): void {
    this.dialogRef.close(this.odabrani);
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }

  ukloni(s : Preporuka) {
    this.preporuke.unshift(s);
    this.odabrani = this.odabrani.filter((o) => o.id != s.id);
  }

  odaberi(s: Preporuka) {
    this.odabrani.unshift(s);
    this.preporuke = this.preporuke.filter(el => el.id != s.id);
  }
}
