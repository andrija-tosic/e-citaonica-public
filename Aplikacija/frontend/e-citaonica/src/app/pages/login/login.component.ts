import { constants } from '../../constants';
import { apiError } from "../../types/apiError.type";
import { ApiMsg } from "../../types/apiMsg.type";
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { noWhitespaceValidator } from 'src/app/validators/noWhiteSpaceValidator';
import { FluentValidationError } from 'src/app/types/fluentValidationError.type';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  errorMessage: string = '';
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private _snackBar: MatSnackBar) {
    this.form = fb.group({
      email: ['', [Validators.email, noWhitespaceValidator]],
      password: ['', [Validators.minLength(8), noWhitespaceValidator]]
    })
  }

  getErrorMessage() {
    if (this.form.controls['email'].hasError('required')) {
      return 'Morate uneti email';
    }

    return this.form.controls['email'].hasError('email') ? 'Email adresa nije validna' : '';
  }

  ngOnInit(): void {
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.login(this.form.value)
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
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
  }

  onOpenId() {
    this.authService.openIdLogin();
  }
}
