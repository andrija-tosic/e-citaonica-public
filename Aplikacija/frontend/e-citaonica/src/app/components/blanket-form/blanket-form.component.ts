import { BreakpointObserver } from '@angular/cdk/layout';
import { noWhitespaceValidator } from 'src/app/validators/noWhiteSpaceValidator';
import { ComponentCanDeactivate } from 'src/app/guards/pending-changes.guard';
import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatChip } from '@angular/material/chips';
import { ReplaySubject, BehaviorSubject, Observable, map, shareReplay } from 'rxjs';
import { Blanket } from 'src/app/models/blanket.model';
import { IspitniRok } from 'src/app/models/ispitni-rok.model';
import { Predmet } from 'src/app/models/predmet.model';

@Component({
  selector: 'blanket-form',
  templateUrl: './blanket-form.component.html',
  styleUrls: ['./blanket-form.component.scss']
})
export class BlanketFormComponent implements OnInit, ComponentCanDeactivate {
  form: FormGroup;
  predmetId: number;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );


  @Input() blanket: Blanket | null = null;
  @Input() buttonLabel: string = 'Postavi blanket';
  @Input() hasCancel: boolean = false;
  @Input() ispitniRokovi$: ReplaySubject<IspitniRok[]>;
  @Input() predmet$: BehaviorSubject<Predmet | null>;

  @Output() onSubmit = new EventEmitter<Blanket>();
  @Output() onCancel = new EventEmitter<void>();
  constructor(
    private fb: FormBuilder,
    private breakpointObserver: BreakpointObserver
  ) {
  }

  @HostListener('window:beforeunload')
  canDeactivate(): boolean | Observable<boolean> {
    return false;
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [0],
      ispitniRokId: [1, Validators.required],
      tip: ['pismeni', Validators.required],
      datum: [new Date(), Validators.required],
      zadaci: this.fb.array([
        this.createZadatak(),
      ], Validators.required)
    })

    this.onUpdateTip({ value: 'pismeni' }); // da bi checkbox za "Zadatak" bio selektovan umesto "Pitanje"
    this.form.get('tip')?.patchValue('pismeni'); // da bude nesto inicijalno selektovano

    if (this.blanket) this.patchForm();

  }

  patchForm() {
    this.form.patchValue({
      ispitniRokId: this.blanket?.ispitniRok.id,
      tip: this.blanket?.tip,
      datum: this.blanket?.datum,
      id: this.blanket?.id
    })

    this.zadaci.removeAt(0);
    this.blanket?.zadaci?.forEach(zadatak => {
      let oblasti: any[] = [];
      zadatak.oblasti.forEach(o => oblasti.push(o.id));
      this.zadaci.push(this.fb.group({
        id: zadatak.id,
        redniBr: zadatak.redniBr,
        tip: zadatak.tip,
        tekst: [zadatak.tekst, noWhitespaceValidator],
        brPoena: [zadatak.brPoena, [Validators.required, Validators.min(5)]],
        oblastiIds: [oblasti.length > 0 ? oblasti : null, Validators.required]
      }))
    })
  }

  get zadaci() {
    return this.form?.get('zadaci') as FormArray;
  }

  get tip() {
    return this.form?.get('tip') as FormControl;
  }

  createZadatak() {
    const redniBr = this.zadaci ? this.zadaci.length + 1 : 1;
    return this.fb.group({
      id: [0],
      redniBr: [redniBr],
      tip: [this.tip?.value === 'pismeni' ? 'zadatak' : 'pitanje'],
      tekst: ['', noWhitespaceValidator],
      brPoena: [25, [Validators.required, Validators.min(5)]],
      oblastiIds: [null, Validators.required],
    })
  }

  addZadatak() {
    this.zadaci.push(this.createZadatak());
  }

  oblastiZadatak(i: number) {
    return this.zadaci.at(i)?.get('oblastiIds') as FormControl;
  }

  toggleSelection(zadatak: number, chip: MatChip) {
    chip.toggleSelected();
    let oblasti: FormControl = this.oblastiZadatak(zadatak);
    if (chip.selected) {
      if (oblasti.value == null) oblasti.patchValue([]);
      (oblasti.value as Array<number>).push(chip.value);
    }
    else {
      oblasti.setValue((oblasti.value as Array<number>).filter((el) => el != chip.value));
      if ((oblasti.value as Array<number>).length == 0) oblasti.patchValue(null);
    }
    if (!oblasti.touched) oblasti.markAsTouched();
    oblasti.updateValueAndValidity();
  }

  onDeleteZadatak(i: number) {
    this.zadaci.removeAt(i);
    this.zadaci.markAsDirty();
    this.zadaci.markAllAsTouched();
  }

  onAddBlanket() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return this.form.updateValueAndValidity();
    }

    this.onSubmit.emit(this.form.value);
    // this.predmetService.dodajBlanket({predmetId : this.predmetId, ...this.form.value})
    // .subscribe({
    //   next: (res) => { 
    //       console.log(res);
    //       this.router.navigate(['../blanketi'], { relativeTo: this.route});
    //     },
    //     error: (err : apiMsg) => console.log(err.msg)
    //   });
  }

  onUpdateTip(event: any) {
    for (let i = 0; i < this.zadaci.length; i++) {
      if (event.value === 'pismeni')
        this.zadaci.controls[i].patchValue({ tip: 'zadatak' })
      else if (event.value === 'teorijski')
        this.zadaci.controls[i].patchValue({ tip: 'pitanje' })
    }
  }
}
