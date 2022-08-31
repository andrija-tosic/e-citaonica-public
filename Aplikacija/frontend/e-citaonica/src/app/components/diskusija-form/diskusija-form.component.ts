import { BreakpointObserver } from '@angular/cdk/layout';
import { noWhitespaceValidator } from 'src/app/validators/noWhiteSpaceValidator';
import { Router, NavigationEnd } from '@angular/router';
import { ComponentCanDeactivate } from 'src/app/guards/pending-changes.guard';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatChip } from '@angular/material/chips';
import { ReplaySubject, BehaviorSubject, Observable, map, shareReplay } from 'rxjs';
import { Diskusija } from 'src/app/models/diskusija.model';
import { DiskusijaAdd } from 'src/app/models/DTOs/diskusija.add.dto';
import { Predmet } from 'src/app/models/predmet.model';
import { Zadatak } from 'src/app/models/zadatak.model';
import { PredmetService } from 'src/app/services/predmet.service';
import { OdaberiKolegeDialogComponent } from '../odaberi-kolege-dialog/odaberi-kolege-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Preporuka } from 'src/app/models/DTOs/preporuka.dto';

@Component({
  selector: 'diskusija-form',
  templateUrl: './diskusija-form.component.html',
  styleUrls: ['./diskusija-form.component.scss']
})
export class DiskusijaFormComponent implements OnInit, ComponentCanDeactivate {
  @ViewChild('naslov') naslovRef: ElementRef;
  @Input('submitLabel') submitLabel: string = 'Postavi';
  @Input('hasCancel') hasCancel: boolean = false;
  @Input('zadatak') zadatak: Zadatak | undefined;
  @Input('diskusija') diskusija: Diskusija | null = null;
  @Input('hasDescription') hasDescription: boolean = true;
  @Input('descLabel') descLabel: string = 'Pokreni novu diskusiju'
  @Input('hasOsobe') hasOsobe: boolean = false;
  @Output('onSubmit') onSubmit = new EventEmitter<DiskusijaAdd>();
  @Output('onCancel') onCancel = new EventEmitter<void>();

  form: FormGroup;
  odabraneOsobe: Preporuka[] = [];
  predmet$: BehaviorSubject<Predmet | null>;
  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(
    private fb: FormBuilder,
    private predmetService: PredmetService,
    private dialog: MatDialog,
    private breakpointObserver: BreakpointObserver
  ) {
    this.predmet$ = predmetService.getPredmet();
  }

  @HostListener('window:beforeunload')
  canDeactivate(): boolean | Observable<boolean> {
    return false;
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [],
      naslov: ['', [Validators.required, noWhitespaceValidator]],
      sadrzaj: ['', [Validators.required, noWhitespaceValidator]],
      tip: ['oblast'],
      oblastiIds: [null, Validators.required],
      zadatakId: [this.zadatak?.id],
      dodaci: [[]]
    })

    if (this.diskusija) this.patchValues();
    else if (this.zadatak) {
      this.form.patchValue({
        tip: 'zadatak',
        zadatakId: this.zadatak.id
      });
      this.form.get('oblastiIds')?.clearValidators();
    }

    window.setTimeout(() => {
      this.naslovRef.nativeElement.focus();
    })
  }

  patchValues() {
    const oblastiIds: number[] = [];
    this.diskusija?.oblasti?.forEach(o => {
      if (o.id) oblastiIds.push(o.id)
    }
    );

    if (this.diskusija?.tip == 'zadatak') {
      this.form.get('oblastiIds')?.clearValidators();
      this.zadatak = this.diskusija.zadatak;
      console.log(this.zadatak);
    }

    this.form.patchValue({
      id: this.diskusija?.id,
      naslov: this.diskusija?.naslov,
      sadrzaj: this.diskusija?.sadrzaj,
      tip: this.diskusija?.tip,
      oblastiIds: oblastiIds.length > 0 ? oblastiIds : null,
      dodaci: this.diskusija?.dodaci,
      zadatakId: this.zadatak?.id
    });

    this.form.updateValueAndValidity();
  }

  get tip() {
    return this.form.get('tip')?.value;
  }

  get oblasti() {
    return this.form.get('oblastiIds') as FormControl;
  }

  toggleSelection(chip: MatChip) {
    chip.toggleSelected();
    if (chip.selected) {
      if (this.oblasti?.value == null) this.oblasti?.patchValue([]);
      this.oblasti?.value.push(chip.value);
    }
    else {
      this.oblasti.setValue(this.oblasti?.value.filter((el: any) => el != chip.value));
      if ((this.oblasti?.value as number[]).length == 0) this.oblasti.patchValue(null);
    }
    if (!this.oblasti?.touched)
      this.oblasti?.markAsTouched();
    this.oblasti?.updateValueAndValidity();
  }

  onSubmitForm() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return this.form.updateValueAndValidity();
    }
    this.onSubmit.emit({
      id: this.form.get('id')?.value,
      tip: this.form.get('tip')?.value,
      naslov: this.form.get('naslov')?.value,
      oblastiIds: this.form.get('oblastiIds')?.value,
      sadrzaj: this.form.get('sadrzaj')?.value,
      dodaci: this.form.get('dodaci')?.value,
      zadatakId: this.form.get('zadatakId')?.value,
      osobeIds: this.odabraneOsobe.map(o => o.id)
    })
  }

  showDialog() {
    const dialogRef = this.dialog.open(OdaberiKolegeDialogComponent, {
      maxWidth: '500px',
      width: '90%',
      data: this.odabraneOsobe
    }).afterClosed().subscribe(res => {
      if (res === false) return;
      this.odabraneOsobe = res;
    })
  }
}
