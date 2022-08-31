import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { map, Observable, shareReplay } from 'rxjs';
import { Komentar } from 'src/app/models/komentar.model';
import { noWhitespaceValidator } from 'src/app/validators/noWhiteSpaceValidator';

@Component({
  selector: 'comment-form',
  templateUrl: './comment-form.component.html',
  styleUrls: ['./comment-form.component.scss']
})
export class CommentFormComponent implements OnInit {
  @Input() komentar: Komentar | undefined;
  @Input() buttonLabel: string = 'Postavi';
  @Input() hasCancel: boolean = true;
  @Input() clearOnSubmit: boolean = true;
  @Input() hasPredlog: boolean = false;
  @Input() validateOnSubmit: boolean = false;
  @Input() disabled: boolean = false;
  @Output()
  onSubmit = new EventEmitter<Komentar>();
  @Output()
  onCancel = new EventEmitter<void>()

  form: FormGroup;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );


  constructor(private fb: FormBuilder,
    private breakpointObserver: BreakpointObserver) {
  }

  ngOnInit(): void {
    console.log(this.komentar);
    this.form = this.fb.group({
      predlogResenja: [false],
      sadrzaj: [this.komentar?.sadrzaj ? this.komentar.sadrzaj : null],
      dodaci: [this.komentar?.dodaci ? this.komentar.dodaci : []],
    })

    if (!this.validateOnSubmit) this.form.get('sadrzaj')?.setValidators([Validators.required, noWhitespaceValidator]);
  }

  submitForm() {
    this.form.updateValueAndValidity();
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    if (this.validateOnSubmit) {
      const control = this.form.get('sadrzaj');
      if (noWhitespaceValidator(control as FormControl)) {
        this.form.get('sadrzaj')?.setErrors({ 'incorrect': true });
        return;
      }
    }

    this.onSubmit.emit({
      id: this.komentar?.id || 0,
      predlogResenja: Boolean(this.form.get('predlogResenja')?.value),
      sadrzaj: this.form.get('sadrzaj')?.value,
      dodaci: this.form.get('dodaci')?.value,
      datumKreiranja: new Date(),
      brZahvalnica: 0
    })

    if (this.clearOnSubmit) {
      this.form.patchValue({
        sadrzaj: null,
        dodaci: [],
        predlogResenja: false
      })
      this.form.markAsPristine();
    }
  }

  onPostavi() {
    this.submitForm();
  }
}
