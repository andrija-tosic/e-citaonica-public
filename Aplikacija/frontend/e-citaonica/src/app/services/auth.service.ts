import { ApiMsg } from "../types/apiMsg.type";
import { User } from './../models/user.model';
import { environment } from './../../environments/environment';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MsalService, MSAL_GUARD_CONFIG } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';
import { BehaviorSubject, NotFoundError, tap } from 'rxjs';
import { UserBasic } from '../models/user-basic.model';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Obavestenje } from "../models/obavestenje.model";

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
}

interface loginData {
  user: UserBasic,
  token: string
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _user: UserBasic;
  private user$: BehaviorSubject<UserBasic | null> =
    new BehaviorSubject<UserBasic | null>(null);
  private _tokenIssuer: string;

  constructor(
    private msalService: MsalService,
    public jwtHelper: JwtHelperService,
    private http: HttpClient,
    private router: Router,
  ) {
    this.msalService.handleRedirectObservable().subscribe({
      next: (response: AuthenticationResult) => this.redirectHandler(response),
    });
  }

  public isAuthenticated(): boolean {
    const token: any = localStorage.getItem('token');
    const user: any = localStorage.getItem('logged-user');

    return user && !this.jwtHelper.isTokenExpired(token);
  }


  getAuthToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserObserver(): BehaviorSubject<UserBasic | null> {
    return this.user$;
  }

  get user() {
    return this._user;
  }

  get tokenIssuer() {
    return this._tokenIssuer;
  }

  setTokenIssuer() {
    this._tokenIssuer = this.jwtHelper
      .decodeToken(localStorage.getItem('token') || '')?.['iss']
      ?.includes('microsoft') ? 'microsoft' : 'citaonica';
  }

  register(data: any) {
    return this.http.post(`${environment.apiEndpoint}/korisnici/register`, {
      ime: data['ime'],
      prezime: data['prezime'],
      email: data['email'],
      lozinka: data['password'],
      jeProfesor: data['isProfesor'],
      indeks: data['indeks'],
      modulId: data['modul'],
      godina: data['godina']
    }, httpOptions);
  }

  openIdRegister(model: any | null) {
    return this.http.post<UserBasic>(`${environment.apiEndpoint}/korisnici/sso-register`, {
      indeks: model['indeks'],
      godina: model['godina'],
      modulId: model['modul'],
      jeProfesor: model['isProfesor'],
    }, httpOptions)
      .pipe(tap(user => {
        localStorage.setItem('logged-user', JSON.stringify(user));
        this._user = user;
        this.user$.next(user);
      }))
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (err: any) => {
          console.log(err);
        }
      });
  }

  confirmEmail(model: any) {
    return this.http.post(`${environment.apiEndpoint}/korisnici/confirm-register`, model, httpOptions);
  }

  login(model: any) {
    return this.http.post<loginData>(`${environment.apiEndpoint}/korisnici/login`, model, httpOptions)
      .pipe(tap((res => {
        this._user = res.user;
        localStorage.setItem('token', res.token);
        this.setTokenIssuer();
        localStorage.setItem('logged-user', JSON.stringify(res.user));
        this.user$.next(res.user);
      }))
    );
  }

  updateUser(user: User) {
    localStorage.setItem('logged-user', JSON.stringify(user));
    this._user = user;
    this.user$.next(user);
  }

  openIdLogin() {
    this.msalService.loginRedirect();
  }

  autoLogin() {
    const locUser: string | null = localStorage.getItem('logged-user');
    if (!locUser) return;
    this._user = JSON.parse(locUser);
    this.setTokenIssuer();
    this.user$.next(this._user);
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  redirectHandler(response: AuthenticationResult) {
    if (!response) return;

    const role = response.account?.idTokenClaims?.roles;
    localStorage.setItem('token', response.idToken);

    this.http
      .get(
        `${environment.apiEndpoint}/korisnici/sso-login/${response.account?.username}`
      )
      .subscribe({
        next: (user: any) => {
          localStorage.setItem('logged-user', JSON.stringify(user));
          this._user = user;
          this.user$.next(user);
          this.setTokenIssuer();
          
          return this.router.navigate(['/']);
        },
        error: (error: HttpErrorResponse) => {
          if (error.status == 404) {
            if (role?.indexOf('profesor') != -1)
              this.openIdRegister({ "isProfesor": true });
            else
              this.router.navigate(['/sso-register']);
          }
        },
      });
  }

  changePassword(formValue: any) {
    return this.http.put<ApiMsg>(`${environment.apiEndpoint}/korisnici/promena-lozinke`, formValue, httpOptions);
  }

  getObavestenja() {
    return this.http.get<Obavestenje[]>(`${environment.apiEndpoint}/korisnici/obavestenja`);
  }
}
