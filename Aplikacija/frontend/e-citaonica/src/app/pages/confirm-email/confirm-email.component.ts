import { constants } from './../../constants';
import { animate, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmEmail } from 'src/app/models/confirm-email.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)' }),
        animate('200ms ease-in', style({ transform: 'translateY(0%)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'translateY(-100%)' }))
      ])
    ])
  ]
})
export class ConfirmEmailComponent implements OnInit {
  model: ConfirmEmail = { id: 0, token: '' }
  horizontonalPosition: MatSnackBarHorizontalPosition = "center";
  verticalPosition: MatSnackBarVerticalPosition = "top";

  constructor(private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private _snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.model.id = Number(this.route.snapshot.queryParamMap.get('id'));
    this.model.token = this.route.snapshot.queryParamMap.get('token') || '';
    this.confirmEmail();
  }

  confirmEmail() {
    this.authService.confirmEmail(this.model)
      .subscribe({
        next: () => {
          this._snackBar.open('Uspešno potvrđen mail!', 'OK', {
            ...constants.snackbarPosition,
            duration: 3000
          })
          this.router.navigate(['/login']);
        },
        error: (res) => {
          console.log(res);
          this._snackBar.open('Došlo je do greške pri potvrđivanju emaila...', '', {
            ...constants.snackbarPosition,
            duration: 3000,
            panelClass: ['mat-toolbar', 'mat-warn']

          })
          this.router.navigate(['/login']);
        }
      })
  }
}
