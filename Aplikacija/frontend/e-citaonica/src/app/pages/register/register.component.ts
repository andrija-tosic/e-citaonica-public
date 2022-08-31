import { apiError } from "../../types/apiError.type";
import { BehaviorSubject, map, Observable, of, shareReplay } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { constants } from '../../constants';
import { ApiMsg } from "../../types/apiMsg.type";
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Modul } from 'src/app/models/modul.model';
import { AuthService } from 'src/app/services/auth.service';
import { IspitniRok } from 'src/app/models/ispitni-rok.model';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { noWhitespaceValidator } from "src/app/validators/noWhiteSpaceValidator";
import { FluentValidationError } from "src/app/types/fluentValidationError.type";

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  isLinear: boolean = false;
  form: FormGroup;
  moduli: BehaviorSubject<Modul[]> = new BehaviorSubject<Modul[]>([]);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe('(max-width: 1000px)')
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private _snackBar: MatSnackBar,
    private api: ApiService,
    private breakpointObserver: BreakpointObserver,
  ) {

    this.form = fb.group({
      ime: ['', noWhitespaceValidator],
      prezime: ['', noWhitespaceValidator],
      email: ['', [Validators.email, noWhitespaceValidator]],
      password: ['', [noWhitespaceValidator, Validators.minLength(8)]],
      passwordRep: ['', [noWhitespaceValidator, Validators.minLength(8)]],
      isProfesor: [false],
      indeks: ['', [Validators.required, Validators.min(1000)]],
      modul: ['', Validators.required],
      godina: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.api.getModuli().subscribe({
      next: (res) => {
        this.moduli.next(res);
      },
      error: (err: apiError) => {
        console.log(err.error.msg);
      }
    });
  }

  jeOpsti(): boolean {

    const opstiId = this.moduli.value.find(m => m.naziv == "Opšti")?.id;

    return (this.form.controls['modul'].value == opstiId);
  }


  onSubmit(formDirective: FormGroupDirective) {
    if ((this.form.get('password')?.value as string).localeCompare(this.form.get('passwordRep')?.value as string) !== 0) {
      this._snackBar.open("Lozinke se ne poklapaju.", "", {
        ...constants.snackbarPosition,
        duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      });
      this.form.markAllAsTouched();
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this._snackBar.open("Podaci nisu validni.", "", {
        ...constants.snackbarPosition,
        duration: 3000,
        panelClass: ['mat-toolbar', 'mat-warn']
      })
        .afterDismissed().subscribe(() => {
          console.log("snack dismissed");
        }
        );
      return;
    }

    this.authService.register(this.form.value)
      .subscribe({
        next: () => {
          this._snackBar.open('Registracija uspešna! Na vaš email poslat je link za verifikaciju',
            'OK', {
            ...constants.snackbarPosition,
            duration: 3000
          });

          this.router.navigate(['/login']);
        },
        error: (err: FluentValidationError | apiError) => {
          if ('msg' in err.error) {
            this._snackBar.open(err.error.msg, '', {
              ...constants.snackbarPosition,
              duration: 3000,
              panelClass: ['mat-toolbar', 'mat-warn']
            });
          }
          else {
            console.log(err);
            this._snackBar.open(Object.entries(err.error.errors)[0][1], '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
          }
        }

      });
    // mnogo teze testiranje ako ima reset
    // this.form.reset();
  }

  getErrorMessage() {
    if (this.form.controls['email'].hasError('required')) {
      return 'Morate uneti email.';
    }

    return this.form.controls['email'].hasError('email') ? 'Email adresa nije validna.' : '';
  }

  lozinkeSePoklapaju() {
    return this.form.controls['password'].value.localeCompare(this.form.controls['passwordRep'].value);
  }

  get studentDisabled(): boolean {
    return this.form.get('isProfesor')?.value;
  }

  profesorCheckChanged() {
    if (this.studentDisabled) {
      this.form.get('indeks')?.disable();
      this.form.get('modul')?.disable();
      this.form.get('godina')?.disable();
    }
    else {
      this.form.get('indeks')!.enable();
      this.form.get('modul')!.enable();
      this.form.get('godina')!.enable();
    }
  }

  onOpenId() {
    this.authService.openIdLogin();
  }
}
