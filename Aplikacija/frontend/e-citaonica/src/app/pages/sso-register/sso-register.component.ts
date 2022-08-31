import { ApiService } from './../../services/api.service';
import { constants } from '../../constants';
import { apiError } from "../../types/apiError.type";
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from 'src/app/services/auth.service';
import { subscribeOn, Subscriber, Observable, of, BehaviorSubject } from 'rxjs';
import { Modul } from 'src/app/models/modul.model';

@Component({
  selector: 'sso-register',
  templateUrl: './sso-register.component.html',
  styleUrls: ['./sso-register.component.scss']
})
export class SsoRegisterComponent implements OnInit {
  isLinear: boolean = false;

  form: FormGroup;
  moduli: BehaviorSubject<Modul[]> = new BehaviorSubject<Modul[]>([]);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private _snackBar: MatSnackBar,
    private api: ApiService) {

    this.form = fb.group({
      indeks: ['', Validators.required],
      modul: ['', Validators.required],
      godina: ['', Validators.required]
    })
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

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this._snackBar.open("Podaci nisu validni.", "", { ...constants.snackbarPosition, duration: 3000, panelClass: ['mat-toolbar', 'mat-warn'] });
      return;
    }
    else {
      this.authService.openIdRegister(this.form.value);
    }
  }
}
