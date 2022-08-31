import { UserBasic } from './../../models/user-basic.model';
import { AuthService } from './../../services/auth.service';
import { ComponentCanDeactivate } from './../../guards/pending-changes.guard';
import { constants, AUTORIZACIJA_DISABLED } from './../../constants';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiService } from './../../services/api.service';
import { Modul } from 'src/app/models/modul.model';
import { IzmenaSlikeDialogComponent } from './../izmena-slike-dialog/izmena-slike-dialog.component';
import { PredmetEdit } from './../../models/DTOs/predmet.edit.dto';
import { DeleteConfirmationDialogComponent } from './../delete-confirmation-dialog/delete-confirmation-dialog.component';
import { ReplaySubject, BehaviorSubject, Observable } from 'rxjs';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild, HostListener } from '@angular/core';
import { Predmet } from 'src/app/models/predmet.model';
import { FormBuilder, Validators, FormGroup, FormArray, FormControl } from '@angular/forms';
import { Profesor } from 'src/app/models/profesor.model';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Oblast } from 'src/app/models/oblast.model';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from 'src/app/services/user.service';
import { MatChip } from '@angular/material/chips';
import { ThrottlingUtils } from '@azure/msal-common';
import { noWhitespaceValidator } from 'src/app/validators/noWhiteSpaceValidator';
import { O } from '@angular/cdk/keycodes';

@Component({
  selector: 'predmet-form',
  templateUrl: './predmet-form.component.html',
  styleUrls: ['./predmet-form.component.scss']
})
export class PredmetFormComponent implements OnInit, ComponentCanDeactivate {
  @Input() predmet: Predmet | null = null;
  @Input() buttonLabel: string = 'Dodaj predmet';
  @Input() hasCancel: boolean = false;

  form: FormGroup;

  ostaliProfesoriFetched: boolean = false;
  predmetniProfesori: Profesor[] = [];
  ostaliProfesori: Profesor[];
  displayedProfs: Profesor[];
  moduli$: BehaviorSubject<Modul[] | null> = new BehaviorSubject<Modul[] | null>(null);

  subject: BehaviorSubject<string> = new BehaviorSubject<string>('');
  user$: BehaviorSubject<UserBasic | null>;

  izabraniModuli: Modul[] = [];

  AUTORIZACIJA_DISABLED = AUTORIZACIJA_DISABLED;

