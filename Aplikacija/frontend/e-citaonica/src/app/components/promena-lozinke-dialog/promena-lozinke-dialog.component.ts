import { constants } from './../../constants';
import { FluentValidationError } from "../../types/fluentValidationError.type"
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from 'src/app/services/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { apiError } from 'src/app/types/apiError.type';

@Component({
  selector: 'promena-lozinke-dialog',
  templateUrl: './promena-lozinke-dialog.component.html',
  styleUrls: ['./promena-lozinke-dialog.component.scss']
})
export class PromenaLozinkeDialogComponent implements OnInit {


  form: FormGroup;
  errorMessage: string = '';

  constructor(public dialogRef: MatDialogRef<PromenaLozinkeDialogComponent>, private fb: FormBuilder,
    private auth: AuthService, private _snackbar: MatSnackBar) {
    this.form = fb.group({
      oldPassword: ['', [Validators.required]],
      newPassword: ['', Validators.required],
      newPasswordAgain: ['', Validators.required]
    })
  }

  getErrorMessage() {

  }

  ngOnInit(): void {
  }

  onConfirm(): void {
    this.auth.changePassword(this.form.value).subscribe({
      next: res => {
        console.log(res);
        this.dialogRef.close(res.msg);
      },
      error: (err: FluentValidationError | apiError) => {
        if ('msg' in err.error) {
          this._snackbar.open(err.error.msg, '', {
            ...constants.snackbarPosition,
            duration: 3000,
            panelClass: ['mat-toolbar', 'mat-warn']
          });
        }
        else {
          console.log(err);
          this._snackbar.open(Object.entries(err.error.errors)[0][1], '', { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
        }
      }
    });
  }

  onDismiss(): void {
    this.dialogRef.close(null);
  }


}
