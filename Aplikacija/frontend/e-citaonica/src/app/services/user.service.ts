import { Diskusija } from 'src/app/models/diskusija.model';
import { ApiMsg } from "../types/apiMsg.type";
import { User } from 'src/app/models/user.model';
import { HttpClient, HttpContext, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Predmet } from '../models/predmet.model';
import { map, Observable, BehaviorSubject } from 'rxjs';
import { AuthService } from './auth.service';
import { UserBasic } from '../models/user-basic.model';
import { constants } from './../constants';
import { SpinnerSkipHeader } from './spinner-interceptor.service';
import { Profesor } from "../models/profesor.model";
import { NGX_LOADING_BAR_IGNORED } from "@ngx-loading-bar/http-client";
import { Blanket } from '../models/blanket.model';
import { Komentar } from '../models/komentar.model';

interface diskusijaQueryParams {
  skip?: number,
  dated?: Date
}

interface profilQueryParams {
  komskip?: number,
  disskip?: number,
  komod?: Date,
  disod?: Date
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  user: User;

  public user$: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);

  constructor(
    private http: HttpClient,
    private authService: AuthService) {
    // this.authService.getUserObserver()
    //   .subscribe((user) => {
    //     this.user = user as User;
    //   });
  }

  fetchUser(id: number) {
    return this.http.get<User>(`${environment.apiEndpoint}/korisnici/${id}`);
  }

  fetchPraceniPredmeti() {
    const id = this.authService.getUserObserver().value!.id;
    this.http.get<Predmet[]>(`${environment.apiEndpoint}/predmeti/praceni/${id}`)
      .subscribe((res) => {
        if (res) {
          console.log("fetched praceni predmeti", res);
          this.authService.getUserObserver().value!.praceniPredmeti = res;
          this.authService.getUserObserver().next(this.authService.getUserObserver().value);

          // this.authService.getUserObserver().next(this.user);
        }
      });
  }

  pratiIliOtprati(predmet: Predmet): Observable<ApiMsg> {
    const id = this.authService.getUserObserver().value!.id;
    return this.http.get<ApiMsg>(`${environment.apiEndpoint}/predmeti/prati-otprati/${id}/${predmet.id}`, constants.httpBar/*{ headers }*/);
  }

  profesoriSaOstalihPredmeta(predmetId: number) {
    return this.http.get<Profesor[]>(`${environment.apiEndpoint}/korisnici/profesori-koji-nisu-sa-predmeta/${predmetId}`);
  }

  getProfesori() {
    return this.http.get<Profesor[]>(`${environment.apiEndpoint}/korisnici/profesori`);
  }

  getDiskusijeKorisnika(id: number) {
    return this.http.get<Diskusija[]>(`${environment.apiEndpoint}/korisnici/diskusije/${id}`);
  }

  getObjaveKorisnika(id: number, query?: profilQueryParams) {
    let params = new HttpParams();
    if (query?.komod) params = params.set('komod', new Date(query.komod)?.toISOString());
    if (query?.disod) params = params.set('disod', new Date(query.disod)?.toISOString());
    if (query?.disskip) params = params.set('disskip', query.disskip);
    if (query?.komskip) params = params.set('komskip', query.komskip);

    return this.http.get<Array<Diskusija | Komentar>>(`${environment.apiEndpoint}/korisnici/objave/${id}`, { params: params });
  }

  getPreporuceneDiskusije(query?: diskusijaQueryParams) {
    let params = new HttpParams();
    if (query?.dated) params = params.set('dated', new Date(query.dated)?.toISOString());
    if (query?.skip) params = params.set('skip', query.skip);
    
    return this.http.get<Diskusija[]>(`${environment.apiEndpoint}/diskusije/preporucene-diskusije`, { 
      params: params
    });
  }


  getPreporuceniBlanketi(praceniPredmetiIds: number[]) {
    return this.http.put<Blanket[]>(`${environment.apiEndpoint}/blanketi/preporuceni-blanketi`, praceniPredmetiIds, constants.httpOptions);
  }

}