  @Output() onSubmit = new EventEmitter<PredmetEdit>();
  @Output() onCancel = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    public userService: UserService,
    private apiService: ApiService,
    private _snackbar: MatSnackBar,
    private authService: AuthService) {

    this.user$ = authService.getUserObserver();

    this.form = this.fb.group({
      id: [0],
      semestar: [1, Validators.required],
      naziv: ['', noWhitespaceValidator],
      opis: ['', noWhitespaceValidator],
      query: [''],
      oblasti: this.fb.array([
        this.createOblast(),
      ], Validators.required),
      moduliIds: [[], Validators.required],
      profesoriIds: [[], Validators.required]
    });
  }

  get oblasti() {
    return this.form?.get('oblasti') as FormArray;
  }

  createOblast() {
    const redniBr = this.oblasti?.length + 1;
    return this.fb.group({
      id: [0],
      redniBr: [redniBr],
      naziv: ['', Validators.required],
    });
  }

  samoOpstiModulIzabran(): { min: number, max: number } {
    if (this.izabraniModuli.length === 1 && this.izabraniModuli[0].naziv === 'Opšti') {
      return { min: 1, max: 2 };
    }
    else {
      return { min: 3, max: 8 };
    }
  }

  @HostListener('window:beforeunload')
  canDeactivate(): Observable<boolean> | boolean {
    return false;
  }

  cancel() {
    this.dialog.open(DeleteConfirmationDialogComponent,
      {
        data:
        {
          title: "Niste sačuvali promene. Ako napustite stranicu, promene će biti poništene. Sigurno napustiti stranicu?"
        }
      }
    ).afterClosed().subscribe(res => {
      if (res) {
        this.onCancel.emit();
      }
    });
  }

  private patchForm(p: Predmet) {
    this.form.patchValue({
      id: p.id,
      naziv: p.naziv,
      semestar: p.semestar,
      opis: p.opis,
      moduliIds: p.moduli!.map(m => m.id),
      profesoriIds: p.profesori!.map(p => p.id),
    });
  }

  ngOnInit(): void {

    this.oblasti.clear();
    if (this.predmet) {
      console.log()
      this.patchForm(this.predmet);
      this.izabraniModuli = [...this.predmet?.moduli!];
      this.predmet!.oblasti! = this.predmet!.oblasti!.sort((o1, o2) => o1.redniBr - o2.redniBr);
      this.predmetniProfesori = [...this.predmet?.profesori!];

      this.predmet?.oblasti?.forEach(o => {
        this.oblasti.push(this.fb.group({
          id: o.id,
          redniBr: o.redniBr,
          naziv: [o.naziv, Validators.required],
        }))
      });
    }
    else {
      this.oblasti.push(this.fb.group({
        id: 0,
        redniBr: 1,
        naziv: ['Nova oblast', Validators.required],
      }));

    }

    this.apiService.getModuli().subscribe(moduli => this.moduli$.next(moduli));

    // console.log(this.ostaliProfesoriFetched);
    if (!this.ostaliProfesoriFetched) {


      if (this.predmet)
        this.userService.profesoriSaOstalihPredmeta(this.predmet?.id!).subscribe(profesori => {
          this.ostaliProfesoriFetched = true;
          this.ostaliProfesori = profesori;
          this.displayedProfs = this.ostaliProfesori.slice(0, 5);
          // console.log(this.ostaliProfesori);
        });
      else
        this.userService.getProfesori().subscribe(profesori => {
          this.ostaliProfesoriFetched = true;
          this.ostaliProfesori = profesori;
          this.displayedProfs = this.ostaliProfesori.slice(0, 5);
          // console.log(this.ostaliProfesori);
        });

    }

    this.subject.subscribe(_ => this.sortProfs(this.ostaliProfesori));
  }

  onAddPredmet() {
    if (this.izabraniModuli.length === 0) {
      this._snackbar.open(`Morate izabrati neki modul.`, '', {
        ...constants.snackbarPosition, duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });

    }




    // console.log(this.izabraniModuli.filter(m => m.naziv !== 'Opšti'));
    // invalid forma ako je pored opstog modula jos neki odabran
    if (this.izabraniModuli.filter(m => m.naziv === 'Opšti').length > 0
      && this.izabraniModuli.length > 1) {
      this._snackbar.open(`Predmet ne može biti na opštom modulu ako je i na nekom od ostalih.`, '', {
        ...constants.snackbarPosition, duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });
      return this.form.updateValueAndValidity();

    }

    if (this.predmetniProfesori.length === 0 && this.oblasti.length === 0) {
      this._snackbar.open(`Morate dodeliti neku oblast i nekog profesora predmetu.`, '', {
        ...constants.snackbarPosition, duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });
      return this.form.updateValueAndValidity();

    }
    else if (this.predmetniProfesori.length === 0) {
      this._snackbar.open(`Morate dodeliti nekog profesora predmetu.`, '', {
        ...constants.snackbarPosition, duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });
      return this.form.updateValueAndValidity();

    }
    else if (this.oblasti.length === 0) {
      this._snackbar.open(`Morate dodeliti neku oblast predmetu.`, '', {
        ...constants.snackbarPosition, duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });
      return this.form.updateValueAndValidity();
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return this.form.updateValueAndValidity();
    }

    this.onSubmit.emit(this.form.value);
  }


  dodajOblast() {
    this.oblasti.push(this.fb.group({
      id: 0,
      redniBr: this.oblasti.length + 1,
      naziv: ['', Validators.required],
    }));

    // mnogo los kod, ali ne vidim drugi nacin...
    setTimeout(() => {
      const oblastInputs = document.querySelectorAll("#oblast-input") as NodeListOf<HTMLElement>;
      oblastInputs[oblastInputs.length - 1].focus();

    }, 100)

  }

  obrisiOblast(i: number) {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, { data: { title: "Sigurno obrisati oblast?" } });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult == true) {
        this.oblasti.removeAt(i);
        this.oblasti.markAsDirty();
        this.oblasti.markAllAsTouched();
      }
    });

  }

  selektujModul(chip: MatChip, modul: Modul) {

    const index = this.izabraniModuli.map(m => m.id).indexOf(modul.id);

    // ako se izabere opsti, da se resetuje semestar na 1.
    if (modul.naziv == "Opšti" && this.izabraniModuli.filter(m => m.id !== modul.id).length === 0) {
      this.form.get('semestar')?.patchValue(1);
    }
    else {
      this.form.get('semestar')?.patchValue(3);
    }


    if (index >= 0) {
      this.izabraniModuli.splice(index, 1);
    } else {
      // samo opsti izabran ili ostali bez opstog
      if ((modul.naziv == "Opšti" && this.izabraniModuli.filter(m => m.id !== modul.id).length > 0)
        || (modul.naziv !== 'Opšti' && this.izabraniModuli.filter(m => m.naziv === 'Opšti').length === 1)) {
        this._snackbar.open(`Predmet ne može biti na opštom modulu ako je i na nekom od ostalih.`, '', {
          ...constants.snackbarPosition, duration: 3000,
          panelClass: ['mat-toolbar', 'mat-warn']
        });

        return;
      }

      this.izabraniModuli.push(modul);
    }

    this.form.get('moduliIds')?.patchValue(this.izabraniModuli.map(m => m.id));

    chip.toggleSelected();
  }


  matChipSelected(modul: Modul) {
    return this.izabraniModuli.map(m => m.id).includes(modul.id);
  }

  updateRedneBrojeve(arr: FormGroup[]) {
    let i = 1;

    arr.forEach(el => {
      el.get('redniBr')?.setValue(i++);
    })
  }

  drop(event: CdkDragDrop<Oblast[]>) {

    const oblasti = new Array<FormGroup>(this.oblasti.length)
      .fill({} as any)
      .map((v, index) => this.oblasti.at(index) as FormGroup);

    moveItemInArray(oblasti, event.previousIndex, event.currentIndex);
    this.updateRedneBrojeve(oblasti);

    this.oblasti.patchValue(oblasti.map(o => o.value));
  }


  dropProfesor(event: CdkDragDrop<Profesor[]>) {
    if (event.previousContainer === event.container) {
      console.log('same');
      moveItemInArray(event.container.data!, event.previousIndex, event.currentIndex);
    } else {
      console.log('transfer');
      if (event.previousContainer.id == "pp")
        this.ostaliProfesori.push(this.predmetniProfesori[event.previousIndex]);
      else
        this.ostaliProfesori = this.ostaliProfesori.filter(p => p.id !== this.displayedProfs[event.previousIndex].id)
      console.log(this.ostaliProfesori);
      transferArrayItem(
        event.previousContainer.data!,
        event.container.data!,
        event.previousIndex,
        event.currentIndex,
      );
      this.form.get('profesoriIds')?.patchValue(this.predmetniProfesori.map(p => p.id));
    }
  }

  sortProfs(profs: Profesor[]) {
    let i = 0;
    for (let j = 0; j < profs?.length && i < 5; j++) {
      if (this.checkFilter(profs[j])) {
        this.displayedProfs[i] = profs[j];
        i++;
      }
    }
    this.displayedProfs = this.displayedProfs?.sort((a, b) => a.ime.length - b.ime.length).slice(0, i);
  }

  checkFilter(profesor: Profesor) {
    return profesor.ime.toLowerCase().includes(this.subject.value.toLowerCase())
      || profesor.email.toLowerCase().includes(this.subject.value.toLowerCase());
  }

}